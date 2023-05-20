using UnityEditor;

namespace OxGKit.NoticeSystem.Editor
{
    public static class NoticeSystemCreateScriptEditor
    {
        private const string TPL_NOTICE_CONDITION_SCRIPT_PATH = "TplScripts/TplNoticeCondition.cs.txt";

        // find current file path
        private static string pathFinder
        {
            get
            {
                var g = AssetDatabase.FindAssets("t:Script NoticeSystemCreateScriptEditor");
                return AssetDatabase.GUIDToAssetPath(g[0]);
            }
        }

        [MenuItem(itemName: "Assets/Create/OxGKit/Notice System/Template Notice Condition.cs", isValidateFunction: false, priority: 51)]
        public static void CreateScriptTplNoticeCondition()
        {
            string currentPath = pathFinder;
            string finalPath = currentPath.Replace("NoticeSystemCreateScriptEditor.cs", "") + TPL_NOTICE_CONDITION_SCRIPT_PATH;

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(finalPath, "NewTplNoticeCondition.cs");
        }
    }
}
