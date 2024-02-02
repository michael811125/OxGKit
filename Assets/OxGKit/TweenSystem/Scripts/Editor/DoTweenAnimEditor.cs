using DG.DOTweenEditor;
using DG.Tweening;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static OxGKit.TweenSystem.DoTweenAnim;

namespace OxGKit.TweenSystem.Editor
{
    [CustomEditor(typeof(DoTweenAnim), true)]
    internal class DoTweenAnimEditor : UnityEditor.Editor
    {
        protected DoTweenAnim _target;

        // Properties
        protected SerializedProperty _isSyncBeginValues;

        // Editor Params
        private bool _isPlaying = false;
        private bool _playTrigger = false;
        private float _progress = 0f;

        // Records Origin Values
        private TweenPosition _tPosition = new TweenPosition();
        private TweenRotation _tRotation = new TweenRotation();
        private TweenScale _tScale = new TweenScale();
        private TweenSize _tSize = new TweenSize();
        private TweenAlpha _tAlpha = new TweenAlpha();
        private TweenImgColor _tImgColor = new TweenImgColor();
        private TweenSprColor _tSprColor = new TweenSprColor();

        private void OnEnable()
        {
            this._target = (DoTweenAnim)target;

            // Init Properties
            this._isSyncBeginValues = serializedObject.FindProperty("_isSyncBeginValue");

            EditorApplication.playModeStateChanged += this._OnEditorPlayModeChanged;
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) this._StopPreviewTweens();
            EditorApplication.playModeStateChanged -= this._OnEditorPlayModeChanged;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(isPreviewMode || Application.isPlaying);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();

            // Preview Editor
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUIStyle style = new GUIStyle();
            var bg = new Texture2D(1, 1);
            ColorUtility.TryParseHtmlString("#124034", out Color color);
            Color[] pixels = Enumerable.Repeat(color, Screen.width * Screen.height).ToArray();
            bg.SetPixels(pixels);
            bg.Apply();
            style.normal.background = bg;
            EditorGUILayout.BeginVertical(style);

            EditorGUILayout.LabelField("Editor");
            EditorGUILayout.Space();

            // Draw Views
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            this._DrawIsSyncBeginValuesView();
            EditorGUILayout.Space();
            this._DrawPlayControlsView();
            EditorGUILayout.Space();
            this._DrawProgressView();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();
        }

