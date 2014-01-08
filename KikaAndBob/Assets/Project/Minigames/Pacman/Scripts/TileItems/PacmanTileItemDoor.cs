using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItemDoor : PacmanTileItem 
{
	public string keyID = "Key01";

	protected void Start()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}

	public override void OnTryEnter ()
	{
		if (PacmanPickups.use.GetPickups(keyID) >= 1)
		{
			PacmanPickups.use.ModifyPickups(keyID, -1);
			parentTile.tileType = PacmanTile.TileType.Open;
			this.gameObject.SetActive(false);
		}
	}

}
