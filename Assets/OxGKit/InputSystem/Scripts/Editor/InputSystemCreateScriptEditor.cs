using UnityEditor;

namespace OxGKit.InputSystem.Editor
{
    public static class InputSystemCreateScriptEditor
    {
        private const string TPL_INPUT_BINDING_COMPOSITE_SCRIPT_PATH = "TplScripts/TplInputBindingComposite.cs.txt";
        private const string TPL_INPUT_ACTION_SCRIPT_PATH = "TplScripts/TplInputAction.cs.txt";

        // find current file path
        private static string pathFinder
        {
            get
            {
                var g = AssetDatabase.FindAssets("t:Script InputSystemCreateScriptEditor");
                return AssetDatabase.GUIDToAssetPath(g[0]);
            }
        }

        [MenuItem(itemName: "Assets/Create/OxGKit/Input System/New Input System (Extension)/Template Input Binding Composite.cs (For Unity New Input System)", isValidateFunction: false, priority: 51)]
        public static void CreateScriptTplInputBindingComposite()
        {
            string currentPath = pathFinder;
            string finalPath = currentPath.Replace("InputSystemCreateScriptEditor.cs", "") + TPL_INPUT_BINDING_COMPOSITE_SCRIPT_PATH;

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(finalPath, "NewTplInputBindingComposite.cs");
        }

        [MenuItem(itemName: "Assets/Create/OxGKit/Input System/Template Input Action.cs (Input Interface For Any)", isValidateFunction: false, priority: 51)]
        public static void CreateScriptTplInputAction()
        {
            string currentPath = pathFinder;
            string finalPath = currentPath.Replace("InputSystemCreateScriptEditor.cs", "") + TPL_INPUT_ACTION_SCRIPT_PATH;

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(finalPath, "NewTplInputAction.cs");
        }
    }
}
