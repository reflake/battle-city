using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using Zenject;

// Player controls its tank
public class Player : MonoBehaviour
{
	[Inject] private readonly Tank _tank = null;
	
	private TankControls _tankControls;
	private Direction _lastKnownMoveDirection = Direction.North;
	private Dictionary<string, Direction> _currentlyPressedDirections = new Dictionary<string, Direction>();

	private void Awake()
	{
		_tankControls = new TankControls();
		
		_tankControls.Enable();

		var movement = _tankControls.Movement;
		
		BindInputDirections("Horizontal", movement.Horizontal, Direction.East, Direction.West);
		BindInputDirections("Vertical", movement.Vertical, Direction.North, Direction.South);

		_tankControls.Action.Shoot.started += ShootInputPressed;
	}

	private void BindInputDirections(string keyName, InputAction bindInputAction, Direction positiveDirection, Direction negativeDirection)
	{
		bindInputAction.performed += InputPressed;
		bindInputAction.canceled += InputCanceled;

		void InputPressed(InputAction.CallbackContext context)
		{
			float val = context.ReadValue<float>();

			AxisToDirection(val);
		}

		void InputCanceled(InputAction.CallbackContext _)
		{
			_currentlyPressedDirections.Remove(keyName);
			
			TryStop();
		}

		void AxisToDirection(float val)
		{
			switch (val)
			{
				case 1:
					Move(positiveDirection);
					_currentlyPressedDirections[keyName] = positiveDirection;
					break;
				case -1:
					Move(negativeDirection);
					_currentlyPressedDirections[keyName] = negativeDirection;
					break;
			}
		}
	}

	private void TryStop()
	{
		if (_currentlyPressedDirections.Count == 0)
		{
			Move(Direction.None);
		}
		else
		{
			Move(_currentlyPressedDirections.Values.First());
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