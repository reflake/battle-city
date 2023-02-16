using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[CreateAssetMenu]
[Serializable]
public class AnimatedObjectTile : TileBase
{
	public Sprite[] m_AnimatedSprites;
	public float m_MinSpeed = 1f;
	public float m_MaxSpeed = 1f;
	public float m_AnimationStartTime;
	public int m_AnimationStartFrame = 0;
	public Tile.ColliderType m_TileColliderType;
	public GameObject m_InstancedGameObject;
	
	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		tileData.transform = Matrix4x4.identity;
		tileData.color = Color.white;
		tileData.gameObject = m_InstancedGameObject;
		if (m_AnimatedSprites != null && m_AnimatedSprites.Length > 0)
		{
			tileData.sprite = m_AnimatedSprites[m_AnimatedSprites.Length - 1];
			tileData.colliderType = m_TileColliderType;
		}
	}
	
	public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
	{
		if (m_AnimatedSprites.Length > 0)
		{
			tileAnimationData.animatedSprites = m_AnimatedSprites;
			tileAnimationData.animationSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);
			tileAnimationData.animationStartTime = m_AnimationStartTime;
			if (0 < m_AnimationStartFrame && m_AnimationStartFrame <= m_AnimatedSprites.Length)
			{
				var tilemapComponent = tilemap.GetComponent<Tilemap>();
				if (tilemapComponent != null && tilemapComponent.animationFrameRate > 0)
					tileAnimationData.animationStartTime = (m_AnimationStartFrame - 1) / tilemapComponent.animationFrameRate;
			}
			return true;
		}
		return false;
	}
}