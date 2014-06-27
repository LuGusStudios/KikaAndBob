using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceFurniture : CatchingMiceWorldObject {

	public override void SetTileType(List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			// TODO: Figure out why the tile is searched for again...?
			// !!!!! Warning: this could potentially lead to a neighboring tile being selected!
			// When a grid index is at i.e. 17, but the system might represent this as 16.9999999, rounding off here to 16!!!
			/*CatchingMiceTile levelTile = CatchingMiceLevelManager.use.Tiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];

			if (levelTile != tile)
			{
				CatchingMiceLogVisualizer.use.LogError("OMG: Wut is this sorcery?");
			}
			levelTile.tileType = tileType;
			levelTile.furniture = this;

			//the z axis will be the anchor point of the object. So the anchor point needs the be the lowest tile of the sprite
			levelTile.location.z = transform.position.z;*/

			if ((tile.tileType & CatchingMiceTile.TileType.Ground) == CatchingMiceTile.TileType.Ground)
			{
				tile.tileType -= CatchingMiceTile.TileType.Ground;
			}

			tile.tileType = tile.tileType | tileType;
			tile.furniture = this;
			
			tile.location.z = transform.position.z;
		}

		// Place the furniture a little bit forward, so that it does not
		// interfere with the texture of the ground
		transform.position = transform.position.zAdd(-zOffset);
	}
}
