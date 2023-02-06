using UnityEngine;
using UnityEngine.InputSystem;

using Zenject;

// Player controls its tank
public class Player : MonoBehaviour
{
	[Inject] private readonly Tank _tank = null;
	
	private TankControls _tankControls;
	private Direction _lastKnownMoveDirection = Direction.North;

	private void Awake()
	{
		_tankControls = new TankControls();
		
		_tankControls.Enable();
		
		_tankControls.Movement.Horizontal.performed += HorizontalMovementInputPressed;
		_tankControls.Movement.Horizontal.canceled += HorizontalMovementInputCanceled;
		_tankControls.Movement.Vertical.performed += VerticalMovementInputPressed;
		_tankControls.Movement.Vertical.canceled += VerticalMovementInputCanceled;

		_tankControls.Action.Shoot.started += ShootInputPressed;
	}

	private void HorizontalMovementInputCanceled(InputAction.CallbackContext _)
	{
		TryStop();
	}

	private void VerticalMovementInputCanceled(InputAction.CallbackContext _)
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

	private void Move(Direction newDirection)
	{
		if (newDirection != Direction.None)
		{
			_lastKnownMoveDirection = newDirection;
		}
		
		_tank.SetMoveDirection(newDirection);
	}

	private void ShootInputPressed(InputAction.CallbackContext _)
	{
		_tank.Shoot(_lastKnownMoveDirection);
	}
}