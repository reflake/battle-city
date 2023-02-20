using System;
using UnityEngine;

namespace Common
{
	public static class Utils
	{
		public static Vector2 ToVector(this Direction direction)
		{
			switch (direction)
			{
				case Direction.North: return Vector2.up;
				case Direction.South: return Vector2.down;
				case Direction.East: return Vector2.right;
				case Direction.West: return Vector2.left;
			}

			throw new Exception($"No vector for this direction: {direction}");
		}

		public static Quaternion DirectionToRotation(this Direction direction)
		{
			switch (direction)
			{
				case Direction.East: return Quaternion.Euler(0,0,0);
				case Direction.North: return Quaternion.Euler(0,0,90);
				case Direction.West: return Quaternion.Euler(0,0,180);
				case Direction.South: return Quaternion.Euler(0,0,270);
			}

			throw new Exception("Unexpected behaviour");
		}

		public static void TurnToDirection(this SpriteRenderer spriteRenderer, Direction direction)
		{
			spriteRenderer.transform.rotation = direction.DirectionToRotation();
		}
	}
}
