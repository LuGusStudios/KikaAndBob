using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanDestroyRock : PacmanTileItem 
{
	public override void Initialize ()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}
	
	public override void DestroyTileItem ()
	{
		parentTile.tileType = PacmanTile.TileType.Open;
		parentTile.tileItems.Remove(this);
		
		foreach(Renderer r in GetComponentsInChildren<Renderer>(true))
		{
			r.enabled = false;
		}
		
		GameObject destroyParticlesObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("DestroySmoke"));
		destroyParticlesObject.transform.position = parentTile.GetWorldLocation().v3().zAdd(-5.0f);
		destroyParticlesObject.transform.parent = PacmanLevelManager.use.temporaryParent;
		
		ParticleSystem destroyParticles = destroyParticlesObject.GetComponent<ParticleSystem>();
		destroyParticles.Play();

		foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
		{
			foreach(PacmanTileItem tileItem in tile.tileItems)
			{
				if (tileItem.GetComponent<PacmanLavaTile>() != null)
				{
					PacmanLavaTile lavaTile = tileItem.GetComponent<PacmanLavaTile>();

					lavaTile.Reset();
					lavaTile.InitializeSprite();
				}
			}
		}

		
		Destroy(this.gameObject, 2.0f);
	}
}
