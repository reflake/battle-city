using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay
{
	public class EscapeHandler : MonoBehaviour
	{
		[Inject] readonly ZenjectSceneLoader _sceneLoader;
		
		Keyboard _kbDevice;

		void Awake()
		{
			_kbDevice = InputSystem.GetDevice<Keyboard>();
		}
		
		void Update()
		{
			if (_kbDevice.escapeKey.IsPressed())
			{
				_sceneLoader.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
			}
		}
	}
}