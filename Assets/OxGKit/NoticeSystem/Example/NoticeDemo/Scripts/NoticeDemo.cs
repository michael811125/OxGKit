using OxGKit.NoticeSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeDemo : MonoBehaviour
{
    public class Wallet
    {
        public int coin;

        public void Reset()
        {
            this.coin = 0;
        }

        public void IncreaseCoin()
        {
            this.coin++;
        }

        public void DecreaseCoin()
        {
            this.coin--;
        }
    }

    public Text coinTxt;
    public List<NoticeItem> noticeItems = new List<NoticeItem>();

    // Reference type
    private Wallet _wallet;
    // Value type
    private int _coin;

    private void Awake()
    {
        // Reference Type
        this._wallet = new Wallet();

        // Register notice items (when register will auto notify)
        this._NotifyAndRegisterNoticeItems();
    }

    private void Update()
    {
        this._UpdateCoin();
    }

    private void _NotifyAndRegisterNoticeItems()
    {
        // [If Invoke twice will renew data and notify again]

        #region Register NoticeItem Conditions
        // Wallet Notice (Including two conditions)
        noticeItems[0].RegisterNotice
        (
             // Value type data
             new NoticeInfo(CoinInWalletCond.id, this._coin),
             // Reference type data
             new NoticeInfo(CoinIsEvenCond.id, this._wallet)
        );

        // Wallet Condition One (coin > 0)
        noticeItems[1].RegisterNotice
        (
             // Value type data
             new NoticeInfo(CoinInWalletCond.id, this._coin)
        );

        // Wallet Condition Two (balance = even)
        noticeItems[2].RegisterNotice
        (
             // Reference type data
             new NoticeInfo(CoinIsEvenCond.id, this._wallet)
        );
        #endregion
    }

    private void _UpdateCoin()
    {
        // Refresh coin display
        if (this.coinTxt != null) this.coinTxt.text = $"${this._wallet.coin}";
    }

    public void ResetCoin()
    {
        // Reset Wallet (Reference Type)
        this._wallet.Reset();

        #region Renew Value Type Data
        // If use value type data must renew data
        this._coin = 0;

        // Assign value type data again, because NoticeItems[0] and [1] contains CoinInWalletCond.id condition
        // [Note: If use Way 2 can don't need to do renew value type data]
        //noticeItems[0].RenewNotice(new NoticeInfo(CoinInWalletCond.id, this._coin));
        //noticeItems[1].RenewNotice(new NoticeInfo(CoinInWalletCond.id, this._coin));
        #endregion

        // [After change condition must notify to check condition and refresh notice display]
        // Efficiency => Way 1 > Way 2 > Way 3 > Way 4
        // Convenient => Way 2 > Way 3 > Way 4 > Way 1

        // Notify Way 1 (Use NoticeManager with condition ids to Notify)
        //NoticeManager.Notify
        //(
        //    CoinInWalletCond.id,
        //    CoinIsEvenCond.id
        //);

        // Notify Way 2 (Register with notify again)
        this._NotifyAndRegisterNoticeItems();

        // Notify Way 3 (Use NoticeManager with notice items to Notify)
        //NoticeManager.Notify(this.noticeItems.ToArray());

        // Notify Way 4 (Call notify by notice item directly)
        //foreach (var noticeItem in this.noticeItems)
        //{
        //    noticeItem.Notify();
        //}
    }

    public void IncreaseCoin()
    {
        // Increase Coin (Reference Type)
        this._wallet.IncreaseCoin();

        #region Renew Value Type Data
        // If use value type data must renew data
        this._coin++;

        // Assign value type data again, because NoticeItems[0] and [1] contains CoinInWalletCond.id condition
        // [Note: If use Way 2 can don't need to do renew value type data]
        //noticeItems[0].RenewNotice(new NoticeInfo(CoinInWalletCond.id, this._coin));
        //noticeItems[1].RenewNotice(new NoticeInfo(CoinInWalletCond.id, this._coin));
        #endregion

        // [After change condition must notify to check condition and refresh notice display]
        // Efficiency => Way 1 > Way 2 > Way 3 > Way 4
        // Convenient => Way 2 > Way 3 > Way 4 > Way 1

        // Notify Way 1 (Use NoticeManager with condition ids to Notify)
        //NoticeManager.Notify
        //(
        //    CoinInWalletCond.id,
        //    CoinIsEvenCond.id
        //);

        // Notify Way 2 (Register with notify again)
        this._NotifyAndRegisterNoticeItems();

        // Notify Way 3 (Use NoticeManager with notice items to Notify)
        //NoticeManager.Notify(this.noticeItems.ToArray());

        // Notify Way 4 (Call notify by notice item directly)
        //foreach (var noticeItem in this.noticeItems)
        //{
        //    noticeItem.Notify();
        //}
    }

    public void DecreaseCoin()
    {
        // Decrease Coin (Reference Type)
        this._wallet.DecreaseCoin();

        #region Renew Value Type Data
        // If use value type data must renew data
        this._coin--;

        // Assign value type data again, because NoticeItems[0] and [1] contains CoinInWalletCond.id condition
        // [Note: If use Way 2 can don't need to do renew value type data]
        //noticeItems[0].RenewNotice(new NoticeInfo(CoinInWalletCond.id, this._coin));
        //noticeItems[1].RenewNotice(new NoticeInfo(CoinInWalletCond.id, this._coin));
        #endregion

        // [After change condition must notify to check condition and refresh notice display]
        // Efficiency => Way 1 > Way 2 > Way 3 > Way 4
        // Convenient => Way 2 > Way 3 > Way 4 > Way 1

        // Notify Way 1 (Use NoticeManager with condition ids to Notify)
        //NoticeManager.Notify
        //(
        //    CoinInWalletCond.id,
        //    CoinIsEvenCond.id
        //);

        // Notify Way 2 (Register with notify again)
        this._NotifyAndRegisterNoticeItems();

        // Notify Way 3 (Use NoticeManager with notice items to Notify)
        //NoticeManager.Notify(this.noticeItems.ToArray());

        // Notify Way 4 (Call notify by notice item directly)
        //foreach (var noticeItem in this.noticeItems)
        //{
        //    noticeItem.Notify();
        //}
    }
}
