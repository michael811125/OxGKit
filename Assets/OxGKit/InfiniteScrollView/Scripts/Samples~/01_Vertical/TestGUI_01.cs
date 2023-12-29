using OxGKit.InfiniteScrollView;
using UnityEngine;

public class TestGUI_01 : MonoBehaviour
{
    private InfiniteScrollView _infiniteScrollView;
    private string _removeIndex = "0";
    private string _snapIndex = "0";

    private async void Awake()
    {
        this._infiniteScrollView = FindObjectOfType<InfiniteScrollView>();
        // Init cells first
        await this._infiniteScrollView.InitializePool();
    }

    private void OnGUI()
    {
        #region Add
        if (GUILayout.Button("Add 100 Random Height Cell"))
        {
            for (int i = 0; i < 100; i++)
            {
                var data = new InfiniteCellData(new Vector2(0, 50));
                this._infiniteScrollView.Add(data);
            }
            this._infiniteScrollView.Refresh();
        }

        if (GUILayout.Button("Add"))
        {
            var data = new InfiniteCellData(new Vector2(0, 50));
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
        #endregion
    }
}
