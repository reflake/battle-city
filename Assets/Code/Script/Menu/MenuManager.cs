using System;
using Cysharp.Threading.Tasks;
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
		[SerializeField] Button exitButton = default;
		
		void Awake()
		{
			playerOneButton.onClick.AddListener(StartGame);
			exitButton.onClick.AddListener(Exit);
		}

		void StartGame()
		{
			LoadGameplay();
		}

		async UniTaskVoid LoadGameplay()
		{
			await _sceneLoader.LoadSceneAsync("Gameplay", LoadSceneMode.Single);
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