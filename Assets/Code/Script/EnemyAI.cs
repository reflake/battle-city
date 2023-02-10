using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Zenject;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
	[Inject] private readonly Tank _tank = null;
	[Inject] private readonly EnemyManager _enemyManager = null;
	
	[SerializeField] private Vector2 _shootDelayRange = Vector2.zero;
	[SerializeField] private Vector2 _moveDelayRange = Vector2.zero;

	private Direction _currentDirection = Direction.None;
	private float _thinkShootTimer = 0f;
	private float _thinkMoveTimer = 0f;

	[Inject]
	private void Construct()
	{
		_tank.OnGetKilled += EnemyKilled;
	}

	private void EnemyKilled()
	{
		_enemyManager.EnemyKilled(this);
	}

	private void Start()
	{
		ChangeDirection();
	}

	private void Update()
	{
		ThinkShoot();
		ThinkMove();
	}

	private void ThinkShoot()
	{
		if (_thinkShootTimer > Time.time)
			return;
		
		_tank.Shoot(_currentDirection);

		_thinkShootTimer = Time.time + Random.Range(_shootDelayRange.x, _shootDelayRange.y);
	}

	private void ThinkMove()
	{
		if (_thinkMoveTimer > Time.time)
			return;

		ChangeDirection();

		_thinkMoveTimer = Time.time + Random.Range(_moveDelayRange.x, _moveDelayRange.y);
	}

	private void ChangeDirection()
	{
		var newPossibleDirections = new List<Direction> { Direction.North, Direction.South, Direction.East, Direction.West };
		int randomDirectionIndex = Random.Range(0, 3);

		newPossibleDirections.Remove(_currentDirection);

		_currentDirection = newPossibleDirections[randomDirectionIndex];
		
		_tank.SetMoveDirection(_currentDirection);
	}
}