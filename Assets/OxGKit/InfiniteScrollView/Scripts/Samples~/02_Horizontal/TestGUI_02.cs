using OxGKit.InfiniteScrollView;
using UnityEngine;

public class TestGUI_02 : MonoBehaviour
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
        if (GUILayout.Button("Add 100 Random Width Cell"))
        {
            for (int i = 0; i < 100; i++)
            {
                this._infiniteScrollView.Add(new InfiniteCellData(new Vector2(50, 0)));
            }
            this._infiniteScrollView.Refresh();
        }

        GUILayout.Label("Add New Cell Width");
        if (GUILayout.Button("Add"))
        {
            this._infiniteScrollView.Add(new InfiniteCellData(new Vector2(50, 0)));
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
