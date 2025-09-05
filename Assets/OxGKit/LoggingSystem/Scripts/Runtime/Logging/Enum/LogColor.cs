namespace OxGKit.LoggingSystem
{
    public enum LogColor
    {
        Disabled = 0,   // 不啟用顏色
        Enabled = 1,    // 一律啟用顏色
        EditorOnly = 2  // 只在 Editor 啟用顏色 (釋出時自動剔除)
    }
}