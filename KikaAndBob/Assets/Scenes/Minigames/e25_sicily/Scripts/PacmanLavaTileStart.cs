using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanLavaTileStart : PacmanTileItem 
{
	public PacmanLavaTile originLavaTile = null;
	public List<PacmanTile> surroundingLavaTiles = new List<PacmanTile>();
	public List<PacmanTile> surroundingOpenTiles = new List<PacmanTile>();

	public Sprite lava1Way = null;
	public Sprite lava2WayStraight = null;
	public Sprite lava2WayCorner = null;
	public Sprite lava3Way = null;

	protected bool done = false;

	// OnEnter alone won't do - we also want this to take effect if the player is hit by the lava without running into it themselves
	protected void Update()
	{
		if (!done && PacmanGameManager.use.gameRunning && PacmanGameManager.use.GetActivePlayer().currentTile == parentTile)
			LugusCoroutines.use.StartRoutine(BurnUp());
	}

    public override void OnEnter(PacmanCharacter character)
	{
		if (done)	// serves both to only call this once AND to make sure it doesn't run anymore once the proper lava tile is present
			return;


		LugusCoroutines.use.StartRoutine(BurnUp());
	}

	protected IEnumerator BurnUp()
	{
		PacmanGameManager.use.gameRunning = false;

		done = true;

		GameObject flameObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("FlameAnimation"));
		Vector3 playerPos = PacmanGameManager.use.GetActivePlayer().transform.position.zAdd(-1f);
		flameObject.transform.position = playerPos;
		flameObject.transform.parent = PacmanLevelManager.use.temporaryParent;

		PacmanGameManager.use.GetActivePlayer().HideCharacter();

		yield return new WaitForSeconds(0.5f);

		GameObject ashObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("AshPile"));
		ashObject.transform.position = playerPos;
		ashObject.transform.parent = PacmanLevelManager.use.temporaryParent;

		yield return new WaitForSeconds(1.5f);

		PacmanGameManager.use.LoseLife();
	}

	public override void Initialize ()
	{
		if (originLavaTile == null)
		{
			Debug.LogError("PacmanLavaTileStart: Origin tile was null. Not continuing.");
			return;
		}

		RegisterSurroundingTiles();

		StartCoroutine(ShowLavaFlow());
	}

	public void RegisterSurroundingTiles()
	{
		foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
		{
			bool isOpenTile = true;
			
			foreach (PacmanTileItem tileItem in tile.tileItems)
			{
				if (tileItem.GetComponent<PacmanLavaTile>() != null || tileItem.GetComponent<PacmanLavaTileStart>() != null)
				{
					surroundingLavaTiles.Add(tile);
					isOpenTile = false;
					break;
				}
				else if (tileItem.GetComponent<PacmanLavaStop>() != null)
				{
					isOpenTile = false;
					break;
				}
			}
			
			if (isOpenTile && tile.tileType != PacmanTile.TileType.Collide)
			{
				surroundingOpenTiles.Add(tile);
			}
		}
	}

	protected IEnumerator ShowLavaFlow()
	{
		// don't want this continuing after winning or losing
		if (!PacmanGameManager.use.gameRunning)
			yield break;


		Vector3[] path = new Vector3[3];

		float zLocation = originLavaTile.transform.position.z;

		// create a path consisting of:
		// 0 - the edge shared between the origin tile and this one
		// 1 - the center of this tile
		// 2 - extending 0-1, or rounding a corner if one is available

		path[0] = Vector3.Lerp(originLavaTile.transform.position, parentTile.GetWorldLocation().v3(), 0.5f).z(zLocation);
		path[1] = parentTile.GetWorldLocation().v3().z(zLocation);

		Vector3 fromStartToMiddle = path[1] - path[0];
		
		path[2] = path[1] + fromStartToMiddle;	// extend vector edge-middle - this is the standard choice


		if (surroundingOpenTiles.Count == 1  &&														// if corner
		    (originLavaTile.parentTile.gridIndices.x != surroundingOpenTiles[0].gridIndices.x &&
			 originLavaTile.parentTile.gridIndices.y != surroundingOpenTiles[0].gridIndices.y))
		{
			path[2] = Vector3.Lerp(parentTile.GetWorldLocation().v3(), surroundingOpenTiles[0].GetWorldLocation().v3(), 0.5f).z(zLocation);
		}


		transform.position = path[0];
		gameObject.MoveTo(path).Time(1.0f).MoveToPath(false).Execute();

		float timer = 0.0f;

		TrailRenderer trailRenderer = this.GetComponent<TrailRenderer>();

		// make tip of lava flow expand as destination approaches (0.9 is about right width)
		while(timer < 1.0f)
		{
			trailRenderer.startWidth = Mathf.Lerp(0, 0.9f, timer/0.9f);
			timer += Time.deltaTime;
			yield return null;
		}

		// some items (e.g. dynamite) might have a custom effect when they are touched by lava
		for (int i = parentTile.tileItems.Count - 1; i >= 0; i--) 
		{
			parentTile.tileItems[i].DestroyTileItem();			
		}

		// now create the actual lava tie
		GameObject newLavaTileObject = (GameObject) Instantiate(originLavaTile.gameObject);
		newLavaTileObject.transform.position = parentTile.GetWorldLocation().v3().z(this.transform.position.z);
		newLavaTileObject.transform.position += new Vector3(0, 0, -0.2f);

		newLavaTileObject.name = "LavaTile" + parentTile.ToString();	// might as well assign a name to prevent "Name(Clone)(Clone)(Clone)(Clone)(Clone)" ...

		// we dont want to copy the original lava tile's particle effect - this indicates the 'origin' lava tile
		// this will basically only happen for the first 'generation' of child lava tiles
		if (newLavaTileObject.GetComponentInChildren<ParticleSystem>() != null)
			Destroy(newLavaTileObject.GetComponentInChildren<ParticleSystem>().gameObject);
	
		// assign lava tile to the temporary items Transform; this will make sure it is removed in the next round
		if (PacmanLevelManager.use.temporaryParent != null)
		{
			newLavaTileObject.transform.parent = PacmanLevelManager.use.temporaryParent;
		}
		else
		{
			Debug.LogWarning("PacmanLavaTileStart: No temporary items parent found. This tile will not be removed in the next round!");
			newLavaTileObject.transform.parent = this.transform.parent;
		}


		done = true;	// set this true so the lava tile created below will be the one the player can now 'die' on instead of this one
						// this may not have any effect, since this script is also removed from the tile item list, but a fallback is not a bad idea in case it somehow stays around

		// assign parentTile of the new TileItem
		PacmanLavaTile newLavaTile = newLavaTileObject.GetComponent<PacmanLavaTile>();
		newLavaTile.parentTile = parentTile;

		// switch out these two tileItemScripts
		parentTile.tileItems.Add(newLavaTile);
		parentTile.tileItems.Remove(this);

		// set up the new lava tile script
		newLavaTile.RegisterSurroundingTiles();
		newLavaTile.InitializeSprite();


		Material oldMaterial = trailRenderer.material;
		SpriteRenderer newSprite = newLavaTile.GetComponent<SpriteRenderer>();

		timer = 0.0f; 
		float fadeDuration = 1.0f;

		// start fading the new sprite in
		while (timer < fadeDuration)
		{
			newSprite.color = newSprite.color.a(Mathf.Lerp(0.0f, 1.0f, timer/fadeDuration));

			timer += Time.deltaTime; 
			yield return null;
		}

		// begin new lava flow starting from the new lava tile
		newLavaTile.Initialize();

		// fade out trail
		timer = 0.0f;
		while (timer < fadeDuration)
		{
			oldMaterial.color = oldMaterial.color.a(Mathf.Lerp(1.0f, 0.0f, timer/fadeDuration)); 
			timer += Time.deltaTime;
			yield return null;
		}

		// eventually remove this object altogether
		Destroy(this.gameObject);
	}
}
