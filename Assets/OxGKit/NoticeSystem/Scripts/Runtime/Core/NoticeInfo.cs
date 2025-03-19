namespace OxGKit.NoticeSystem
{
    public struct NoticeInfo
    {
        public int conditionId;
        public object data;

        public NoticeInfo(int conditionId, object data)
        {
            this.conditionId = conditionId;
            this.data = data;
        }
    }
}