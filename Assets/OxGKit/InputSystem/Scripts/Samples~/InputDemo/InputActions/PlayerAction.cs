using System;
using OxGKit.InputSystem;
using UnityEngine.InputSystem;
using OxGKit.InputSystem.Example;

public class PlayerAction : IInputAction
{
    /*
	 * NOTE: You can implements input signals from Unity New Input System, also can implements from other plugins.
	 *
     * [Handle Input Action Example]
     *  
     * Inputs.IA.GetInputAction<TInputAction>().onAction += Somethings.Handler;
     */

    public event Action<MoveInputComposite.MoveInput> onMoveAction;

    public void OnCreate()
    {
        var ctrls = Inputs.CM.GetControlMap<PlayerControls>();
        if (ctrls != null)
        {
            // add move input performed (player move)
            ctrls.Player.Move.performed += this.OnMoveAction;
            // add move input canceled to reset input data
            ctrls.Player.Move.canceled += this.OnMoveAction;
        }
    }

    public void OnUpdate(float dt)
    {
        /*
		 * Do Somethings OnUpdate
         */
    }

    public void RemoveAllListeners()
    {
        this.onMoveAction = null;
    }

    protected void OnMoveAction(InputAction.CallbackContext context)
    {
        // If there is a special input can make composite by yourself
        // Read your composite values
        var moveInput = context.ReadValue<MoveInputComposite.MoveInput>();
        this.onMoveAction?.Invoke(moveInput);
    }
}