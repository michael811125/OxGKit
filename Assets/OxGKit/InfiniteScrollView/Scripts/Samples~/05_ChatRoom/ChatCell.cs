using OxGKit.InfiniteScrollView;
using UnityEngine;
using UnityEngine.UI;

public class ChatCell : InfiniteCell
{
    // Cell template components
    public Text speakerText;
    public Text messageText;

    public override void OnRefresh()
    {
        // Get cell data
        ChatCellData data = (ChatCellData)this.cellData.data;

        // Set cell data to cell template
        this.speakerText.text = data.speaker;
        this.messageText.text = data.message;
        this.speakerText.alignment = data.isSelf ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
        this.messageText.alignment = data.isSelf ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
        this.rectTransform.sizeDelta = this.cellData.cellSize;
    }
}
