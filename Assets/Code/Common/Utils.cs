using System;

using UnityEngine;

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
}
