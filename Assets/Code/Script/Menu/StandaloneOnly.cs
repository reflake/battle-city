using System;
using UnityEngine;

namespace Menu
{
	public class StandaloneOnly : MonoBehaviour
	{
		void Awake()
		{
			#if !UNITY_STANDALONE
			
			gameObject.SetActive(false);
			
			#endif 
		}
	}
}