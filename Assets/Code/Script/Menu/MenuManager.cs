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

		async UniTaskVoid LoadGameplayScene(StartGameConfiguration configuration)
		{
			await _sceneLoader.LoadSceneAsync("Gameplay", LoadSceneMode.Single, container =>
			{
				container.BindInstance(configuration);
			});
		}

		void StartGame()
		{
			LoadGameplayScene(new StartGameConfiguration
			{
				constructorMode = false,
				levelNumber = 0,
			});
		}

		void LaunchConstructorMode()
		{
			LoadGameplayScene(new StartGameConfiguration { constructorMode = true, });
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