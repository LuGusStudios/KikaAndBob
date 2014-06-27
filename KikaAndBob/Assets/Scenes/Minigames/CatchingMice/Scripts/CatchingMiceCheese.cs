using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCheese : CatchingMiceWorldObject {

	public List<Sprite> cheeseVisuals = new List<Sprite>();

	public int Stacks
	{
		get
		{
			return stacks;
		}

		set
		{
			stacks = value;

			// Destroy the cheese if 0 stacks are left,
			// change sprite otherwise
			if (stacks <= 0)
			{
				DestroySelf();
			}
			else
			{
				int visualIndex = Mathf.FloorToInt(((float)stacks / (float)initialStacks) * (float)cheeseVisuals.Count);
				visualIndex = Mathf.Max(Mathf.Min(visualIndex, cheeseVisuals.Count - 1), 0);

				if (cheeseRenderer != null)
				{
					cheeseRenderer.sprite = cheeseVisuals[visualIndex];
				}
			}
		}
	}

	protected int stacks;
	protected int initialStacks;

	protected SpriteRenderer cheeseRenderer = null;

	public void SetupGlobal()
	{
		initialStacks = stacks;

		if (cheeseRenderer == null)
		{
			cheeseRenderer = GetComponentInChildren<SpriteRenderer>();
			if (cheeseRenderer == null)
			{
				CatchingMiceLogVisualizer.use.LogError("Could not find the sprite renderer for the cheese.");
			}
			else
			{
				cheeseRenderer.sprite = cheeseVisuals[cheeseVisuals.Count - 1];
			}
		}
	}

	protected void Start()
	{
		SetupGlobal();
	}

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
			tile.cheese = this;
		}
		
		transform.position = transform.position.yAdd(yOffset).zAdd(-zOffset);
	}

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		// A cheese object can only be placed on the ground, and should not be placed
		// on the same tile as a trap

		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.tileType & CatchingMiceTile.TileType.Furniture) == CatchingMiceTile.TileType.Furniture)
		{
			CatchingMiceLogVisualizer.use.LogError("Cheese " + transform.name + " cannot be placed on furniture.");
			return false;
		}
		else if ((tile.trap != null) || ((tile.tileType & CatchingMiceTile.TileType.Trap) == CatchingMiceTile.TileType.Trap))
		{
			CatchingMiceLogVisualizer.use.LogError("Cheese " + transform.name + " cannot be placed on the same tile as a trap.");
			return false;
		}

		return true;
	}

	protected void DestroySelf()
	{
		CatchingMiceLevelManager.use.RemoveCheeseFromTile(parentTile);
		gameObject.SetActive(false);
	}
}
