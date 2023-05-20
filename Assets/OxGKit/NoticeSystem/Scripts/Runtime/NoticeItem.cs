using UnityEngine;
using System.Collections.Generic;

namespace OxGKit.NoticeSystem
{
    public class NoticeItem : MonoBehaviour
    {
        private HashSet<NoticeInfo> _dictNoticeInfos;

        public NoticeItem()
        {
            this._dictNoticeInfos = new HashSet<NoticeInfo>();
        }

        /// <summary>
        /// Register Notice Infos (Auto Notify)
        /// </summary>
        /// <param name="noticeInfos"></param>
        public void RegisterNotice(params NoticeInfo[] noticeInfos)
        {
            foreach (var noticeInfo in noticeInfos)
            {
                if (!this._dictNoticeInfos.Contains(noticeInfo))
                {
                    // 註冊持有條件訊息
                    this._dictNoticeInfos.Add(noticeInfo);
                    // 並且需要向 NoticeManager 註冊通知物件
                    NoticeManager.RegisterNoticeItem(noticeInfo.conditionId, this);
                }
            }

            // 自動檢查條件是否顯示
            this.CheckConditionAndVisible();
        }

        /// <summary>
        /// Deregister Notice Infos
        /// </summary>
        public void DeregisterNotice()
        {
            // 針對自身條件筆數進行註銷
            foreach (var noticeInfo in this._dictNoticeInfos)
            {
                // 註銷條件
                NoticeManager.DeregisterNoticeItem(noticeInfo.conditionId, this);
            }

            // 全註銷後清除緩存
            this._dictNoticeInfos.Clear();

            // 自動檢查條件是否顯示
            this.CheckConditionAndVisible();
        }

        /// <summary>
        /// Check notice condition and visible
        /// </summary>
        internal void CheckConditionAndVisible()
        {
            foreach (var noticeInfo in this._dictNoticeInfos)
            {
                // 將 NoticeItem 的獨立數據交給 NoticeManager 進行條件判斷 (條件方法池)
                bool active = NoticeManager.CheckCondition(noticeInfo.conditionId, noticeInfo.refData);

                // 任一條件為激活狀態, 則開啟顯示
                if (active)
                {
                    if (!this.gameObject.activeSelf)
                    {
                        this.gameObject.SetActive(true);
                        Debug.Log($"<color=#6dedff>[{nameof(NoticeSystem)}] <color=#94ff2c>(Match Condition)</color> Notify Notice Item <color=#94ff2c>{this.name}</color></color>");
                    }
                    return;
                }
            }

            // 無任何激活條件, 則關閉顯示
            this.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            this.DeregisterNotice();

            this._dictNoticeInfos.Clear();
            this._dictNoticeInfos = null;
        }
    }
}