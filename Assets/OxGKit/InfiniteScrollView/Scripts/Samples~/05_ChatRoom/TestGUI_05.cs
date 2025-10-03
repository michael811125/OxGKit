using OxGKit.InfiniteScrollView;
using UnityEngine;
using UnityEngine.UI;

public class TestGUI_05 : MonoBehaviour
{
    public InfiniteScrollView chatScrollView;
    public Text heightInstrument;
    public float baseCellHeight = 20;
    public InputField inputField;

    private string _speaker = "Tester";
    private string _message = "Hello World ! Hello My Dear !";

    private async void Awake()
    {
        this.chatScrollView = FindObjectOfType<InfiniteScrollView>();
        // Init cells first
        await this.chatScrollView.InitializePool();
    }

    private void OnGUI()
    {
        GUILayout.Label("Speaker");
        this._speaker = GUILayout.TextField(this._speaker);
        GUILayout.Label("Message");
        this._message = GUILayout.TextArea(_message, GUILayout.MaxWidth(300), GUILayout.MaxHeight(100));
        if (GUILayout.Button("Add"))
        {
            this._AddChatData(new ChatCellData(this._speaker, this._message, false));
        }
    }

    public void OnSubmit(string input)
    {
        this._AddChatDataAndSubmit(new ChatCellData(this._speaker, input, true));
        this.inputField.text = string.Empty;
        this.inputField.ActivateInputField();
        this.inputField.Select();
    }

    private void _AddChatDataAndSubmit(ChatCellData chatCellData)
    {
        this.heightInstrument.text = chatCellData.message;
        var infiniteData = new InfiniteCellData(new Vector2(0, this.heightInstrument.preferredHeight + this.baseCellHeight), chatCellData);
        this.chatScrollView.Add(infiniteData);
        this.chatScrollView.Refresh();
        this.chatScrollView.SnapLast(0.1f);
    }

    private void _AddChatData(ChatCellData chatCellData)
    {
        this.heightInstrument.text = chatCellData.message;
        var chatMessageHeight = this.heightInstrument.preferredHeight + this.baseCellHeight;
        var infiniteData = new InfiniteCellData(new Vector2(0, chatMessageHeight), chatCellData);
        this.chatScrollView.Add(infiniteData);
        // If filled to triiger refreshOnNextScroll will refresh on next value changed (at next scrolling)
        this.chatScrollView.Refresh(this.chatScrollView.isVisibleRangeFilled);
    }
}