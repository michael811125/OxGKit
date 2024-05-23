using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace OxGKit.InfiniteScrollView.Editor
{
    public static class InfiniteScrollViewCreatePrefabEditor
    {
        class DoCreatePrefabAsset : EndNameEditAction
        {
            // Subclass and override this method to create specialised prefab asset creation functions
            protected virtual GameObject CreateGameObject(string name)
            {
                return new GameObject(name);
            }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                GameObject go = CreateGameObject(Path.GetFileNameWithoutExtension(pathName));
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, pathName);
                GameObject.DestroyImmediate(go);
            }
        }

        class DoCreateRectTransformPrefabAsset : DoCreatePrefabAsset
        {
            protected override GameObject CreateGameObject(string name)
            {
                var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));

                // calibrate RectTransform
                RectTransform objRect = obj.GetComponent<RectTransform>();
                objRect.anchorMin = new Vector2(0.5f, 0.5f);
                objRect.anchorMax = new Vector2(0.5f, 0.5f);
                objRect.sizeDelta = new Vector2(128, 128);
                objRect.localScale = Vector3.one;
                objRect.localPosition = Vector3.zero;

                return obj;
            }
        }

        static void CreatePrefabAsset(string name, DoCreatePrefabAsset createAction)
        {
            string directory = GetSelectedAssetDirectory();
            string path = Path.Combine(directory, $"{name}.prefab");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, createAction, path, EditorGUIUtility.FindTexture("Prefab Icon"), null);
        }

        static string GetSelectedAssetDirectory()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Directory.Exists(path))
                return path;
            else
                return Path.GetDirectoryName(path);
        }

        [MenuItem("Assets/Create/OxGKit/Infinite ScrollView/Template Infinite Cell (RectTransform Prefab)", isValidateFunction: false, priority: 51)]
        public static void CreateTplRectTransformCP()
        {
            CreatePrefabAsset("NewTplInfiniteCell", ScriptableObject.CreateInstance<DoCreateRectTransformPrefabAsset>());
        }
    }
}