using UnityEngine;

namespace OxGKit.ActionSystem
{
    internal static class Logger
    {
        public static void OnDone(string targetActionName, string ownerActionName)
        {
            Debug.Log($"<color=#9cffd6>[{nameof(ActionSystem)}] <color=#ffc41c>{targetActionName}</color> is done and removed from <color=#1cff77>{ownerActionName}</color>.</color>");
        }

        public static void OnRemove(string targetActionName, int uid, string ownerActionName)
        {
            Debug.Log($"<color=#9cffd6>[{nameof(ActionSystem)}] <color=#ff2ed0>UID: {uid}</color> Removed <color=#ffc41c>{targetActionName}</color> from <color=#1cff77>{ownerActionName}</color></color>");
        }
    }
}
