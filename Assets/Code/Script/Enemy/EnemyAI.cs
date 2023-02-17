using System.Collections.Generic;
using UnityEngine;

using Zenject;
using Random = UnityEngine.Random;

public partial class EnemyAI : MonoBehaviour
{
	[Inject] readonly Tank _tank = null;
	[Inject] readonly EnemyManager _enemyManager = null;
	
	[SerializeField] Vector2 _shootDelayRange = Vector2.zero;
	[SerializeField] Vector2 _moveDelayRange = Vector2.zero;

	Direction _currentDirection = Direction.None;
	float _thinkShootTimer = 0f;
	float _thinkMoveTimer = 0f;
	float _lastTurnTime = 0f;

	public Tank Tank => _tank;

	[Inject]
	void Construct()
	{
		_tank.OnGetHit += EnemyHit;
		_tank.OnGetKilled += EnemyKilled;
	}

	void EnemyHit(DamageData damagedata)
	{
		if (_tank.Powered)
		{
			_enemyManager.DropPowerUp();
		
			_tank.Powered = false;
		}
	}

	void EnemyKilled()
	{
		_enemyManager.EnemyKilled(this);
	}

	void Start()
	{
		ChangeDirection();
	}

	void Update()
	{
		ThinkShoot();
		ThinkMove();
	}

	void ThinkShoot()
	{
		if (_lastTurnTime + 1.5f > Time.time)
			return;
		
		if (_thinkShootTimer > Time.time)
			return;
		
		_tank.Shoot(_currentDirection);

		ShootCooldown();
	}

	void ShootCooldown()
	{
		_thinkShootTimer = Time.time + Random.Range(_shootDelayRange.x, _shootDelayRange.y);
	}

	void ThinkMove()
	{
		if (_thinkMoveTimer > Time.time)
			return;

		ChangeDirection();

		_thinkMoveTimer = Time.time + Random.Range(_moveDelayRange.x, _moveDelayRange.y);
	}

	void ChangeDirection()
	{
		var newPossibleDirections = new List<Direction> { Direction.North, Direction.South, Direction.East, Direction.West };
		int randomDirectionIndex = Random.Range(0, 3);

		newPossibleDirections.Remove(_currentDirection);

		_currentDirection = newPossibleDirections[randomDirectionIndex];
		
		_tank.SetMoveDirection(_currentDirection);

		_lastTurnTime = Time.time;
	}

	public void Spawn(Vector2 position)
	{
		ShootCooldown();
		
		_tank.SetSpawnPosition(position);
		_tank.Respawn();
	}

	public void ShouldDropPowerUp()
	{
		_tank.Powered = true;
	}
}