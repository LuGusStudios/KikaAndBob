using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceWorldObject : MonoBehaviour
{
	public float yOffset = 0.5f;
	public float zOffset = 0f;
	public CatchingMiceTile.TileType tileType = CatchingMiceTile.TileType.None;
	public CatchingMiceTile parentTile = null;
	public List<CatchingMiceTile> overlappingTiles = new List<CatchingMiceTile>();	// List of all tiles this object overlaps

	protected BoxCollider2D[] boxColliders2D;

	public virtual bool CalculateColliders()
	{
		boxColliders2D = GetComponentsInChildren<BoxCollider2D>();
		List<CatchingMiceTile> tileList = new List<CatchingMiceTile>();

		if (boxColliders2D.Length <= 0) {
			CatchingMiceLogVisualizer.use.Log("No collider has been found. Will be using 1 tile");
			CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTileByLocation(transform.position.x, transform.position.y);

			if (ValidateTile(tile))
			{
				tileList.Add(tile);
			}
			else
			{
				return false;
			}
		}
		else
		{
			foreach (BoxCollider2D col2D in boxColliders2D)
			{
				float xTiles = Mathf.Ceil(col2D.size.x / CatchingMiceLevelManager.use.scale);
				float yTiles = Mathf.Ceil(col2D.size.y / CatchingMiceLevelManager.use.scale);

				// Needs to go over every other point
				for (int y = 1; y < (int)yTiles * 2; y += 2)
				{
					for (int x = 1; x < (int)xTiles * 2; x += 2)
					{
						// Gets most left position of the collider and add the wanted tile distance
						float xTile = ((col2D.transform.position.x + col2D.center.x) - col2D.Bounds().extents.x) + xTiles / (xTiles * 2) * x;
						// Shifts the tile gridOffset down first, then gets the lowest position and add the wanted tile distance
						float yTile = ((col2D.transform.position.y + col2D.center.y) - col2D.Bounds().extents.y) - yOffset + yTiles / (yTiles * 2) * y;
						
						CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTile(Mathf.RoundToInt(xTile / CatchingMiceLevelManager.use.scale), Mathf.RoundToInt(yTile / CatchingMiceLevelManager.use.scale));

						if (ValidateTile(tile))
						{
							tileList.Add(tile);
						}
						else
						{
							return false;
						}
					}
				}
			}
		}

		SetTileType(tileList);

		overlappingTiles = tileList;

		return true;
	}

	public virtual void SetTileType(List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			// TODO: Figure out why the tile is searched for again...?
			// !!!!! Warning: this could potentially lead to a neighboring tile being selected!
			// When a grid index is at i.e. 17, but the system might represent this as 16.9999999, rounding off here to 16!!!
			/*CatchingMiceTile levelTile = CatchingMiceLevelManager.use.Tiles[(int)tile.gridIndices.x, (int)tile.gridIndices.y];

			if (levelTile != tile)
			{
				Debug.LogError("OMG: Wut is this sorcery?");
			}
			levelTile.tileType = tileType;
			levelTile.furniture = this;

			//the z axis will be the anchor point of the object. So the anchor point needs the be the lowest tile of the sprite
			levelTile.location.z = transform.position.z;*/

			tile.tileType = tile.tileType | tileType;
			tile.location.z = transform.position.z;
		}
	}

	public virtual bool ValidateTile(CatchingMiceTile tile)
	{
		if (tile == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The object " + transform.name + " cannot be placed on a null tile.");
			return false;
		}
		else if ((tileType & tile.tileType) == tileType)
		{
			CatchingMiceLogVisualizer.use.LogError("The tile already contains an item of the same type as " + transform.name + ".");
			return false;
		}

		return true;
	}
}
