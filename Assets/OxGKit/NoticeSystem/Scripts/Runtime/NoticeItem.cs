using UnityEngine;

namespace OxGKit.NoticeSystem
{
    public class NoticeItem : MonoBehaviour
    {
        private NoticeInfo[] _noticeInfos;

        /// <summary>
        /// When data changes must call notify to changes visible
        /// </summary>
        public void Notify()
        {
            if (this._noticeInfos == null) return;

            // Collect notify condition id and remove duplicates
            for (int i = 0; i < this._noticeInfos.Length; i++)
            {
                NoticeManager.NotifyCollector(this._noticeInfos[i].conditionId);
            }

            // Notify all after collect 
            NoticeManager.NotifyAll();
        }

        /// <summary>
        /// Renew data for value type
        /// </summary>
        /// <param name="conditionId"></param>
        /// <param name="data"></param>
        /// <param name="checkConditionAndVisible"></param>
        public void RenewNotice(int conditionId, object data, bool checkConditionAndVisible = false)
        {
            bool isRenewed = false;
            for (int i = 0; i < this._noticeInfos.Length; i++)
            {
                // Assign new noticeInfo by condition id
                if (this._noticeInfos[i].conditionId == conditionId)
                {
                    this._noticeInfos[i].data = data;
                    isRenewed = true;
                    if (checkConditionAndVisible) this.CheckConditionAndVisible();
                    break;
                }
            }

            // If is not existing will auto register
            if (!isRenewed) this.RegisterNotice(new NoticeInfo(conditionId, data));
        }

        /// <summary>
        /// Register notice infos and auto check condition with visible (If register again will assign and renew)
        /// </summary>
        /// <param name="noticeInfos"></param>
        public void RegisterNotice(params NoticeInfo[] noticeInfos)
        {
            if (noticeInfos == null) return;

            // Assign noticeInfos directly (renew)
            this._noticeInfos = noticeInfos;

            for (int i = 0; i < this._noticeInfos.Length; i++)
            {
                // Register noticeItem by condition id
                NoticeManager.RegisterNoticeItem(this._noticeInfos[i].conditionId, this);
            }

            // Auto check condition with visible
            this.CheckConditionAndVisible();
        }

        /// <summary>
        /// Deregister notice infos
        /// </summary>
        public void DeregisterNotice()
        {
            // 針對自身條件筆數進行註銷
            for (int i = 0; i < this._noticeInfos.Length; i++)
            {
                // 註銷條件
                NoticeManager.DeregisterNoticeItem(this._noticeInfos[i].conditionId, this);
            }

            // 全註銷後清除緩存
            this._noticeInfos = null;

            // 自動檢查條件是否顯示
            this.CheckConditionAndVisible();
        }

        /// <summary>
        /// Check notice condition and visible
        /// </summary>
        internal void CheckConditionAndVisible()
        {
            if (this._noticeInfos != null)
            {
                for (int i = 0; i < this._noticeInfos.Length; i++)
                {
                    // 將 NoticeItem 的獨立數據交給 NoticeManager 進行條件判斷 (條件方法池)
                    bool active = NoticeManager.CheckCondition(this._noticeInfos[i].conditionId, this._noticeInfos[i].data);

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
            }

            // 無任何激活條件, 則關閉顯示
            this.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            this.DeregisterNotice();
            this._noticeInfos = null;
        }
    }
}