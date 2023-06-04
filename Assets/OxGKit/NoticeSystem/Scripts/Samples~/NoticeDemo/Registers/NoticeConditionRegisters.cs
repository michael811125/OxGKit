using OxGKit.NoticeSystem;

public static class NoticeConditionRegisters
{
    static NoticeConditionRegisters()
    {
        NoticeManager.RegisterCondition<CoinInWalletCond>();
        NoticeManager.RegisterCondition<CoinIsEvenCond>();
    }

    /// <summary>
    /// Manually trigger static constructor
    /// </summary>
    public static void Init() { }
}