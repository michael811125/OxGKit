using OxGKit.LoggingSystem;

namespace OxGKit.ActionSystem
{
    internal static class Reporter
    {
        public static void OnDone(string targetActionName, string ownerActionName)
        {
            Logging.Print<Logger>($"<color=#9cffd6>[{nameof(ActionSystem)}] <color=#ffc41c>{targetActionName}</color> is done and removed from <color=#1cff77>{ownerActionName}</color>.</color>");
        }

        public static void OnRemove(string targetActionName, int uid, string ownerActionName)
        {
            Logging.Print<Logger>($"<color=#9cffd6>[{nameof(ActionSystem)}] <color=#ff2ed0>UID: {uid}</color> Removed <color=#ffc41c>{targetActionName}</color> from <color=#1cff77>{ownerActionName}</color></color>");
        }
    }
}
