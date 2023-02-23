using System;
using Cysharp.Threading.Tasks;
using Gameplay;
using LevelDesigner;
using Project;
using UnityEngine;
using Zenject;

namespace Scene
{
	public class SceneManager : MonoBehaviour
	{
		[Inject] readonly Constructor _constructor;
		[Inject] readonly GameManager _gameManager;
		[Inject] readonly CustomLevelContext _customLevelContext;
		[Inject] readonly ZenjectSceneLoader _sceneLoader;

		[InjectOptional] readonly StartGameConfiguration _startGameConfiguration;

		void Start()
		{
			// No transition data, then just start the game
			if (_startGameConfiguration == null)
			{
				_gameManager.StartLevel(0);
				//_constructor.Activate();
				return;
			}

			if (_startGameConfiguration.constructorMode)
			{
				_constructor.Activate();
			}
			else if (_customLevelContext.HasCustomLevelForNewGame)
			{
				int levelNumber = _startGameConfiguration.levelNumber;
				var customLevelData = _customLevelContext.GetNewGameLevel();
				
				_gameManager.StartLevel(levelNumber, customLevelData);
				_customLevelContext.ClearNewGameCustomLevel();
			}
			else
			{
				int levelNumber = _startGameConfiguration.levelNumber;
				
				_gameManager.StartLevel(levelNumber);
			}
		}

		public void MoveToMenu()
		{
			_sceneLoader.LoadSceneAsync("MainMenu");
		}
	}
}