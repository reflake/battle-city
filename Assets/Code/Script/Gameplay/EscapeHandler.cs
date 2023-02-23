using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Scene;

namespace Gameplay
{
	public class EscapeHandler : MonoBehaviour
	{
		[Inject] readonly SceneManager _sceneManager;
		
		Keyboard _kbDevice;

		void Awake()
		{
			_kbDevice = InputSystem.GetDevice<Keyboard>();
		}
		
		void Update()
		{
			if (_kbDevice.escapeKey.IsPressed())
			{
				_sceneManager.MoveToMenu();
			}
		}
	}
}