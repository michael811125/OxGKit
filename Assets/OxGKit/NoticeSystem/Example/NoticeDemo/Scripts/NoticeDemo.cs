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
        this._wallet = new Wallet();
    }

    private void OnEnable()
    {
        this._InitNoticeItem();
    }

    private void Update()
    {
        this._UpdateCoin();
    }

    private void _InitNoticeItem()
    {
        // Init Wallet
        this._wallet.Reset();

        #region Register NoticeItem Conditions
        // Wallet Notice
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
        // Reset Wallet 
        this._wallet.Reset();

        #region Renew Value Type Data
        // If use value type data must renew data
        this._coin = 0;
        // Wallet Notice
        noticeItems[0].RenewNotice(CoinInWalletCond.id, this._coin);

        // Wallet Condition One (coin > 0)
        noticeItems[1].RenewNotice(CoinInWalletCond.id, this._coin);
        #endregion

        // After change condition must notify to check condition and refresh notice display
        NoticeManager.Notify
        (
            CoinInWalletCond.id,
            CoinIsEvenCond.id
        );

        // Also can call notify by noticeItem
        //foreach (var noticeItem in this.noticeItems)
        //{
        //    noticeItem.Notify();
        //}
    }

    public void IncreaseCoin()
    {
        // Increase Coin
        this._wallet.IncreaseCoin();

        #region Renew Value Type Data
        // If use value type data must renew data
        this._coin++;
        // Wallet Notice
        noticeItems[0].RenewNotice(CoinInWalletCond.id, this._coin);

        // Wallet Condition One (coin > 0)
        noticeItems[1].RenewNotice(CoinInWalletCond.id, this._coin);
        #endregion

        // After change condition must notify to check condition and refresh notice display
        NoticeManager.Notify
        (
            CoinInWalletCond.id,
            CoinIsEvenCond.id
        );

        // Also can call notify by noticeItem
        //foreach (var noticeItem in this.noticeItems)
        //{
        //    noticeItem.Notify();
        //}
    }

    public void DecreaseCoin()
    {
        // Decrease Coin
        this._wallet.DecreaseCoin();

        #region Renew Value Type Data
        // If use value type data must renew data
        this._coin--;
        // Wallet Notice
        noticeItems[0].RenewNotice(CoinInWalletCond.id, this._coin);

        // Wallet Condition One (coin > 0)
        noticeItems[1].RenewNotice(CoinInWalletCond.id, this._coin);
        #endregion

        // After change condition must notify to check condition and refresh notice display
        NoticeManager.Notify
        (
            CoinInWalletCond.id,
            CoinIsEvenCond.id
        );

        // Also can call notify by noticeItem
        //foreach (var noticeItem in this.noticeItems)
        //{
        //    noticeItem.Notify();
        //}
    }
}
