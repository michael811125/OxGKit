using OxGKit.NoticeSystem;

public static class NoticeConditionRegister
{
    static NoticeConditionRegister()
    {
        NoticeManager.RegisterCondition<CoinInWalletCond>();
        NoticeManager.RegisterCondition<CoinIsEvenCond>();
    }

    /// <summary>
    /// Manually trigger static constructor
    /// </summary>
    public static void Init() { }
}