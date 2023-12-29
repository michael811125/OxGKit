using UnityEditor;

namespace OxGKit.InfiniteScrollView.Editor
{
    public static class InfiniteScrollViewCreateScriptEditor
    {
        private const string TPL_INFINITE_CELL_SCRIPT_PATH = "TplScripts/TplInfiniteCell.cs.txt";

        // find current file path
        private static string _pathFinder
        {
            get
            {
                var g = AssetDatabase.FindAssets("t:Script InfiniteScrollViewCreateScriptEditor");
                return AssetDatabase.GUIDToAssetPath(g[0]);
            }
        }

        [MenuItem(itemName: "Assets/Create/OxGKit/Infinite ScrollView/Template Infinite Cell.cs", isValidateFunction: false, priority: 51)]
        public static void CreateScriptTplInfiniteCell()
        {
            string currentPath = _pathFinder;
            string finalPath = currentPath.Replace("InfiniteScrollViewCreateScriptEditor.cs", "") + TPL_INFINITE_CELL_SCRIPT_PATH;

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(finalPath, "NewTplInfiniteCell.cs");
        }
    }
}
