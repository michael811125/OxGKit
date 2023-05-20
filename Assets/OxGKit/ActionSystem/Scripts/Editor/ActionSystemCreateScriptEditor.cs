using UnityEditor;

namespace OxGKit.ActionSystem.Editor
{
    public static class ActionSystemCreateScriptEditor
    {
        private const string TPL_ACTION_SCRIPT_PATH = "TplScripts/TplAction.cs.txt";

        private static string pathFinder
        {
            get
            {
                var g = AssetDatabase.FindAssets("t:Script ActionSystemCreateScriptEditor");
                return AssetDatabase.GUIDToAssetPath(g[0]);
            }
        }

        [MenuItem(itemName: "Assets/Create/OxGKit/Action System/Template Action.cs", isValidateFunction: false, priority: 51)]
        public static void CreateScriptTplAction()
        {
            string currentPath = pathFinder;
            string finalPath = currentPath.Replace("ActionSystemCreateScriptEditor.cs", "") + TPL_ACTION_SCRIPT_PATH;

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(finalPath, "NewTplAction.cs");
        }
    }
}
