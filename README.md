<p align="center">
  <img width="384" height="384" src="Docs/OxGKit_Logo_v1.png">
</p>

[![License](https://img.shields.io/github/license/michael811125/OxGKit)](https://github.com/michael811125/OxGKit/blob/master/LICENSE.md)

---

## 基本介紹

OxGKit 是基於 Unity 設計於遊戲開發常用的系統工具組 (皆為獨立工具)，目前包含序列系統 (Action System)、通知系統 (Notice System)、輸入控制系統 (Input System)。

*[會持續擴充工具系統組]*

![](https://github.com/michael811125/OxGKit/blob/master/Docs/img_1.png)

---

## 工具系統介紹

### ActionSystem (dependence UniTask)

序列系統，能夠自行定義 Action 並且自行組合運行組，預設 Actions 有 SequenceAction, ParallelAction, ParallelDelayAction, DelayAction, DelegateAction，另外如果針對動畫需要進行拼湊處理，也可以使用 ActionSystem 作為運行。
- 透過 Right-Click Create/OxGKit/Action System/Template Action.cs 實作自定義 Action。

*[參考 Example]*

### Installation

| Install vi git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/ActionSystem/Scripts to Package Manager |

**第三方庫 (需自行安裝)**
- 使用 [UnitTask v2.3.3](https://github.com/Cysharp/UniTask)

**如果沒有要使用 ActionSystem，可以直接刪除整個 ActionSystem。**

※備註 : Right-Click Create/OxGKit/Action System... (Template cs)

---

### NoticeSystem (RedDotSystem)

通知系統 (也稱紅點系統)，支援動態新增刪除通知條件，可以自行定義通知條件，再針對 NoticeItem 進行條件持有註冊，當 NoticeItem 身上其中持有任一符合條件則通知顯示圖示 (紅點)。
- 透過 Right-Click Create/OxGKit/Notice System/Template Notice Condition.cs 實作通知條件。
- 將 NoticeItem prefab 拖曳至 UI 上，自行指定 ICON，再取得 NoticeItem 身上的組件進行條件註冊。
- 當有數據狀態變更時，必須通知特定條件 ID 進行 Notify，將會透過條件 ID 進行查找持有的 NoticeItems，並且進行刷新顯示。

*[參考 Example]*

### Installation

| Install vi git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/NoticeSystem/Scripts to Package Manager |

**如果沒有要使用 NoticeSystem，可以直接刪除整個 NoticeSystem。**

※備註 : Right-Click Create/OxGKit/Notice System... (Template cs)

---

### InputSystem (dependence Unity New Input System)

輸入控制系統，支援 Unity New Input System，驅動區分為 Control Maps (Input Action Asset), Binding Composites, Input Actions，自行建立 Unity New Inpupt System 的控制表，並且提供使用於 Unity New Input System 的 Binding Composite 腳本模板，最後再由 Input Action 派送輸入訊號控制由訂閱者訂閱，進而做到遊戲中的控制邏輯不需要知道平台裝置區分，皆由 Input Action 進行整合，當然 Input Action 也支援其他輸入控制插件，作為單純的輸入控制派送者。
- 透過 Right-Click Create/OxGKit/Input System/Template Input Action.cs 實作 InputAction 介面。
- 調用 Inputs API (using.OxGkit.InputSystem)

*[參考 Example]*

### Installation

| Install vi git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/InputSystem/Scripts to Package Manager |

**第三方庫 (需自行安裝)**
- 使用 [Unity New Input System v1.5.1](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/Installation.html)

**如果沒有要使用 InputSystem，可以直接刪除整個 InputSystem。**

※備註 : Right-Click Create/OxGKit/Input System... (Template cs)

---

### TweenSystem (dependence DoTween Pro)

補間動畫 (僅支持 [DoTween Pro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416))。
- Add Component/OxGKit/TweenSystem/DoTweenAnime
- Add Component/OxGKit/TweenSystem/DoTweenAnimeEvent

*[參考 Example]*

### Installation

| Install vi git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/TweenSystem/Scripts to Package Manager |

**第三方庫 (需自行購買安裝)**
- 使用 [DoTween Pro v1.0.335 or higher](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)

**如果沒有要使用 TweenSystem，可以直接刪除整個 TweenSystem。**

---

### Utilities

各通用組件 (Essential)。
- Utilities 
  - Timer: DeltaTimer, RealTimer, DTUpdate, RTUpdate.
  - Adapter: UISafeAreaAdapter.
  - Pool: NodePool (GameObject Pool).
  - ButtonPlus: Inherited by Unity Button. extend Long Click and Transition Scale.
  - UMT: Unity Main Thread.
  - Singleton: MonoSingleton (MonoBehaviour), NewSingleton (class).
  - Requester: RequestAudio, RequestTexture2D, RequestSprite, RequestBytes, RequestText.
  - Cacher: ARCCache<TKey, TValue>, LRUCache<TKey, TValue>, LRUKCache<TKey, TValue>.
  - TextureAnimation.
- Editor
  - RectTransform: RectTransformAdjuster (Hotkey: Shift+R, R: RectTransform).
  - MissingScriptsFinder.
  - SymlinkUtility.

*[參考 Example]*

### Installation

| Install vi git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/Utilities/Scripts to Package Manager |

**如果沒有要使用 Utilities，可以直接刪除整個 Utilities。**

---

### Unity 版本

建議使用 Unity 2021.3.26f1(LTS) or higher 版本 - [Unity Download](https://unity3d.com/get-unity/download/archive)

---

## License

This library is under the MIT License.