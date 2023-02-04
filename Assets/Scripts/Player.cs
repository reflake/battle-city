using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// Player controls its tank
public class Player : MonoBehaviour
{
	[Inject] private readonly Tank _tank = null;
	
	private TankControls _tankControls;

	private void Awake()
	{
		_tankControls = new TankControls();
		
		_tankControls.Enable();
		
		_tankControls.Movement.Horizontal.performed += HorizontalMovementInputPressed;
		_tankControls.Movement.Horizontal.canceled += HorizontalMovementInputCanceled;
		_tankControls.Movement.Vertical.performed += VerticalMovementInputPressed;
		_tankControls.Movement.Vertical.canceled += VerticalMovementInputCanceled;
	}

	private void HorizontalMovementInputCanceled(InputAction.CallbackContext obj)
	{
		TryStop();
	}

	private void VerticalMovementInputCanceled(InputAction.CallbackContext obj)
	{
		TryStop();
	}

	private void HorizontalMovementInputPressed(InputAction.CallbackContext context)
	{
		float val = context.ReadValue<float>();

		HorizontalAxisToDirection(val);
	}

	private void HorizontalAxisToDirection(float val)
	{
		switch (val)
		{
			case 1:
				Move(Direction.East);
				break;
			case -1:
				Move(Direction.West);
				break;
		}
	}

	private void VerticalMovementInputPressed(InputAction.CallbackContext context)
	{
		float val = context.ReadValue<float>();

		VerticalAxisToDirection(val);
	}

	private void VerticalAxisToDirection(float val)
	{
		switch (val)
		{
			case 1:
				Move(Direction.North);
				break;
			case -1:
				Move(Direction.South);
				break;
		}
	}

	private void TryStop()
	{
		if (!_tankControls.Movement.Horizontal.inProgress && !_tankControls.Movement.Vertical.inProgress)
		{
			Move(Direction.None);
		}
		else
		{
			float horizontalAxis = _tankControls.Movement.Horizontal.ReadValue<float>();
			float verticalAxis = _tankControls.Movement.Vertical.ReadValue<float>();
			
			HorizontalAxisToDirection(horizontalAxis);
			VerticalAxisToDirection(verticalAxis);
		}
	}

	private void Move(Direction dir)
	{
		_tank.SetMoveDirection(dir);
	}
}