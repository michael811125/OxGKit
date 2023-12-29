public class ChatCellData
{
    public string speaker;
    public string message;
    public bool isSelf;

    public ChatCellData(string speaker, string message, bool isSelf)
    {
        this.speaker = speaker;
        this.message = message;
        this.isSelf = isSelf;
    }
}
