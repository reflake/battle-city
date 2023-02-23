using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.Rendering;

public class DebugGizmos : MonoBehaviour
{
	class Box
	{
		public Bounds Bounds;
	}
	
	public static DebugGizmos Instance = null;

	List<Box> boxes = new();
	
	void Awake()
	{
		Instance = this;
	}

	public async UniTaskVoid DrawBox(Bounds bounds)
	{
		#if UNITY_EDITOR

		Box box = new Box
		{
			Bounds = bounds
		};
		
		boxes.Add(box);

		await UniTask.Delay(1500);

		boxes.Remove(box);

#endif
	}

#if UNITY_EDITOR

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