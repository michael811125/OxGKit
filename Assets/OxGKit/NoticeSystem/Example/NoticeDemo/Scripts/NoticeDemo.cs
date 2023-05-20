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

    public Text coin;
    public List<NoticeItem> noticeItems = new List<NoticeItem>();

    private Wallet _wallet;

    private void Awake()
    {
        this._wallet = new Wallet();
    }

    private void Start()
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
            new NoticeInfo(CoinInWalletCond.id, this._wallet),
            new NoticeInfo(CoinIsEvenCond.id, this._wallet)
        );

        // Wallet Condition One (coin > 0)
        noticeItems[1].RegisterNotice
        (
            new NoticeInfo(CoinInWalletCond.id, this._wallet)
        );

        // Wallet Condition Two (balance = even)
        noticeItems[2].RegisterNotice
        (
            new NoticeInfo(CoinIsEvenCond.id, this._wallet)
        );
        #endregion
    }

    private void _UpdateCoin()
    {
        // Refresh coin display
        if (this.coin != null) this.coin.text = $"${this._wallet.coin}";
    }

    public void ResetCoin()
    {
        // Reset Wallet 
        this._wallet.Reset();

        // After change condition must notify to check condition and refresh notice display
        NoticeManager.Notify
        (
            CoinInWalletCond.id,
            CoinIsEvenCond.id
        );
    }

    public void IncreaseCoin()
    {
        // Increase Coin
        this._wallet.IncreaseCoin();

        // After change condition must notify to check condition and refresh notice display
        NoticeManager.Notify
        (
            CoinInWalletCond.id,
            CoinIsEvenCond.id
        );
    }

    public void DecreaseCoin()
    {
        // Decrease Coin
        this._wallet.DecreaseCoin();

        // After change condition must notify to check condition and refresh notice display
        NoticeManager.Notify
        (
            CoinInWalletCond.id,
            CoinIsEvenCond.id
        );
    }
}
