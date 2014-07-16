using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceWorldObjectTrapGround : CatchingMiceTrap
{
    public override void SetTileType(List<CatchingMiceTile> tiles)
    {
        // Every tile can be placed
        foreach (CatchingMiceTile tile in tiles)
        {
			// TODO: Figure out why the tile is searched for again...?
			// !!!!! Warning: this could potentially lead to a neighboring tile being selected!
			// When a grid index is at i.e. 17, but the system might represent this as 16.9999999, rounding off here to 16!!!
            /*CatchingMiceTile levelTile = CatchingMiceLevelManager.use.tiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];

			if (levelTile != tile)
			{
				CatchingMiceLogVisualizer.use.LogError("OMG: Wut is this sorcery?");
			}

            //Adds the furniture type to the tile with the or operator because a tile multiple types (ex. a tile can have a trap on a furniture)
            levelTile.tileType = levelTile.tileType | tileType;
            levelTile.trapObject = this;

            transform.position = transform.position.yAdd(gridOffset).zAdd(-0.25f);*/
			
			tile.tileType = tile.tileType | tileType;
			tile.trap = this;
        }
		
		transform.position = transform.position.yAdd(yOffset).zAdd(-zOffset);
    }

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture)
		{
			CatchingMiceLogVisualizer.use.LogError("Ground trap " + transform.name + " cannot be placed on furniture.");
			return false;
		}
		else if ((tile.trap != null) || ((tile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap))
		{
			CatchingMiceLogVisualizer.use.LogError("Ground trap " + transform.name + " cannot be placed because another trap is already present.");
			return false;
		}
		else if ((tile.obstacle != null) || ((tile.tileType & CatchingMiceTile.TileType.Obstacle) == CatchingMiceTile.TileType.Obstacle))
		{
			CatchingMiceLogVisualizer.use.LogError("Ground trap " + transform.name + " cannot be placed because an obstacle is already present.");
			return false;
		}

		return true;
	}
}
