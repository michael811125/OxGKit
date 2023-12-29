using OxGKit.InfiniteScrollView;
using UnityEngine;

public class TestGUI_03 : MonoBehaviour
{
    private InfiniteScrollView _infiniteScrollView;
    private string _removeIndex = "0";
    private string _snapIndex = "0";

    private async void Awake()
    {
        this._infiniteScrollView = FindObjectOfType<InfiniteScrollView>();
        // Init cells first
        await this._infiniteScrollView.InitializePool();
        this._infiniteScrollView.onCellSelected += OnCellSelected;
    }

    private void OnCellSelected(InfiniteCell selectedCell)
    {
        Debug.Log("On Cell Selected " + selectedCell.cellData.index);
    }

    private void OnGUI()
    {
        #region Add
        if (GUILayout.Button("Add 1000 Cell"))
        {
            for (int i = 0; i < 1000; i++)
            {
                this._infiniteScrollView.Add(new InfiniteCellData(new Vector2(100, 100)));
            }
            this._infiniteScrollView.Refresh();
        }

        if (GUILayout.Button("Add"))
        {
            var data = new InfiniteCellData(new Vector2(100, 100));
            this._infiniteScrollView.Add(data);
            this._infiniteScrollView.Refresh();
            this._infiniteScrollView.SnapLast(0.1f);
        }
        #endregion

        #region Remove
        GUILayout.Label("Remove Index");
        this._removeIndex = GUILayout.TextField(this._removeIndex);
        if (GUILayout.Button("Remove"))
        {
            this._infiniteScrollView.Remove(int.Parse(this._removeIndex));
        }
        #endregion

        #region Snap
        GUILayout.Label("Snap Index");
        this._snapIndex = GUILayout.TextField(this._snapIndex);
        if (GUILayout.Button("Snap"))
        {
            this._infiniteScrollView.Snap(int.Parse(this._snapIndex), 0.1f);
        }
        #endregion

        #region Scroll
        GUILayout.Label("Vertical");
        if (GUILayout.Button("Scroll to top"))
        {
            this._infiniteScrollView.ScrollToTop();
        }

        if (GUILayout.Button("Scroll to bottom"))
        {
            this._infiniteScrollView.ScrollToBottom();
        }

        GUILayout.Label("Horizontal");
        if (GUILayout.Button("Scroll to left"))
        {
            this._infiniteScrollView.ScrollToLeft();
        }

        if (GUILayout.Button("Scroll to right"))
        {
            this._infiniteScrollView.ScrollToRight();
        }
        #endregion
    }
}
