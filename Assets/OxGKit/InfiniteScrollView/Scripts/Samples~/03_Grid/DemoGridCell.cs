using OxGKit.InfiniteScrollView;
using UnityEngine.UI;

public class DemoGridCell : InfiniteCell
{
    public Text text;

    public override void OnRefresh()
    {
        this.rectTransform.sizeDelta = this.cellData.cellSize;
        this.text.text = this.cellData.index.ToString();
    }
}
