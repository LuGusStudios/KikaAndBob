using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanPope :  PacmanTileItem
{
	public override void Initialize ()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}

	public override void OnTryEnter ()
	{
		if (PacmanLevelManager.use.AllItemsPickedUp())
		{
			PacmanGameManager.use.WinGame();
		}
	}
}
