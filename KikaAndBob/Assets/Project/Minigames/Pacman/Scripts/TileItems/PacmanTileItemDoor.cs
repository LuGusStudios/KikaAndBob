using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItemDoor : PacmanTileItem 
{
	public string keyID = "Key01";
	protected bool opened = false;

	public override void Initialize ()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}

	public override void OnTryEnter ()
	{
		if (!opened && PacmanPickups.use.GetPickupAmount(keyID) >= 1)
		{
			opened = true;
			PacmanPickups.use.ModifyPickupAmount(keyID, -1);
			parentTile.tileType = PacmanTile.TileType.Open;
			this.gameObject.SetActive(false);
		}
	}

}
