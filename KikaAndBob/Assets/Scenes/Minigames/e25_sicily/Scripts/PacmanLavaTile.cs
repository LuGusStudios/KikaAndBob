using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanLavaTile : PacmanTileItem 
{
	protected float updateCheckSpeed = 0.25f;

	public override void Initialize ()
	{
		//parentTile.tileType = PacmanTile.TileType.Lethal;

		StartCoroutine(UpdateRoutine());	// starting this the old way - it doesn't need to be terminated, and this way it will be stopped if the object disappears
	}

	protected IEnumerator UpdateRoutine()
	{
		while (true)
		{
			foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
			{
				if (tile == null)
					continue;

				if (tile.tileType != PacmanTile.TileType.Collide)
				{
					GameObject newLavaTile = (GameObject) Instantiate(this.gameObject);
					newLavaTile.transform.position = tile.GetWorldLocation().v3().z(this.transform.position.z);
					newLavaTile.name = "LavaTile" + tile.ToString();	// might as well assign a name to prevent "Name(Clone)(Clone)(Clone)(Clone)(Clone)" ...

				}
			}


			yield return new WaitForSeconds(updateCheckSpeed);
		}

	}

	public override void OnTryEnter ()
	{
	}

	public override void OnEnter ()
	{
	}
}
