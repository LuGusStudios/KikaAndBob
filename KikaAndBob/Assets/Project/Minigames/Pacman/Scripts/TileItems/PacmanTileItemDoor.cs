using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItemDoor : PacmanTileItem 
{
	public string keyID = "Key01";

	public override void Initialize ()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}

	public override void OnTryEnter ()
	{
		if (PacmanPickups.use.GetPickupAmount(keyID) >= 1)
		{
			PacmanPickups.use.ModifyPickupAmount(keyID, -1);
			parentTile.tileType = PacmanTile.TileType.Open;
			this.gameObject.SetActive(false);
		}
	}

}
