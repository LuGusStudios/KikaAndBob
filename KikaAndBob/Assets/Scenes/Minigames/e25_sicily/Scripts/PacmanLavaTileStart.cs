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

		GameObject particleObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("FireParticles"));
		ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
		Vector3 playerPos = PacmanGameManager.use.GetActivePlayer().transform.position.zAdd(-1f);
		particleObject.transform.position = playerPos;
		ps.Play();

		yield return new WaitForSeconds(1.0f);

		GameObject ashObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("AshPile"));
		ashObject.transform.position = playerPos;
		PacmanGameManager.use.GetActivePlayer().gameObject.SetActive(false);


		yield return new WaitForSeconds(1.0f);

		Destroy(ashObject, 1.0f);	// we need to destroy this some time!
		PacmanGameManager.use.LoseLife();
	}

	public override void Initialize ()
	{
		if (originLavaTile == null)
		{
			Debug.LogError("PacmanLavaTileStart: Origin tile was null. Not continuing.");
			return;
		}


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

		StartCoroutine(ShowLavaFlow());
	}

	protected IEnumerator ShowLavaFlow()
	{

		Vector3[] path = new Vector3[3];

		float zLocation = originLavaTile.transform.position.z;

		// create a path consisting of:
		// 0 - the edge shared between the origin tile and this one
		// 1 - the center of this tile
		// 2 - ???

		path[0] = Vector3.Lerp(originLavaTile.transform.position, parentTile.GetWorldLocation().v3(), 0.5f).z(zLocation);
		path[1] = parentTile.GetWorldLocation().v3().z(zLocation);

		Vector3 fromStartToMiddle = path[1] - path[0];
		
		path[2] = path[1] + fromStartToMiddle;	// extend vector edge-middle - this is standard choice


		if (surroundingOpenTiles.Count == 1  &&														// if c
		    (originLavaTile.parentTile.gridIndices.x != surroundingOpenTiles[0].gridIndices.x &&
			 originLavaTile.parentTile.gridIndices.y != surroundingOpenTiles[0].gridIndices.y))
		{
			path[2] = Vector3.Lerp(parentTile.GetWorldLocation().v3(), surroundingOpenTiles[0].GetWorldLocation().v3(), 0.5f).z(zLocation);
		}
//		else if (surroundingOpenTiles.Count == 0 && surroundingLavaTiles.Count >= 2)	// this is an edge corner
//		{
//			PacmanTile targetTile = null;
//
//			while (targetTile == null || targetTile == originLavaTile)		// if this is the case, pick a random other lava tile (not origin tile) to turn towards
//			{
//				targetTile = surroundingLavaTiles[Random.Range(0, surroundingLavaTiles.Count)];
//			}
//
//
//			path[2] = Vector3.Lerp(parentTile.GetWorldLocation().v3(), targetTile.GetWorldLocation().v3(), 0.5f).z(zLocation);
//		}


		TrailRenderer trailRenderer = this.GetComponent<TrailRenderer>();
		trailRenderer.time = 13.0f;


		transform.position = path[0];
		gameObject.MoveTo(path).Time(1.0f).MoveToPath(false).Execute();

		float timer = 0.0f;

		while(timer < 1.0f)
		{
			trailRenderer.startWidth = Mathf.Lerp(0, 0.9f, timer/0.9f);
			timer += Time.deltaTime;
			yield return null;
		}


		GameObject newLavaTileObject = (GameObject) Instantiate(originLavaTile.gameObject);
		newLavaTileObject.transform.position = parentTile.GetWorldLocation().v3().z(this.transform.position.z);
		newLavaTileObject.transform.position += new Vector3(0, 0, -0.2f);

		newLavaTileObject.name = "LavaTile" + parentTile.ToString();	// might as well assign a name to prevent "Name(Clone)(Clone)(Clone)(Clone)(Clone)" ...

		// we dont want to copy the original lava tile's particle effect - this indicates the 'origin' lava tile
		// this will basically only happen for the first 'generation' of child lava tiles

		if (newLavaTileObject.GetComponentInChildren<ParticleSystem>() != null)
			Destroy(newLavaTileObject.GetComponentInChildren<ParticleSystem>().gameObject);
	
		if (PacmanLevelManager.use.temporaryParent != null)
		{
			newLavaTileObject.transform.parent = PacmanLevelManager.use.temporaryParent;
		}
		else
		{
			Debug.LogWarning("PacmanLavaTileStart: No temporary items parent found. This tile will not be removed in the next round!");
			newLavaTileObject.transform.parent = this.transform.parent;
		}


		done = true;	// set this true so the lava tile created below will be the one the player can 'die' on
		PacmanLavaTile newLavaTile = newLavaTileObject.GetComponent<PacmanLavaTile>();
		newLavaTile.parentTile = parentTile;

		parentTile.tileItems.Add(newLavaTile);
		parentTile.tileItems.Remove(this);


		newLavaTile.RegisterSurroundingTiles();
		newLavaTile.InitializeSprite();


		Material oldMaterial = trailRenderer.material;
		SpriteRenderer newSprite = newLavaTile.GetComponent<SpriteRenderer>();

		timer = 0.0f; 
		float fadeDuration = 1.0f;



		Vector3 originalScale = newSprite.transform.localScale;
		while (timer < fadeDuration)
		{
			//oldMaterial.color = oldMaterial.color.a(Mathf.Lerp(1.0f, 0.0f, timer/fadeDuration)); 
			newSprite.color = newSprite.color.a(Mathf.Lerp(0.0f, 1.0f, timer/fadeDuration));

//			newSprite.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, timer/fadeDuration);

			timer += Time.deltaTime; 
			yield return null;
		}

		newLavaTile.Initialize();

		timer = 0.0f;
		while (timer < fadeDuration)
		{
			oldMaterial.color = oldMaterial.color.a(Mathf.Lerp(1.0f, 0.0f, timer/fadeDuration)); 
			timer += Time.deltaTime;
			yield return null;
		}

		Destroy(this.gameObject);
	}
}
