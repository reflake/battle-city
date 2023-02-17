using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DebugGizmos : MonoBehaviour
{
	struct Box
	{
		public float TimeToLife;
		public Bounds Bounds;
	}
	
	public static DebugGizmos Instance = null;

	List<Box> boxes = new();
	
	void Awake()
	{
		Instance = this;
	}

	public void DrawBox(Bounds bounds)
	{
		#if UNITY_EDITOR
		
		boxes.Add(new Box
		{
			TimeToLife = Time.unscaledTime + 1.5f,
			Bounds = bounds
		});
		
		#endif
	}

#if UNITY_EDITOR

	void LateUpdate()
	{
		boxes.RemoveAll(box => box.TimeToLife < Time.unscaledTime);
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;

		foreach (var box in boxes)
		{
			Gizmos.DrawWireCube(box.Bounds.center, box.Bounds.size);
		}
		
	}
#endif
}