using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

using Zenject;

// Player controls its tank
public partial class Player : MonoBehaviour
{
	[SerializeField] int lives = 3;

	[Inject] readonly PlayerManager _playerManager = null;
	[Inject] readonly Tank _tank = null;
	[Inject] readonly PlayerSpritesData _playerSpritesData;

	public int Lives => lives;

	// Events
	public event LivesChangeDelegate OnLivesChange;
	
	TankControls _tankControls;
	Direction _lastKnownMoveDirection = Direction.North;
	Dictionary<string, Direction> _currentlyPressedDirections = new();
	int _upgradeLevel = 0;

	[Inject]
	void Construct()
	{
		_playerManager.OnSpawnPlayers += InitialPlayerSpawn;
		_playerManager.OnDespawnPlayers += PlayerDespawn;
		
		_tank.OnGetKilled += PlayerKilled;
		_tank.SpritesData = _playerSpritesData;
	}

	void Awake()
	{
		_tankControls = new TankControls();
		
		SetControlsEnabled(true);

		// Bind inputs to tank actions
		var movement = _tankControls.Movement;
		
		BindInputMoveDirections("Horizontal", movement.Horizontal, Direction.East, Direction.West);
		BindInputMoveDirections("Vertical", movement.Vertical, Direction.North, Direction.South);

		_tankControls.Action.Shoot.started += ShootInputPressed;
	}

	void OnDestroy()
	{
		_tank.OnGetKilled -= PlayerKilled;
	}

	void BindInputMoveDirections(string keyName, InputAction bindInputAction, Direction positiveDirection, Direction negativeDirection)
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

	void TryStop()
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

	void Move(Direction newDirection)
	{
		if (newDirection != Direction.None)
		{
			_lastKnownMoveDirection = newDirection;
		}
		
		_tank.SetMoveDirection(newDirection);
	}

	void ShootInputPressed(InputAction.CallbackContext _)
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

	public void SetPlayerSpawn(Vector2 spawnPosition)
	{
		_tank.SetSpawnPosition(spawnPosition);
	}
	
	void InitialPlayerSpawn()
	{
		RespawnTank();
	}

	void PlayerDespawn()
	{
		gameObject.SetActive(false);
	}

	void PlayerKilled()
	{
		if (lives > 0)
		{
			lives--;
			
			OnLivesChange?.Invoke(lives);

			RespawnTank();;
		}
		else
		{
			_playerManager.PlayerDefeated();
		}
	}

	async UniTaskVoid RespawnTank()
	{
		// TODO: show respawn animation
		// Wait before respawn
		await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.DeltaTime, PlayerLoopTiming.Update);
		
		_tank.Respawn();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.TryGetComponent<PowerUp>(out var powerUp))
		{
			powerUp.PickupByPlayer(this);
		}
	}

	public void UpgradeTank()
	{
		_upgradeLevel++;

		switch (_upgradeLevel)
		{
			case 1:
				_tank.ProjectileSpeed = 1.5f;
				break;
			case 2:
				_tank.FireRate = 2;
				break;
			case 3:
				_tank.FirePower = 2;
				break;
		}
	}
}