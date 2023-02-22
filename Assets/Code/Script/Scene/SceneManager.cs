using System;
using Gameplay;
using LevelDesigner;
using UnityEngine;
using Zenject;

namespace Scene
{
	public class SceneManager : MonoBehaviour
	{
		[Inject] readonly Constructor _constructor;
		[Inject] readonly GameManager _gameManager;

		[InjectOptional] readonly MainMenuTransitionData _mainMenuMainMenuTransitionData;

		void Start()
		{
			// No transition data, then just start the game
			if (_mainMenuMainMenuTransitionData == null)
			{
				// _gameManager.SetLevel(0);
				_constructor.Activate();
				return;
			}

			if (_mainMenuMainMenuTransitionData.constructorMode)
			{
				_constructor.Activate();
			}
			else
			{
				_gameManager.SetLevel(_mainMenuMainMenuTransitionData.levelNumber);
			}
		}
	}
}