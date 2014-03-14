using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanLavaTileStart : PacmanTileItem 
{
	public PacmanLavaTile originLavaTile = null;
	public List<PacmanTile> surroundingLavaTiles = new List<PacmanTile>();
	public List<PacmanTile> surroundingOpenTiles = new List<PacmanTile>();

	protected bool done = false;

	public override void OnEnter ()
	{
		if (done)
			return;

		// TO DO: Lose life
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
			bool isLavaTile = false;

			foreach (GameObject tileItem in tile.tileItems)
			{
				if (tileItem.GetComponent<PacmanLavaTile>() != null || tileItem.GetComponent<PacmanLavaTileStart>() != null)
				{
					surroundingLavaTiles.Add(tile);
					isLavaTile = true;
					break;
				}
			}

			if (!isLavaTile && tile.tileType != PacmanTile.TileType.Collide)
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

		if (surroundingOpenTiles.Count <= 0)
		{
			path[2] = path[1];
		}
		else if (surroundingOpenTiles.Count >= 1)
		{
			path[2] = Vector3.Lerp(parentTile.GetWorldLocation().v3(), surroundingOpenTiles[0].GetWorldLocation().v3(), 0.5f).z(zLocation);
		}


		transform.position = path[0];
		gameObject.MoveTo(path).Time(1.0f).MoveToPath(false).Execute();

		yield return new WaitForSeconds(1.0f);


		GameObject newLavaTileObject = (GameObject) Instantiate(originLavaTile.gameObject);
		newLavaTileObject.transform.position = parentTile.GetWorldLocation().v3().z(this.transform.position.z);
		newLavaTileObject.name = "LavaTile" + parentTile.ToString();	// might as well assign a name to prevent "Name(Clone)(Clone)(Clone)(Clone)(Clone)" ...
		newLavaTileObject.transform.parent = this.transform.parent;

		PacmanLavaTile newLavaTile = newLavaTileObject.GetComponent<PacmanLavaTile>();
		newLavaTile.parentTile = parentTile;
		parentTile.tileItems.Add(newLavaTile.gameObject);
		newLavaTile.Initialize();

		Material oldMaterial = this.GetComponent<TrailRenderer>().material;
		SpriteRenderer newSprite = newLavaTile.GetComponent<SpriteRenderer>();

		float timer = 0.0f;
		float fadeDuration = 1.0f;

		while (timer < fadeDuration)
		{
			oldMaterial.SetColor("_Tint", oldMaterial.GetColor("_Tint").a(Mathf.Lerp(1.0f, 0.0f, timer/fadeDuration))); 
			newSprite.color = newSprite.color.a(Mathf.Lerp(0.0f, 1.0f, timer/fadeDuration));
			timer += Time.deltaTime;
			yield return null;
		}

		parentTile.tileItems.Remove(this.gameObject);
		Destroy(this.gameObject);
	}

}
