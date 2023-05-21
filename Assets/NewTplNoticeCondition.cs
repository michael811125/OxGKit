using OxGKit.NoticeSystem;
using UnityEngine;

public class NewTplNoticeCondition : NoticeCondition
{
    #region Defualt
    public static int id { get { return NoticeManager.GetConditionId<NewTplNoticeCondition>(); } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Register()
    {
        NoticeManager.RegisterCondition<NewTplNoticeCondition>();
    }
    #endregion

    public override bool ShowCondition(object refData)
    {
        /*
         * Implement Notice Condition
         */
		 
        return false;
    }
}