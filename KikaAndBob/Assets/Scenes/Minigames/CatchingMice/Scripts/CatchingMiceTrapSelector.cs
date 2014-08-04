using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceTrapSelector : LugusSingletonExisting<CatchingMiceTrapSelector>
{
	public float itemScale = 0.7f;
	public float scrollSpeed = 0.5f;
	public bool dragging = false;

	protected Transform itemsParent = null;
	protected Transform itemsEnd = null;
	protected CatchingMiceTrap currentSelectedTrap = null;
	protected Vector3 itemOffset = new Vector3(0, -2, 0);
	protected List<TrapSelectorItem> items = new List<TrapSelectorItem>();
	protected Transform mouseDragger = null;
	protected SpriteRenderer dragSprite = null;
	protected SpriteRenderer dragSpriteArrow = null;
	protected CatchingMiceTile currentHoverTile = null;
	protected Button buttonUp = null;
	protected Button buttonDown = null;
	protected Vector2 maxScroll = Vector2.zero;
	protected Vector2 currentScroll = Vector2.zero;
	protected int currentTopTrap = 0;
	protected Vector3 startPosition = Vector3.zero;
	protected Transform background = null;
	protected bool scrollNecessary = false;
	protected TextMeshWrapper costDisplay = null;
	protected bool visible = true;


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

		if (itemsEnd == null)
		{
			itemsEnd = this.transform.FindChild("ItemsEnd");
		}
		
		if (itemsEnd == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing items end transform.");
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

		if (buttonUp == null)
		{
			buttonUp = transform.FindChild("ButtonUp").GetComponent<Button>();
		}

		if (buttonUp == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing up button.");
		}

		if (buttonDown == null)
		{
			buttonDown = transform.FindChild("ButtonDown").GetComponent<Button>();
		}
		
		if (buttonDown == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing down button.");
		}

		if (background == null)
		{
			background = transform.FindChild("Background");
		}
		
		if (background == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing background.");
		}

		if (costDisplay == null)
		{
			costDisplay = transform.FindChild("Cost").GetComponent<TextMeshWrapper>();
		}

		if (costDisplay == null)
		{
			Debug.LogError("CatchingMiceTrapSelector: Missing cost display text mesh.");
		}


		startPosition = itemsParent.transform.localPosition;
	}



	public void SetupGlobal()
	{
		CatchingMiceGameManager.use.onPickupCountChanged += CreateTrapList;

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

	public void CreateTrapList(int newAmount)
	{
		foreach(TrapSelectorItem item in items)
		{
			Destroy(item.button.gameObject);
		}

		items.Clear();

		int offsetCounter = 1;
		Vector3 currentOffset = Vector3.zero;

		if (CatchingMiceLevelManager.use.CurrentLevel == null)
		{
			return;
		}

		if (CatchingMiceLevelManager.use.CurrentLevel.availableTraps.Length <= 0)
		{
			Debug.Log("CatchingMiceTrapSelector: Disabled trap selector because there aren't any traps available.");
			SetVisible(false);
			return;
		}
		else
		{
			SetVisible(true);
		}

		foreach(CatchingMiceTrap trap in CatchingMiceLevelManager.use.trapPrefabs)
		{
	
			bool availableTrap = false;

			foreach(string s in CatchingMiceLevelManager.use.CurrentLevel.availableTraps)
			{
				if (s == trap.name)
				{
					availableTrap = true;
					break;
				}
			}

			if (!availableTrap)
				continue;

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

			TextMeshWrapper newCostDisplay = (TextMeshWrapper) Instantiate(costDisplay);
			newCostDisplay.SetText(trap.Cost.ToString());

			newCostDisplay.transform.parent = trapButton.transform;
			newCostDisplay.transform.localPosition = new Vector3(2, 0, -1);

			// using icon.bounds.extents.y messes up when newCostDisplay is already parent to a scaled object - use world position instead
			// Then, also subtract a bit because the text mesh's pivot doesn't exactly line up with the text bottom...
			newCostDisplay.transform.position = newCostDisplay.transform.position.y(icon.bounds.min.y - 0.2f);
		

			// grey out trap icon and do not add collider if necessary
			if (trap.Cost > newAmount)
			{
				icon.color = icon.color.a(0.4f);
				newCostDisplay.textMesh.color = newCostDisplay.textMesh.color.a(0.4f);
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

		// update max scroll length
		maxScroll = currentOffset.yAdd(0.2f);	// add a little buffer at the bottom for the number - eyeballing

		// is the list of trap icons longer than the window?
		if (maxScroll.y <= Vector3.Distance(startPosition, itemsEnd.position))
			scrollNecessary = false;
		else
			scrollNecessary = true;

		// if so, display scroll arrows; if not, hide them
		if (scrollNecessary)
		{
			buttonUp.gameObject.SetActive(true);
			buttonDown.gameObject.SetActive(true);
		}
		else
		{
			buttonUp.gameObject.SetActive(false);
			buttonDown.gameObject.SetActive(false);
		}
	}

	// makes the whole trap selector bar visibile/invisible
	public void SetVisible(bool enabled)
	{
		foreach(Transform t in this.transform)
		{
			t.gameObject.SetActive(enabled);
		}

		visible = enabled;

		if (visible)
		{
			HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.TopRight, KikaAndBob.ScreenAnchor.Top);
		}
		else
		{
			HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.TopRight, KikaAndBob.ScreenAnchor.TopRight);
		}
	}


	protected void Update () 
	{
		// hide in menus or when there are no traps available
		if (!CatchingMiceGameManager.use.GameRunning)
		{
			if (visible)
			{
				SetVisible(false);
			}
		}
		else
		{
			if (!visible && CatchingMiceLevelManager.use.CurrentLevel != null && CatchingMiceLevelManager.use.CurrentLevel.availableTraps.Length > 0)
			{
				SetVisible(true);
			}
		}

		dragging = false;

		if (LugusInput.use.down)
		{
			currentHoverTile = null;

			Transform hit = LugusInput.use.RayCastFromMouseDown(LugusCamera.ui);

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

			if (currentSelectedTrap != null && currentHoverTile != null && currentSelectedTrap.ValidateTile(currentHoverTile))
			{
				if (CatchingMiceGameManager.use.PickupCount >= currentSelectedTrap.Cost)
				{
					CatchingMiceGameManager.use.PickupCount -= Mathf.RoundToInt(currentSelectedTrap.Cost);
					ScoreVisualizer.use.NewScore(KikaAndBob.CommodityType.Cookie, -currentSelectedTrap.Cost).Animate(false).Execute();
					CatchingMiceLevelManager.use.InstantiateTrap(currentSelectedTrap, currentHoverTile);			
				}
			}

			ResetDrag();
		}

		if (LugusInput.use.dragging)
		{
			if (currentSelectedTrap != null)
				dragging = true;

			DragSprite();
		}

		// selector scroll
		if (LugusInput.use.down || LugusInput.use.dragging)
		{
			Transform hit = LugusInput.use.RayCastFromMouse();

			if (hit == buttonUp.transform)
			{
				itemsParent.localPosition = itemsParent.localPosition.yAdd(-scrollSpeed * Time.timeScale);
			}
			else if (hit == buttonDown.transform)
			{
				itemsParent.localPosition = itemsParent.localPosition.yAdd(scrollSpeed * Time.timeScale);
			}

			ClampScroll();
		}
	}
	
	protected void ClampScroll()
	{
		if (!scrollNecessary)
			return;

		itemsParent.transform.localPosition = new Vector3(
			itemsParent.transform.localPosition.x,
			Mathf.Clamp(itemsParent.transform.localPosition.y, startPosition.y, (maxScroll.y - Mathf.Abs(itemsEnd.localPosition.y))),
			itemsParent.transform.localPosition.z);
	}

	protected void ResetDrag()
	{
		dragSprite.enabled = false;
		dragSprite.color = Color.white;
		dragSpriteArrow.enabled = false;
		currentSelectedTrap = null;
		currentHoverTile = null;
	}


	protected void DragSprite()
	{
		if (currentSelectedTrap == null)
			return;

		Vector3 clampPosition = LugusCamera.ui.ScreenToWorldPoint(LugusInput.use.lastPoint).z(-1);
		
		currentHoverTile = CatchingMiceLevelManager.use.GetTileFromMousePosition(LugusCamera.game, false);


		// check if hovering over trap selector
		// if so, make it invalid to place a trap
		//RaycastHit[] hits = Physics.RaycastAll(LugusCamera.ui.ScreenPointToRay(LugusInput.use.lastPoint));

		RaycastHit2D[] hits = Physics2D.RaycastAll(LugusCamera.ui.ScreenToWorldPoint(LugusInput.use.lastPoint), Vector3.forward);
		
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider != null && hit.collider.transform == background)
			{
				currentHoverTile = null;
				break;
			}
		}

		
		if (currentHoverTile != null && currentSelectedTrap.ValidateTile(currentHoverTile))
		{
			if (dragSpriteArrow.enabled == false)
				dragSpriteArrow.enabled = true;
			
		//	clampPosition = currentHoverTile.waypoint.transform.position.z(clampPosition.z);
			dragSprite.transform.localPosition = new Vector3(0, 1.5f, 0);

			dragSprite.color = Color.white;
			
		}
		else
		{
			if (dragSpriteArrow.enabled == true)
				dragSpriteArrow.enabled = false;

			dragSprite.transform.localPosition = Vector3.zero;
			dragSprite.color = Color.red;
		}
		
		mouseDragger.transform.position = clampPosition;
	}
}


