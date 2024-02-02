using OxGKit.TweenSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenDemo : MonoBehaviour
{
    public List<Toggle> tgls = new List<Toggle>();

    private void Awake()
    {
        this._InitEvents();
    }

    void Start()
    {
        this._BasicDisplay();
    }

    private void _InitEvents()
    {
        for (int i = 0; i < this.tgls.Count; i++)
        {
            int idx = i;
            this.tgls[i].onValueChanged.RemoveAllListeners();
            this.tgls[i].onValueChanged.AddListener((isOn) => this._RefreshToggleTweenAnime(isOn, idx));
            this.tgls[i].onValueChanged.AddListener((isOn) => this._OnToggleEvent(isOn, idx));
        }
    }

    private void _BasicDisplay()
    {
        this._DrawTogglesView();
    }

    private void _DrawTogglesView()
    {
        // Init event and anime which toggle isOn first
        for (int i = 0; i < this.tgls.Count; i++)
        {
            int idx = i;
            if (this.tgls[i].isOn)
            {
                this._RefreshToggleTweenAnime(this.tgls[i].isOn, idx);
                this._OnToggleEvent(!this.tgls[i].isOn, idx);
                return;
            }
        }
    }

    private void _RefreshToggleTweenAnime(bool isOn, int idx)
    {
        this.tgls[idx].GetComponent<DoTweenAnimEvent>().PlayTriggerOnce
         (
             isOn,
             () => Debug.Log("<color=#a1ff30>Toggle Tween Trigger Once</color>")
         );
    }

    private void _OnToggleEvent(bool isOn, int idx)
    {
        if (isOn) return;

        Debug.Log($"<color=#fff530>IsOn: {this.tgls[idx].name}</color>");
    }
}
