using OxGKit.InfiniteScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemoTabPageScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public InfiniteScrollView scrollView;
    public string[] pageContents;
    public Toggle[] toggles;
    public ToggleGroup toggleGroup;
    public Vector2 eachContentSize = new Vector2(600, 400);

    public float _snapThreshold = 100;
    private bool _isEndDragging;

    private async void Awake()
    {
        // Init cells first
        await this.scrollView.InitializePool();
    }

    private void Start()
    {
        foreach (var data in this.pageContents)
        {
            this.scrollView.Add(new InfiniteCellData(eachContentSize, new DemoTabPageData { content = data }));
        }
        this.scrollView.Refresh();
    }

    public void OnToggleChange(int index)
    {
        if (this.toggles[index].isOn)
        {
            this.scrollView.Snap(index, 0.1f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this._isEndDragging = false;
        this.toggleGroup.SetAllTogglesOff();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this._isEndDragging = true;
    }

    private void Update()
    {
        if (this._isEndDragging)
        {
            if (Mathf.Abs(this.scrollView.scrollRect.velocity.x) <= this._snapThreshold)
            {
                this._isEndDragging = false;
                var clampX = Mathf.Min(0, this.scrollView.scrollRect.content.anchoredPosition.x);
                int closingIndex = Mathf.Abs(Mathf.RoundToInt(clampX / eachContentSize.x));
                this.toggles[closingIndex].isOn = true;
            }
        }
    }
}
