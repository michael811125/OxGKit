using OxGKit.NoticeSystem;
using UnityEngine;

public class CoinIsEvenCond : NoticeCondition
{
    #region Defualt
    public static int id { get { return NoticeManager.GetConditionId<CoinIsEvenCond>(); } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register()
    {
        NoticeManager.RegisterCondition<CoinIsEvenCond>();
    }
    #endregion

    public override bool ShowCondition(object refData)
    {
        if (refData != null)
        {
            NoticeDemo.Wallet wallet = refData as NoticeDemo.Wallet;

            // deficit
            if (wallet.coin <= 0) return false;
            // balance = even
            else if ((wallet.coin % 2) == 0) return true;
        }

        return false;
    }
}