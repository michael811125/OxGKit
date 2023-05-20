using OxGKit.NoticeSystem;
using UnityEngine;

public class CoinInWalletCond : NoticeCondition
{
    #region Defualt
    public static int id { get { return NoticeManager.GetConditionId<CoinInWalletCond>(); } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register()
    {
        NoticeManager.RegisterCondition<CoinInWalletCond>();
    }
    #endregion

    public override bool ShowCondition(object data)
    {
        if (data != null)
        {
            NoticeDemo.Wallet wallet = data as NoticeDemo.Wallet;

            // balance > 0
            if (wallet.coin > 0) return true;
        }

        return false;
    }
}