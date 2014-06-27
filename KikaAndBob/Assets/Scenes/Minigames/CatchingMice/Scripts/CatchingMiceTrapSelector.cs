using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceTrapSelector : MonoBehaviour 
{
	public float itemScale = 0.7f;
	protected Transform itemsParent = null;
	protected CatchingMiceTrap currentSelectedTrap = null;
	protected Vector3 itemOffset = new Vector3(0, -2, 0);
	protected List<TrapSelectorItem> items = new List<TrapSelectorItem>();
	protected Transform mouseDragger = null;
	protected SpriteRenderer dragSprite = null;
	protected SpriteRenderer dragSpriteArrow = null;
	protected CatchingMiceTile currentHoverTile = null;

	protected struct TrapSelectorItem
	{
		public Transform button;
		public Sprite sprite;
		public CatchingMiceTrap trap;
	}
	
	public void SetupLocal()
	{
		if (itemsParent == null)
		{
			itemsParent = transform.FindChild("Items");
		}

		if (itemsParent == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing items transform.");
		}

		if (mouseDragger == null)
		{
			mouseDragger = this.transform.FindChild("MouseDragger");
		}

		if (mouseDragger == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing mouse dragger.");
		}
		
		
		if (dragSprite == null)
		{
			dragSprite = this.transform.FindChild("MouseDragger/TrapSprite").GetComponent<SpriteRenderer>();
		}

		if (dragSprite == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing drag sprite.");
		}

		if (dragSpriteArrow == null)
		{
			dragSpriteArrow = this.transform.FindChild("MouseDragger/Arrow").GetComponent<SpriteRenderer>();
		}
		
		if (dragSpriteArrow == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing drag sprite arrow.");
		}
		else
		{
			dragSpriteArrow.gameObject.MoveTo(dragSpriteArrow.transform.localPosition + new Vector3(0, 0.5f, 0)).IsLocal(true).Time(0.5f).Looptype(iTween.LoopType.pingPong).Execute();
		}

		CatchingMiceGameManager.use.onPickupCountChanged += CreateTrapList;
	}



	public void SetupGlobal()
	{
		CreateTrapList();
		ResetDrag();
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void CreateTrapList()
	{
		items.Clear();

		int offsetCounter = 1;
		Vector3 currentOffset = Vector3.zero;

		foreach(CatchingMiceTrap trap in CatchingMiceLevelManager.use.trapPrefabs)
		{
			SpriteRenderer spriteRenderer = trap.GetComponentsInChildren<SpriteRenderer>(true)[0];
			
			if (spriteRenderer == null)
			{
				Debug.LogError("CatchingMiceTrapSelector: Missing sprite renderer on trap: " + trap.name);
				continue;
			}

			GameObject trapButton = new GameObject(trap.name);
			trapButton.transform.parent = itemsParent.transform;
			trapButton.transform.localScale = Vector3.one * itemScale;
			trapButton.layer = LayerMask.NameToLayer("GUI");

			SpriteRenderer icon = trapButton.AddComponent<SpriteRenderer>();
			icon.sprite = spriteRenderer.sprite;
			currentOffset += icon.bounds.extents;	// increase offset by half the sprite
			trapButton.transform.localPosition = new Vector3(0, -currentOffset.y, 0);	// change this to x (or z) for a menu in a different direction
			currentOffset += icon.bounds.extents;	// then increase it again for the other half

			if (trap.Cost > CatchingMiceGameManager.use.PickupCount)
			{
				icon.color = icon.color.a(0.4f);
			}
			else
			{
				BoxCollider2D buttonCollider = trapButton.AddComponent<BoxCollider2D>();
			}

			TrapSelectorItem newSelectorItem = new TrapSelectorItem();
			newSelectorItem.button = trapButton.transform;
			newSelectorItem.sprite = icon.sprite;
			newSelectorItem.trap = trap;
			
			items.Add(newSelectorItem);
		}
	}

	
	protected void Update () 
	{
		if (LugusInput.use.down)
		{
			currentHoverTile = null;

			Transform hit = LugusInput.use.RayCastFromMouseDown();

			foreach(TrapSelectorItem item in items)
			{
				if (hit == item.button)
				{
					currentSelectedTrap = item.trap;
					dragSprite.enabled = true;
					dragSprite.sprite = item.sprite;
					dragSprite.transform.localScale = item.button.localScale;
					break;
				}
			}
		}
		else if (LugusInput.use.up)
		{
			DragSprite();

			if (currentSelectedTrap != null && currentHoverTile != null)
			{
				if (CatchingMiceGameManager.use.PickupCount >= currentSelectedTrap.Cost)
				{
					CatchingMiceGameManager.use.PickupCount -= Mathf.RoundToInt(currentSelectedTrap.Cost);
					CatchingMiceLevelManager.use.InstantiateTrap(currentSelectedTrap, currentHoverTile);
				}
			}

			ResetDrag();
		}

		if (LugusInput.use.dragging)
		{
			DragSprite();
		}
	}

	protected void ResetDrag()
	{
		dragSprite.enabled = false;
		dragSpriteArrow.enabled = false;
		currentSelectedTrap = null;
		currentHoverTile = null;
	}
	
//	protected void PlaceTrap(CatchingMiceTile tile)
//	{
//		if (tile == null)
//			return;
//
//		if (currentSelectedTrap == null)
//		{
//			Debug.LogError("CatchingMiceTrapSelector: Selected trap was null. This shouldn't happen!");
//			return;
//		}
//
//
//		// Create the trap item
//		GameObject tileItem = (GameObject)Instantiate(currentSelectedTrap.gameObject);
//		tileItem.transform.parent = CatchingMiceLevelManager.use.ObjectParent;
//		tileItem.transform.name += " " + targetTile.gridIndices;
//		tileItem.transform.localPosition = targetTile.location;
//		
//		CatchingMiceTrap trap = tileItem.GetComponent<CatchingMiceTrap>();
//		if (trap != null)
//		{
//			if (trap.CalculateColliders())
//			{
//				trap.parentTile = targetTile;
//				
//				if ((targetTile.tileType & CatchingMiceTile.TileType.Trap) != CatchingMiceTile.TileType.Trap)
//				{
//					CatchingMiceLogVisualizer.use.LogWarning("The tile type of the tile has no trap flag set!");
//				}
//				
//				trapTiles.Add(targetTile);
//				trap.Stacks = definition.stacks;
//			}
//			else
//			{
//				CatchingMiceLogVisualizer.use.LogError("The trap " + trap.name + " could not be placed on the grid.");
//				DestroyGameObject(trap.gameObject);
//			}
//		}
//		else
//		{
//			CatchingMiceLogVisualizer.use.LogError("The trap prefab " + trapPrefab.name + " does not have a Trap component attached.");
//			DestroyGameObject(tileItem);
//		}
//
//
//	}

	protected void DragSprite()
	{
		if (currentSelectedTrap == null)
			return;

		Vector3 clampPosition = LugusCamera.ui.ScreenToWorldPoint(LugusInput.use.lastPoint).z(-1);
		
		currentHoverTile = CatchingMiceLevelManager.use.GetTileFromMousePosition(false);
		
		if (currentHoverTile != null && currentSelectedTrap.ValidateTile(currentHoverTile))
		{
			if (dragSpriteArrow.enabled == false)
				dragSpriteArrow.enabled = true;
			
			clampPosition = currentHoverTile.waypoint.transform.position.z(clampPosition.z);
			dragSprite.transform.localPosition = new Vector3(0, 1.5f, 0);
			
		}
		else
		{
			if (dragSpriteArrow.enabled == true)
				dragSpriteArrow.enabled = false;
			dragSprite.transform.localPosition = Vector3.zero;
		}
		
		mouseDragger.transform.position = clampPosition;
	}


	
	
	
	
}