        private void _DrawIsSyncBeginValuesView()
        {
            EditorGUI.BeginDisabledGroup(isPreviewMode);
            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(this._isSyncBeginValues);
                if (changedCheck.changed) serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void _DrawPlayControlsView()
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle previewButtonStyle = new GUIStyle(GUI.skin.button);
            previewButtonStyle.fixedWidth = previewButtonStyle.fixedHeight = 40;

            // Active Mode
            if (this._target.driveMode == DriveMode.Active)
            {
                var content = EditorGUIUtility.IconContent("PlayButton On");
                content.tooltip = "Play";

                EditorGUI.BeginDisabledGroup(this._isPlaying || this._progress > 0f);
                if (GUILayout.Button(content, previewButtonStyle))
                {
                    if (!isPreviewMode)
                    {
                        isPreviewMode = true;
                        // Records origin tween values
                        this._InitOriginTweens();
                        // Init tweens
                        this._target.InitTweens();
                    }

                    this._isPlaying = true;
                    // Play tweens for prepare values
                    this._target.PlayTween(true, () =>
                    {
                        DOTweenEditorPreview.Stop();
                        this._isPlaying = false;
                    });

                    this._PreparePreviewTweens();
                    DOTweenEditorPreview.Start();
                }
                EditorGUI.EndDisabledGroup();
            }
            // Event Mode
            else if (this._target.driveMode == DriveMode.Event)
            {
                GUIContent content;
                if (!this._playTrigger)
                {
                    content = EditorGUIUtility.IconContent("Animation.NextKey");
                    content.tooltip = "Play Trigger (true)";
                }
                else
                {
                    content = EditorGUIUtility.IconContent("Animation.PrevKey");
                    content.tooltip = "Play Trigger (false)";
                }

                EditorGUI.BeginDisabledGroup(this._isPlaying);
                if (GUILayout.Button(content, previewButtonStyle))
                {
                    if (!isPreviewMode)
                    {
                        isPreviewMode = true;
                        // Records origin tween values
                        this._InitOriginTweens();
                        // Init tweens
                        this._target.InitTweens();
                    }

                    this._isPlaying = true;
                    this._playTrigger = !this._playTrigger;
                    // Play tweens for prepare values
                    this._target.PlayTween(this._playTrigger, () =>
                    {
                        DOTweenEditorPreview.Stop();
                        this._isPlaying = false;
                    });

                    this._PreparePreviewTweens();
                    DOTweenEditorPreview.Start();
                }
                EditorGUI.EndDisabledGroup();
            }

            // Stop Section
            {
                EditorGUI.BeginDisabledGroup(!isPreviewMode);
                var content = EditorGUIUtility.IconContent("animationdopesheetkeyframe");
                content.tooltip = "Stop";

                if (GUILayout.Button(content, previewButtonStyle))
                {
                    this._StopPreviewTweens();
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void _DrawProgressView()
        {
            if (this._target.driveMode == DriveMode.Active)
            {
                EditorGUILayout.LabelField("Progress");

                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginDisabledGroup(this._isPlaying);
                float previewProgress = EditorGUILayout.Slider(this._progress, 0, 1);

                if (EditorGUI.EndChangeCheck())
                {
                    this._progress = previewProgress;

                    if (!isPreviewMode && this._progress > 0f)
                    {
                        isPreviewMode = true;
                        // Records origin tween values
                        this._InitOriginTweens();
                        // Init tweens
                        this._target.InitTweens();
                        // Play tweens for prepare values
                        this._target.PlayTween(true);
                    }
                    else if (this._progress <= 0f && isPreviewMode)
                    {
                        this._StopPreviewTweens();
                    }

                    if (isPreviewMode && this._progress > 0f) this._PlayProgressPreviewTweens(this._progress);
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        private void _PreparePreviewTweens()
        {
            // Tween Position
            if (this._target.tPositionOn)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(this._target.tPosition.seq);
            }

            // Tween Rotation
            if (this._target.tRotationOn)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(this._target.tRotation.seq);
            }

            // Tween Scale
            if (this._target.tScaleOn)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(this._target.tScale.seq);
            }

            // Tween Size (RectTransform)
            if (this._target.tSizeOn)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(this._target.tSize.seq);
            }

            // Tween Alpha (CanvasGroup)
            if (this._target.tAlphaOn)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(this._target.tAlpha.seq);
            }

            // Tween Image Color (Image)
            if (this._target.tImgColorOn)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(this._target.tImgColor.seq);
            }

            // Tween Sprite Color (Sprite)
            if (this._target.tSprColorOn)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(this._target.tSprColor.seq);
            }
        }

        private void _PlayProgressPreviewTweens(float progress)
        {
            // Tween Position
            if (this._target.tPositionOn)
            {
                this._target.tPosition.seq.Goto(progress * this._target.tPosition.seq.Duration());
            }

            // Tween Rotation
            if (this._target.tRotationOn)
            {
                this._target.tRotation.seq.Goto(progress * this._target.tRotation.seq.Duration());
            }

            // Tween Scale
            if (this._target.tScaleOn)
            {
                this._target.tScale.seq.Goto(progress * this._target.tScale.seq.Duration());
            }

            // Tween Size (RectTransform)
            if (this._target.tSizeOn)
            {
                this._target.tSize.seq.Goto(progress * this._target.tSize.seq.Duration());
            }

            // Tween Alpha (CanvasGroup)
            if (this._target.tAlphaOn)
            {
                this._target.tAlpha.seq.Goto(progress * this._target.tAlpha.seq.Duration());
            }

            // Tween Image Color (Image)
            if (this._target.tImgColorOn)
            {
                this._target.tImgColor.seq.Goto(progress * this._target.tImgColor.seq.Duration());
            }

            // Tween Sprite Color (Sprite)
            if (this._target.tSprColorOn)
            {
                this._target.tSprColor.seq.Goto(progress * this._target.tSprColor.seq.Duration());
            }
        }

        private void _StopPreviewTweens()
        {
            if (DOTweenEditorPreview.isPreviewing) DOTweenEditorPreview.Stop(true);
            this._ResetOriginTweens();
            this._playTrigger = false;
            this._isPlaying = false;
            this._progress = 0f;
            isPreviewMode = false;
        }

        private void _InitOriginTweens()
        {
            // Init Tween Position
            if (this._target.tPositionOn)
            {
                this._tPosition.transform = this._target.transform;
                this._tPosition.Init(this._target.transform.localPosition);
            }

            // Init Tween Rotation
            if (this._target.tRotationOn)
            {
                this._tRotation.transform = this._target.transform;
                this._tRotation.Init(this._target.transform.localRotation.eulerAngles);
            }

            // Init Tween Scale
            if (this._target.tScaleOn)
            {

                this._tScale.transform = this._target.transform;
                this._tScale.Init(this._target.transform.localScale);
            }

            // Init Tween Size (RectTransform)
            if (this._target.tSizeOn)
            {
                var rectTransform = this._target.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    this._tSize.rectTransform = rectTransform;
                    this._tSize.Init(rectTransform.sizeDelta);
                }
            }

            // Init Tween Alpha (CanvasGroup)
            if (this._target.tAlphaOn)
            {
                var cg = this._target.transform.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    this._tAlpha.cg = cg;
                    this._tAlpha.Init(cg.alpha);
                }
            }

            // Init Tween Image Color (Image)
            if (this._target.tImgColorOn)
            {
                var img = this._target.transform.GetComponent<Image>();
                if (img != null)
                {
                    this._tImgColor.img = img;
                    this._tImgColor.Init(img.color);
                }
            }

            // Init Tween Sprite Color (Sprite)
            if (this._target.tSprColorOn)
            {
                var spr = this._target.transform.GetComponent<SpriteRenderer>();
                if (spr != null)
                {
                    this._tSprColor.spr = spr;
                    this._tSprColor.Init(spr.color);
                }
            }
        }

        private void _ResetOriginTweens()
        {
            if (isPreviewMode)
            {
                if (this._target.tPositionOn) this._tPosition.Reset();
                if (this._target.tRotationOn) this._tRotation.Reset();
                if (this._target.tScaleOn) this._tScale.Reset();
                if (this._target.tSizeOn) this._tSize.Reset();
                if (this._target.tAlphaOn) this._tAlpha.Reset();
                if (this._target.tImgColorOn) this._tImgColor.Reset();
                if (this._target.tSprColorOn) this._tSprColor.Reset();
            }
        }

        private void _OnEditorPlayModeChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingEditMode)
            {
                this._StopPreviewTweens();
            }
        }
    }
}