using OxGKit.LoggingSystem;

namespace OxGKit.ActionSystem
{
    internal static class Reporter
    {
        public static void OnDone(string targetActionName, string ownerActionName)
        {
            Logging.PrintInfo<Logger>($"[{nameof(ActionSystem)}] {targetActionName} is done and removed from {ownerActionName}.");
        }

        public static void OnRemove(string targetActionName, int uid, string ownerActionName)
        {
            Logging.Print<Logger>($"[{nameof(ActionSystem)}] UID: {uid} Removed {targetActionName} from {ownerActionName}");
        }
    }
}
