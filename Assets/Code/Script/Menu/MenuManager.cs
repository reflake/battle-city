using System;
using Cysharp.Threading.Tasks;
using Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Menu
{
	public class MenuManager : MonoBehaviour
	{
		[Inject] readonly ZenjectSceneLoader _sceneLoader;
		
		[SerializeField] Button playerOneButton = default;
		[SerializeField] Button constructorModeButton = default;
		[SerializeField] Button exitButton = default;
		
		void Awake()
		{
			playerOneButton.onClick.AddListener(StartGame);
			constructorModeButton.onClick.AddListener(LaunchConstructorMode);
			exitButton.onClick.AddListener(Exit);
		}

		void StartGame()
		{
			async UniTaskVoid LoadGameplay()
			{
				await _sceneLoader.LoadSceneAsync("Gameplay", LoadSceneMode.Single, container =>
				{
					container.BindInstance(new MainMenuTransitionData
					{
						constructorMode = false,
						levelNumber = 0,
					});
				});
			}
			
			LoadGameplay();
		}

		void LaunchConstructorMode()
		{
			async UniTaskVoid LoadConstructorMode()
			{
				await _sceneLoader.LoadSceneAsync("Gameplay", LoadSceneMode.Single, container =>
				{
					container.BindInstance(new MainMenuTransitionData { constructorMode = true, });
				});
			}

			LoadConstructorMode();
		}

		void Exit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
		}
	}
}