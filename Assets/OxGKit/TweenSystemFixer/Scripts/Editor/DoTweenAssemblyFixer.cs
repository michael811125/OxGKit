using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace OxGKit.TweenSystemFixer.Editor
{
    public static class DoTweenAssemblyFixer
    {
        private const string _doTweenModulesFileName = "DOTween.Modules.asmdef";
        private const string _doTweenModulesFixedGUID = "fdf3e181e62e9d243a7fee5ce890ab71";

        [MenuItem("Assets/OxGKit TweenSystem GUID Fixer/Search And Reassign DOTween.Modules GUID", false, -999)]
        public static void RepairDoTweenModulesGUID()
        {
            _DoReassignFixedGUIDForDOTweenModulesAssembly();
        }

        private static void _DoReassignFixedGUIDForDOTweenModulesAssembly()
        {
            var assetGUIDs = AssetGUIDAssigner.ExtractGUIDs(Selection.assetGUIDs, true);

            bool found = false;
            foreach (var guid in assetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(assetPath);
                if (assetPath.IndexOf(_doTweenModulesFileName) != -1 && fileName == _doTweenModulesFileName)
                {
                    found = true;
                    AssetDatabase.StartAssetEditing();
                    AssetGUIDAssigner.ReassignGUID(new string[] { guid }, _doTweenModulesFixedGUID);
                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                }
            }

            if (!found) Debug.LogError($"Cannot found {_doTweenModulesFileName}, repair failed");
        }

        /// <summary>
        /// Reference https://github.com/jeffjadulco/unity-guid-regenerator
        /// </summary>
        internal class AssetGUIDAssigner
        {
            // Basically, we want to limit the types here (e.g. "t:GameObject t:Scene t:Material").
            // But to support ScriptableObjects dynamically, we just include the base of all assets which is "t:Object"
            private const string SearchFilter = "t:Object";

            // Set to "Assets/" folder only. We don't want to include other directories of the root folder
            private static readonly string[] SearchDirectories = { "Assets" };

            // Searches for Directories and extracts all asset guids inside it using AssetDatabase.FindAssets
            public static string[] ExtractGUIDs(string[] selectedGUIDs, bool includeFolders)
            {
                var finalGuids = new List<string>();
                foreach (var guid in selectedGUIDs)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (IsDirectory(assetPath))
                    {
                        string[] searchDirectory = { assetPath };

                        if (includeFolders) finalGuids.Add(guid);
                        finalGuids.AddRange(AssetDatabase.FindAssets(SearchFilter, searchDirectory));
                    }
                    else
                    {
                        finalGuids.Add(guid);
                    }
                }

                return finalGuids.ToArray();
            }

            public static void ReassignGUID(string[] selectedGUIDs, string fixedGUID)
            {
                var assetGUIDs = AssetDatabase.FindAssets(SearchFilter, SearchDirectories);

                var updatedAssets = new Dictionary<string, int>();
                var skippedAssets = new List<string>();

                var inverseReferenceMap = new Dictionary<string, HashSet<string>>();

                /*
                * PREPARATION PART 1 - Initialize map to store all paths that have a reference to our selectedGUIDs
                */
                foreach (var selectedGUID in selectedGUIDs)
                {
                    inverseReferenceMap[selectedGUID] = new HashSet<string>();
                }

                /*
                 * PREPARATION PART 2 - Scan all assets and store the inverse reference if contains a reference to any selectedGUI...
                 */
                var scanProgress = 0;
                var referencesCount = 0;
                foreach (var guid in assetGUIDs)
                {
                    scanProgress++;
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (IsDirectory(path)) continue;

                    var dependencies = AssetDatabase.GetDependencies(path);
                    foreach (var dependency in dependencies)
                    {
                        EditorUtility.DisplayProgressBar($"Scanning guid references on:", path, (float)scanProgress / assetGUIDs.Length);

                        var dependencyGUID = AssetDatabase.AssetPathToGUID(dependency);
                        if (inverseReferenceMap.ContainsKey(dependencyGUID))
                        {
                            inverseReferenceMap[dependencyGUID].Add(path);

                            // Also include .meta path. This fixes broken references when an FBX uses external materials
                            var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(path);
                            inverseReferenceMap[dependencyGUID].Add(metaPath);

                            referencesCount++;
                        }
                    }
                }

                var countProgress = 0;

                foreach (var selectedGUID in selectedGUIDs)
                {
                    var newGUID = fixedGUID;
                    try
                    {
                        /*
                         * PART 1 - Replace the GUID of the selected asset itself. If the .meta file does not exists or does not match the guid (which shouldn't happen), do not proceed to part 2
                         */
                        var assetPath = AssetDatabase.GUIDToAssetPath(selectedGUID);
                        var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);

                        if (!File.Exists(metaPath))
                        {
                            skippedAssets.Add(assetPath);
                            throw new FileNotFoundException($"The meta file of selected asset cannot be found. Asset: {assetPath}");
                        }

                        var metaContents = File.ReadAllText(metaPath);

                        // Check if guid in .meta file matches the guid of selected asset
                        if (!metaContents.Contains(selectedGUID))
                        {
                            skippedAssets.Add(assetPath);
                            throw new ArgumentException($"The GUID of [{assetPath}] does not match the GUID in its meta file.");
                        }

                        // Allow regenerating guid of folder because modifying it doesn't seem to be harmful
                        // if (IsDirectory(assetPath)) continue;

                        // Skip scene files
                        if (assetPath.EndsWith(".unity"))
                        {
                            skippedAssets.Add(assetPath);
                            continue;
                        }

                        var metaAttributes = File.GetAttributes(metaPath);
                        var bIsInitiallyHidden = false;

                        // If the .meta file is hidden, unhide it temporarily
                        if (metaAttributes.HasFlag(FileAttributes.Hidden))
                        {
                            bIsInitiallyHidden = true;
                            HideFile(metaPath, metaAttributes);
                        }

                        metaContents = metaContents.Replace(selectedGUID, newGUID);
                        File.WriteAllText(metaPath, metaContents);

                        if (bIsInitiallyHidden) UnhideFile(metaPath, metaAttributes);

                        if (IsDirectory(assetPath))
                        {
                            // Skip PART 2 for directories as they should not have any references in assets or scenes
                            updatedAssets.Add(AssetDatabase.GUIDToAssetPath(selectedGUID), 0);
                            continue;
                        }

                        /*
                         * PART 2 - Update the GUID for all assets that references the selected GUID
                         */
                        var countReplaced = 0;
                        var referencePaths = inverseReferenceMap[selectedGUID];
                        foreach (var referencePath in referencePaths)
                        {
                            countProgress++;

                            EditorUtility.DisplayProgressBar($"Regenerating GUID: {assetPath}", referencePath, (float)countProgress / referencesCount);

                            if (IsDirectory(referencePath)) continue;

                            var contents = File.ReadAllText(referencePath);

                            if (!contents.Contains(selectedGUID)) continue;

                            contents = contents.Replace(selectedGUID, newGUID);
                            File.WriteAllText(referencePath, contents);

                            countReplaced++;
                        }

                        updatedAssets.Add(AssetDatabase.GUIDToAssetPath(selectedGUID), countReplaced);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    finally
                    {
                        EditorUtility.ClearProgressBar();
                    }
                }

                if (EditorUtility.DisplayDialog($"Fixed {_doTweenModulesFileName} GUID",
                    $"Reassigned {_doTweenModulesFileName} GUID. \nSee console logs for detailed report.", "Done"))
                {
                    var message = $"Reassigned {_doTweenModulesFileName} GUID to fixed GUID: {_doTweenModulesFixedGUID}\n";
                    message = updatedAssets.Aggregate(message, (current, kvp) => current + $"{kvp.Value} references => {kvp.Key}\n");
                    Debug.Log($"{message}");
                }
            }

            public static void HideFile(string path, FileAttributes attributes)
            {
                attributes &= ~FileAttributes.Hidden;
                File.SetAttributes(path, attributes);
            }

            public static void UnhideFile(string path, FileAttributes attributes)
            {
                attributes |= FileAttributes.Hidden;
                File.SetAttributes(path, attributes);
            }

            public static bool IsDirectory(string path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }
    }
}