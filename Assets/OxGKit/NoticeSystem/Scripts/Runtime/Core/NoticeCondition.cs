namespace OxGKit.NoticeSystem
{
    public abstract class NoticeCondition
    {
        private int _id;

        public void SetId(int id)
        {
            this._id = id;
        }

        public int GetId()
        {
            return this._id;
        }

        public abstract bool ShowCondition(object data);
    }
}