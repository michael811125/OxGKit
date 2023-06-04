using OxGKit.NoticeSystem;
using System;

public class CoinInWalletCond : NoticeCondition
{
    #region Defualt
    public static int id { get { return NoticeManager.GetConditionId<CoinInWalletCond>(); } }
    #endregion

    public override bool ShowCondition(object data)
    {
        if (data != null)
        {
            //NoticeDemo.Wallet wallet = data as NoticeDemo.Wallet;
            int coin = Convert.ToInt32(data);

            // balance > 0
            //if (wallet.coin > 0) return true;
            if (coin > 0) return true;
        }

        return false;
    }
}