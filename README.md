<p align="center">
  <img width="384" height="384" src="Docs/OxGKit_Logo_v1.png">
</p>

[![License](https://img.shields.io/github/license/michael811125/OxGKit)](https://github.com/michael811125/OxGKit/blob/master/LICENSE.md)

---

## 基本介紹

OxGKit 是基於 Unity 設計於遊戲開發常用的系統工具組 (皆為獨立工具)。

**目前包含以下**

- 動作序列系統 (Action System)
- 通知系統 (Notice System)
- 輸入控制系統 (Input System)
- 日誌系統 (Logging System)
- DoTween Pro 的補間動畫系統 (TweenSystem)
- 各通用必備組件工具 (Utilities)

*[會持續擴充工具系統組]*

![](https://github.com/michael811125/OxGKit/blob/master/Docs/img_1.png)

---

## 工具系統介紹

### ActionSystem (dependence UniTask, OxGKit.LoggingSystem)

動作序列系統，能夠自行定義 Action 並且自行組合運行組，預設 Actions 有 SequenceAction, ParallelAction, ParallelDelayAction, DelayAction, DelegateAction，另外如果針對動畫需要進行拼湊處理，也可以使用 ActionSystem 作為運行。
- 透過 Right-Click Create/OxGKit/Action System/Template Action.cs 實作自定義 Action。

*[參考 Example]*

### Installation

| Install via git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/ActionSystem/Scripts to Package Manager |

**第三方庫 (需自行安裝)**
- 使用 [UnitTask v2.3.3](https://github.com/Cysharp/UniTask)
- 使用 OxGKit.LoggingSystem, Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/LoggingSystem/Scripts to Package Manager

**ActionSystem Demo**

https://github.com/michael811125/OxGKit/assets/30960759/169d91ea-3709-420c-8751-f50119e97d35

**如果沒有要使用 ActionSystem，可以直接刪除整個 ActionSystem。**

※備註 : Right-Click Create/OxGKit/Action System... (Template cs)

---

### NoticeSystem or RedDotSystem (dependence OxGKit.LoggingSystem)

通知系統 (也稱紅點系統)，支援動態新增刪除通知條件，可以自行定義通知條件，再針對 NoticeItem 進行條件持有註冊，當 NoticeItem 身上其中持有任一符合條件則通知顯示圖示 (紅點)。
- 透過 Right-Click Create/OxGKit/Notice System/Template Notice Condition.cs 實作通知條件。
- 將 NoticeItem prefab 拖曳至 UI 上，自行指定 ICON，再取得 NoticeItem 身上的組件進行條件註冊。
- 當有數據狀態變更時，必須通知特定條件 ID 進行 Notify，將會透過條件 ID 進行查找持有的 NoticeItems，並且進行刷新顯示。

*[參考 Example]*

### Installation

| Install via git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/NoticeSystem/Scripts to Package Manager |

**第三方庫 (需自行安裝)**
- 使用 OxGKit.LoggingSystem, Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/LoggingSystem/Scripts to Package Manager

**NoticeSystem Demo**

https://github.com/michael811125/OxGKit/assets/30960759/c6966327-3ede-432e-b8fe-bee53f629f25

**如果沒有要使用 NoticeSystem，可以直接刪除整個 NoticeSystem。**

※備註 : Right-Click Create/OxGKit/Notice System... (Template cs)

---

### InputSystem (dependence Unity New InputSystem, OxGKit.LoggingSystem)

輸入控制系統，支援 Unity New InputSystem，如果使用 Unity New InputSystem 需自行建立 Unity New InpuptSystem 的控制表 (Control Maps)，並且還有提供使用於 Unity New InputSystem 的 Binding Composite 腳本模板，最後再由 Input Action 派送輸入訊號控制由訂閱者訂閱，進而做到遊戲中的控制邏輯不需要知道平台裝置區分，皆由 Input Action 進行整合，當然 Input Action 也支援其他輸入控制插件，作為單純的輸入控制派送者。
- 透過 Right-Click Create/OxGKit/Input System/Template Input Action.cs 實作 InputAction 介面。
- 調用 Inputs API (using.OxGkit.InputSystem)

**主要層級驅動區分為以下**

- For Unity New InputSystem
  - Control Maps (Input Action Asset)
  - Binding Composites
- For Any Inputs
  - Input Actions (此為獨立作為通用訊號派送者，不依賴任何輸入控制插件，皆可自由實現)

*[參考 Example]*

### Installation

| Install via git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/InputSystem/Scripts to Package Manager |

**第三方庫 (需自行安裝)**
- 使用 [Unity New InputSystem v1.5.1](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/Installation.html)
- 使用 OxGKit.LoggingSystem, Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/LoggingSystem/Scripts to Package Manager

**InputSystem Demo**

https://github.com/michael811125/OxGKit/assets/30960759/20548ee4-b77b-4cda-8d49-c82b16deddb7

**如果沒有要使用 InputSystem，可以直接刪除整個 InputSystem。**

※備註 : Right-Click Create/OxGKit/Input System... (Template cs)

---

### LoggingSystem (dependence LWMyBox)

日誌系統，透過拖曳 LoggingLauncher 至場景上激活環境配置 (僅需激活一次)，並且需加載 LoggerSetting 進行日誌開關控制。 
- 透過 Right-Click Create/OxGKit/Logging System/Create Logger Setting 建立配置檔。
- 透過菜單選項 OxGKit/Logging System/Logger Setting 開啟將會自動生成配置檔。

**Build 激活標籤**
- OXGKIT_LOGGER_ON

![](https://github.com/michael811125/OxGKit/blob/master/Docs/img_4.png)

新增 Logger 或移除 Logger 皆需手動執行 Refresh and Load Loggers 重載。
```C#
using OxGKit.LoggingSystem;

[LoggerName("YourLoggerName")]
public class YourLogger : Logging {}

// After add must execute [Refresh and Load Loggers] in LoggerSetting or open LoggerSetting Editor Window
```

*[參考 Example]*

### Installation

| Install via git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/LoggingSystem/Scripts to Package Manager |

**第三方庫 (需自行安裝)**
- 使用 [LWMyBox v1.1.3 or higher](https://github.com/michael811125/LWMyBox), Add https://github.com/michael811125/LWMyBox.git to Package Manager

**LoggingSystem Demo**

https://github.com/michael811125/OxGKit/assets/30960759/1eb672cc-0d34-4e90-9b7c-d35a23c99af9

**LoggingSystem Build Test**

https://github.com/michael811125/OxGKit/assets/30960759/cef1a484-d617-466d-bf3e-6104032d7c3f

**如果沒有要使用 LoggingSystem，可以直接刪除整個 LoggingSystem，注意其他庫引用關係。**

---

### TweenSystem (dependence DoTween Pro, LWMyBox, OxGKit.Utilities)

補間動畫 (僅支持 [DoTween Pro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416))。
- Add Component/OxGKit/TweenSystem/DoTweenAnime
- Add Component/OxGKit/TweenSystem/DoTweenAnimeEvent

**Highly Recommended [brunomikoski - Animation Sequencer](https://github.com/brunomikoski/Animation-Sequencer)**

<!---
**注意：建議先安裝 OxGKit.TweenSystemFixer，避免編譯失敗導致 MenuItem 無法顯示與啟用**

- Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/TweenSystemFixer/Scripts to Package Manager
--->

Preview Mode (Only DoTweenAnime component is supported)

![](https://github.com/michael811125/OxGKit/blob/master/Docs/img_3.png)

※Note: The DoTweenAnimeEvent only plays at runtime.

*[參考 Example]*

### Installation

| Install via git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/TweenSystem/Scripts to Package Manager |

**第三方庫 (需自行購買安裝)**
- 使用 [DoTween Pro v1.0.335 or higher](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)

**第三方庫 (需自行安裝)**
- 使用 [LWMyBox v1.1.3 or higher](https://github.com/michael811125/LWMyBox), Add https://github.com/michael811125/LWMyBox.git to Package Manager
- 使用 OxGKit.Utilities, Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/Utilities/Scripts to Package Manager

<!--- ### Create DoTween Assemblies (Must use TweenSystemFixer to fix GUID) --->
### Create DoTween Assemblies

![](https://github.com/michael811125/OxGKit/blob/master/Docs/gif_1.gif)

**TweenSystem Demo**

https://github.com/michael811125/OxGKit/assets/30960759/6bf690b6-c4d0-40f6-b3c9-b50ab62562e8

**如果沒有要使用 TweenSystem，可以直接刪除整個 TweenSystem。**

<!---
---

### TweenSystemFixer (only for OxGkit.TweenSystem)

用於修復 OxGKit.TweenSystem 查找 DOTween.Modules assembly GUID 失敗問題，主要是因為 DoTween Pro 插件每次生成的 Assembly GUID 都不一樣，所以需要固定設置 OxGKit.TweenSystem 原本引用的 GUID，才能正確引用到 DOTween.Modules 的庫。

**僅用於修復 OxGKit.TweenSystem 已引用 DoTween.Modules 的固定 GUID**

### Installation

| Install via git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/TweenSystemFixer/Scripts to Package Manager |

### Reassign DOTween.Modules assembly GUID (Fix Error GUID)

#### 手動修復 Fixed GUID

如果未事先額外安裝 OxGKit.TweenSystemFixer 者，可以直接使用 Notepad++ 開啟 Plugins/Demigiant/DoTween/Modules/DOTween.Modules.asmdef.meta，直接修改 GUID 為以下 Fixed GUID 即可。
- 如果已經有其他 Assembly 引用原有 DoTween.Modules GUID，也要將原有 DoTween.Modules 的 GUID 取代成以下 Fixed GUID。

**Fixed GUID**

```
fdf3e181e62e9d243a7fee5ce890ab71
```

#### 工具修復 Fixed GUID

![](https://github.com/michael811125/OxGKit/blob/master/Docs/img_2.png)

![](https://github.com/michael811125/OxGKit/blob/master/Docs/gif_2.gif)
--->
---

### Utilities (dependence UniTask)

各通用組件 (Essential)。
- Utilities 
  - Timer: DeltaTimer, RealTimer, DTUpdate, RTUpdate.
  - Adapter: UISafeAreaAdapter.
  - Pool: NodePool (GameObject Pool).
  - ButtonPlus: Inherited by Unity Button. extend Long Click and Transition Scale.
  - Unity Main Thread: UMT.
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

| Install via git URL |
|:-|
| Add https://github.com/michael811125/OxGKit.git?path=Assets/OxGKit/Utilities/Scripts to Package Manager |

**第三方庫 (獨立安裝時，需自行安裝; 如果搭配 [OxGFrame](https://github.com/michael811125/OxGFrame) 則不需要額外安裝 UniTask)**
- 使用 [UnitTask v2.3.3](https://github.com/Cysharp/UniTask)
- 使用 [LWMyBox v1.1.3 or higher](https://github.com/michael811125/LWMyBox), Add https://github.com/michael811125/LWMyBox.git to Package Manager

**Utilities Demo (RectTransformAdjuster)**

https://github.com/michael811125/OxGKit/assets/30960759/cd43fc8b-c6f7-4878-990b-99dcacb8ed1f

**Utilities Demo (ButtonPlus)**

https://github.com/michael811125/OxGKit/assets/30960759/891291af-1bb4-4515-bec6-a9877f4ca254

**如果沒有要使用 Utilities，可以直接刪除整個 Utilities，注意其他庫引用關係。**

---

### Unity 版本

建議使用 Unity 2021.3.26f1(LTS) or higher 版本 - [Unity Download](https://unity3d.com/get-unity/download/archive)

---

## License

This library is under the MIT License.
