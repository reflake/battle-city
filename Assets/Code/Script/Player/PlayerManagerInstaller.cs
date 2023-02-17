﻿using UnityEngine;
using Zenject;

public class PlayerManagerInstaller : MonoInstaller
{
	[SerializeField] Player playerPrefab = null;
	
	public override void InstallBindings()
	{
		Container
			.BindFactory<PlayerSpritesData, Player, Player.Factory>()
			.FromSubContainerResolve()
			.ByNewContextPrefab<PlayerInstaller>(playerPrefab.gameObject);
	}
}