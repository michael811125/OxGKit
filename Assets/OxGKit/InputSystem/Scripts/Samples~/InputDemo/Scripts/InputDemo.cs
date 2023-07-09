using OxGKit.InputSystem;
using OxGKit.InputSystem.Example;
using UnityEngine;

public class InputDemo : MonoBehaviour
{
    public GameObject player;
    public float speed = 2.5f;
    public float speedOffset = 2f;

    private Vector2 _direction;
    private bool _speedUp;

    private void Awake()
    {
        // If use Unity New Input System must register control maps
        Inputs.CM.RegisterControlMap<PlayerControls>();

        // Register an input action (after register control maps)
        Inputs.IA.RegisterInputAction<PlayerAction>();
    }

    private void OnEnable()
    {
        // Add handle event
        Inputs.IA.GetInputAction<PlayerAction>().onMoveAction += this._OnMoveAction;
    }

    private void OnDisable()
    {
        // Del handle event
        Inputs.IA.GetInputAction<PlayerAction>().onMoveAction -= this._OnMoveAction;
    }

    private void Update()
    {
        // Call by Main MonoBehaviour (Main Program)
        Inputs.IA.OnUpdateInputActions(Time.unscaledDeltaTime);
    }

    private void FixedUpdate()
    {
        // Update player movement per frame
        this._UpdateMovement(Time.fixedDeltaTime);
    }

    private void _OnMoveAction(MoveInputComposite.MoveInput moveInput)
    {
        // Records move input signals and input data
        this._speedUp = moveInput.modifier;
        this._direction = moveInput.direction;

        Debug.Log($"<color=#ffcf59><color=#c2ff19>[InputSystem]</color> {nameof(PlayerAction)} Move <color=#ff59bc>SpeedUp: {moveInput.modifier}</color>, <color=#59d7ff>Direction: {moveInput.direction}</color></color>");
    }

    private void _UpdateMovement(float dt)
    {
        if (this._direction != Vector2.zero)
        {
            var speed = (this._speedUp) ? this.speed + this.speedOffset : this.speed;

            this.player.transform.position = new Vector3
            (
                this.player.transform.position.x + this._direction.x * speed * dt,
                this.player.transform.position.y,
                this.player.transform.position.z + this._direction.y * speed * dt
            );
        }
    }
}
