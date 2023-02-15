using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

using Zenject;

// Player controls its tank
public class Player : MonoBehaviour
{
	[SerializeField] private int lives = 3;

	[Inject] private readonly PlayerManager _playerManager = null;
	[Inject] private readonly Tank _tank = null;
	
	private TankControls _tankControls;
	private Direction _lastKnownMoveDirection = Direction.North;
	private Dictionary<string, Direction> _currentlyPressedDirections = new();

	[Inject]
	private void Construct()
	{
		_tank.OnGetKilled += PlayerKilled;
	}

	private void Awake()
	{
		_tankControls = new TankControls();
		
		SetControlsEnabled(true);

		// Bind inputs to tank actions
		var movement = _tankControls.Movement;
		
		BindInputMoveDirections("Horizontal", movement.Horizontal, Direction.East, Direction.West);
		BindInputMoveDirections("Vertical", movement.Vertical, Direction.North, Direction.South);

		_tankControls.Action.Shoot.started += ShootInputPressed;
	}

	private void OnDestroy()
	{
		_tank.OnGetKilled -= PlayerKilled;
	}

	private void BindInputMoveDirections(string keyName, InputAction bindInputAction, Direction positiveDirection, Direction negativeDirection)
	{
		bindInputAction.performed += MoveInputPressed;
		bindInputAction.canceled += MoveInputCanceled;

		void MoveInputPressed(InputAction.CallbackContext context)
		{
			float val = context.ReadValue<float>();

			// Get direction by axis value
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

		void MoveInputCanceled(InputAction.CallbackContext _)
		{
			_currentlyPressedDirections.Remove(keyName);
			
			TryStop();
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

	public void SetControlsEnabled(bool enabled)
	{
		if (enabled)
		{
			_tankControls.Enable();
		}
		else
		{
			_tankControls.Disable();
		}
	}

	private void PlayerKilled()
	{
		if (lives > 0)
		{
			lives--;

			Task.Run(() => RespawnTank());;
		}
		else
		{
			_playerManager.PlayerDefeated();
		}
	}

	private async UniTaskVoid RespawnTank()
	{
		// TODO: show respawn animation
		// Wait before respawn
		await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.DeltaTime, PlayerLoopTiming.Update);
		
		_tank.Respawn();
	}
}