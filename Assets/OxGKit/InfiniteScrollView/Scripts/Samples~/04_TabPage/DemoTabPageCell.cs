using OxGKit.InfiniteScrollView;
using UnityEngine.UI;

public class DemoTabPageCell : InfiniteCell
{
    public Text text;

    public override void OnRefresh()
    {
        DemoTabPageData data = (DemoTabPageData)this.cellData.data;
        this.rectTransform.sizeDelta = this.cellData.cellSize;
        this.text.text = data.content;
    }
}
