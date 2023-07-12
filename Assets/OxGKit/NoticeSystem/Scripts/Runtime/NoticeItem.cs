using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OxGKit.NoticeSystem
{
    [DisallowMultipleComponent]
    [AddComponentMenu("OxGKit/NoticeSystem/" + nameof(NoticeItem))]
    public class NoticeItem : MonoBehaviour
    {
        private Dictionary<int, NoticeInfo> _dictNoticeInfos = new Dictionary<int, NoticeInfo>();

        /// <summary>
        /// Get notice infos
        /// </summary>
        /// <returns></returns>
        internal NoticeInfo[] GetNoticeInfos()
        {
            return this._dictNoticeInfos.Values.ToArray();
        }

        /// <summary>
        /// Check has condition id
        /// </summary>
        /// <param name="conditionId"></param>
        /// <returns></returns>
        public bool HasCondition(int conditionId)
        {
            return this._dictNoticeInfos.ContainsKey(conditionId);
        }

        /// <summary>
        /// When data changes must call notify to changes visible
        /// </summary>
        public void Notify()
        {
            foreach (var noticeInfo in this._dictNoticeInfos.Values)
            {
                NoticeManager.Notify(noticeInfo.conditionId);
            }
        }

        /// <summary>
        /// Renew data for value type (Method Chaining)
        /// </summary>
        /// <param name="noticeInfos"></param>
        /// <returns></returns>
        public NoticeItem RenewNotice(NoticeInfo noticeInfo)
        {
            bool isRenewed = false;

            int id = noticeInfo.conditionId;
            if (this.HasCondition(id))
            {
                this._dictNoticeInfos[id] = noticeInfo;
                isRenewed = true;
            }

            // If is not existing will auto register
            if (!isRenewed) this.RegisterNotice(noticeInfo);

            return this;
        }

        /// <summary>
        /// Register notice infos and auto check condition with visible (If register again will assign and renew)
        /// </summary>
        /// <param name="noticeInfos"></param>
        public void RegisterNotice(params NoticeInfo[] noticeInfos)
        {
            if (noticeInfos == null) return;

            for (int i = 0; i < noticeInfos.Length; i++)
            {
                int id = noticeInfos[i].conditionId;
                if (this.HasCondition(id))
                {
                    this._dictNoticeInfos[id] = noticeInfos[i];
                }
                else this._dictNoticeInfos.Add(id, noticeInfos[i]);
            }

            foreach (var noticeInfo in this._dictNoticeInfos.Values)
            {
                // Register noticeItem by condition id
                NoticeManager.RegisterNoticeItem(noticeInfo.conditionId, this);
            }

            // Auto check condition with visible
            this.CheckConditionAndVisible();
        }

        /// <summary>
        /// Deregister notice infos
        /// </summary>
        /// <param name="conditionIds"></param>
        public void DeregisterNotice(params int[] conditionIds)
        {
            if (conditionIds == null || conditionIds.Length == 0)
            {
                // Deregister by self conditions
                foreach (var noticeInfo in this._dictNoticeInfos.Values)
                {
                    // Deregister condition and notice item
                    NoticeManager.DeregisterNoticeItem(noticeInfo.conditionId, this);
                }

                // After deregister to clear cache
                this._dictNoticeInfos.Clear();
            }
            else
            {
                for (int i = 0; i < conditionIds.Length; i++)
                {
                    if (this.HasCondition(conditionIds[i]))
                    {
                        NoticeManager.DeregisterNoticeItem(conditionIds[i], this);
                        this._dictNoticeInfos.Remove(conditionIds[i]);
                    }
                }
            }

            this.CheckConditionAndVisible();
        }

        /// <summary>
        /// Check notice condition and visible
        /// </summary>
        internal void CheckConditionAndVisible()
        {
            foreach (var noticeInfo in this._dictNoticeInfos.Values)
            {
                // 將 NoticeItem 的獨立數據交給 NoticeManager 進行條件判斷 (條件方法池)
                bool active = NoticeManager.CheckCondition(noticeInfo.conditionId, noticeInfo.data);

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
            this._dictNoticeInfos = null;
        }
    }
}