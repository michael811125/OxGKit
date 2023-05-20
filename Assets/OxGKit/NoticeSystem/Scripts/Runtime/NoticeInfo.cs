namespace OxGKit.NoticeSystem
{
    public struct NoticeInfo
    {
        public int conditionId;
        public object refData;

        public NoticeInfo(int conditionId, object refData)
        {
            this.conditionId = conditionId;
            this.refData = refData;
        }
    }
}