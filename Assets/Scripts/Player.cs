using System;
using UnityEngine;
using Zenject;

// Player controls its tank
public class Player : MonoBehaviour
{
	[Inject] private readonly Tank _tank = null;

	void FixedUpdate()
	{
		float inputAxisX = Input.GetAxis("Horizontal");
		float inputAxisY = Input.GetAxis("Vertical");
		
		var newDirection = DirectionByInput(inputAxisX, inputAxisY);
		
		_tank.SetMoveDirection(newDirection);
	}

	Direction DirectionByInput(float inputAxisX, float inputAxisY)
	{
		const float inputEpsilon = 0.05f;
        
		if (Mathf.Abs(inputAxisY) > inputEpsilon)
		{
			return inputAxisY > 0 ? Direction.North : Direction.South;
		}
		else if (Mathf.Abs(inputAxisX) > inputEpsilon)
		{
			return inputAxisX > 0 ? Direction.East : Direction.West;
		}
		else
		{
			return Direction.None;
		}
	}
}