using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

// Use InputBindingComposite<TValue> as a base class for a composite that returns
// values of type TValue.
// NOTE: It is possible to define a composite that returns different kinds of values
//       but doing so requires deriving directly from InputBindingComposite.
#if UNITY_EDITOR
[InitializeOnLoad] // Automatically register in editor.
#endif
public class MoveInputComposite : InputBindingComposite<MoveInputComposite.MoveInput>
{
    /// <summary>
    /// Determines how a <c>Vector2</c> is synthesized from part controls.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// Part controls are treated as analog meaning that the floating-point values read from controls
        /// will come through as is (minus the fact that the down and left direction values are negated).
        /// </summary>
        Analog = 2,

        /// <summary>
        /// Part controls are treated as buttons (on/off) and the resulting vector is normalized. This means
        /// that if, for example, both left and up are pressed, instead of returning a vector (-1,1), a vector
        /// of roughly (-0.7,0.7) (that is, corresponding to <c>new Vector2(-1,1).normalized</c>) is returned instead.
        /// The resulting 2D area is diamond-shaped.
        /// </summary>
        DigitalNormalized = 0,

        /// <summary>
        /// Part controls are treated as buttons (on/off) and the resulting vector is not normalized. This means
        /// that if, for example, both left and up are pressed, the resulting vector is (-1,1) and has a length
        /// greater than 1. The resulting 2D area is box-shaped.
        /// </summary>
        Digital = 1
    }

    public struct MoveInput
    {
        public bool modifier;
        public Vector2 direction;
    }

    [InputControl(layout = "Button")]
    public int modifier;

    [InputControl(layout = "Axis")]
    public int up;

    [InputControl(layout = "Axis")]
    public int down;

    [InputControl(layout = "Axis")]
    public int left;

    [InputControl(layout = "Axis")]
    public int right;

    [Obsolete("Use Mode.DigitalNormalized with 'mode' instead")]
    public bool normalize = true;

    public Mode mode;

    // This method computes the resulting input value of the composite based
    // on the input from its part bindings.
    public override MoveInput ReadValue(ref InputBindingCompositeContext context)
    {
        var modifier = context.ReadValueAsButton(this.modifier);
        Vector2 direction;
        var mode = this.mode;

        if (mode == Mode.Analog)
        {
            var upValue = context.ReadValue<float>(up);
            var downValue = context.ReadValue<float>(down);
            var leftValue = context.ReadValue<float>(left);
            var rightValue = context.ReadValue<float>(right);

            direction = DpadControl.MakeDpadVector(upValue, downValue, leftValue, rightValue);

            return new MoveInput
            {
                modifier = modifier,
                direction = direction
            };
        }

        var upIsPressed = context.ReadValueAsButton(up);
        var downIsPressed = context.ReadValueAsButton(down);
        var leftIsPressed = context.ReadValueAsButton(left);
        var rightIsPressed = context.ReadValueAsButton(right);

        // Legacy. We need to reference the obsolete member here so temporarily
        // turn of the warning.
#pragma warning disable CS0618
        if (!normalize) // Was on by default.
            mode = Mode.Digital;
#pragma warning restore CS0618

        direction = DpadControl.MakeDpadVector(upIsPressed, downIsPressed, leftIsPressed, rightIsPressed, mode == Mode.DigitalNormalized);

        return new MoveInput
        {
            modifier = modifier,
            direction = direction
        };
    }

    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
    {
        var value = ReadValue(ref context);
        return value.direction.magnitude;
    }

#if UNITY_EDITOR
    internal class MoveInputCompositeEditor : InputParameterEditor<MoveInputComposite>
    {
        private GUIContent m_ModeLabel = new GUIContent("Mode",
            "How to synthesize a Vector2 from the inputs. Digital "
            + "treats part bindings as buttons (on/off) whereas Analog preserves "
            + "floating-point magnitudes as read from controls.");

        public override void OnGUI()
        {
            target.mode = (Mode)EditorGUILayout.EnumPopup(m_ModeLabel, target.mode);
        }

#if UNITY_INPUT_SYSTEM_PROJECT_WIDE_ACTIONS
        public override void OnDrawVisualElements(VisualElement root, Action onChangedCallback)
        {
            var modeField = new EnumField("Mode", target.mode)
            {
                tooltip = m_ModeLabel.text
            };

            modeField.RegisterValueChangedCallback(evt =>
            {
                target.mode = (Mode)evt.newValue;
                onChangedCallback();
            });

            root.Add(modeField);
        }
#endif
    }
#endif

    static MoveInputComposite()
    {
        // Can give custom name or use default (type name with "Composite" clipped off).
        // Same composite can be registered multiple times with different names to introduce
        // aliases.
        //
        // NOTE: Registering from the static constructor using InitializeOnLoad and
        //       RuntimeInitializeOnLoadMethod is only one way. You can register the
        //       composite from wherever it works best for you. Note, however, that
        //       the registration has to take place before the composite is first used
        //       in a binding. Also, for the composite to show in the editor, it has
        //       to be registered from code that runs in edit mode.
        InputSystem.RegisterBindingComposite<MoveInputComposite>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() { } // Trigger static constructor.
}
