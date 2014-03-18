using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanDynamiteCharged : PacmanTileItem 
{
	protected bool destroyed = false;
	protected float timer = 0.0f;
	protected float chargeTime = 1.0f;
	protected bool isCounting = false;

	protected void Update()
	{
		if (!isCounting)
			return;

		if (timer < chargeTime)
			timer += Time.deltaTime;
		else
			DestroyTileItem();
	}

	public void IsCounting(bool counting)
	{
		isCounting = counting;
	}
	
	public override void DestroyTileItem ()
	{
		if (destroyed)
			return;

		destroyed = true;

		LugusCoroutines.use.StartRoutine(DestroyDynamite());
	}

	protected IEnumerator DestroyDynamite()
	{
		if (parentTile == null)
		{
			Debug.LogError("PacmanDynamiteCharged: Parent tile was null");
			yield break;
		}

		GameObject explosion = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("DynamiteExplosion"));
		explosion.transform.position = parentTile.GetWorldLocation().v3().zAdd(-5.0f);

		this.GetComponent<SpriteRenderer>().enabled = false;

		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("Explosion01"));

		yield return new WaitForSeconds(0.5f);

		parentTile.tileItems.Remove(this);	// remove this FIRST - otherwise neighboring dynamites could endlessly destroy eachother

		// see if we're exploding the player
		if (parentTile == PacmanGameManager.use.GetActivePlayer().currentTile)
		{
			PacmanGameManager.use.GetActivePlayer().DoHitEffect();	
		}

		// clear tiles around blast area
		foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
		{
			// see if we're exploding the player
			if (tile == PacmanGameManager.use.GetActivePlayer().currentTile)
			{
				PacmanGameManager.use.GetActivePlayer().DoHitEffect();
			}

			Destroy(tile.rendered);
			tile.tileType = PacmanTile.TileType.Open;

			// destroy tile items on neighboring tiles
			for (int i = tile.tileItems.Count - 1; i >= 0; i--) 
			{
				tile.tileItems[i].DestroyTileItem();	// item in question determines how it is destroyed	
			}
		}

		// also destroy any remaining items on this tile itself

	
		for (int i = parentTile.tileItems.Count - 1; i >= 0; i--) 
		{
			PacmanTileItem tileItem = parentTile.tileItems[i];

			tileItem.DestroyTileItem();	// item in question determines how it is destroyed

			// also allow lava to flow into newly opened up tiles
			if (tileItem.GetComponent<PacmanLavaTile>() != null)
			{
				tileItem.GetComponent<PacmanLavaTile>().RegisterSurroundingTiles();
			}

			if (tileItem.GetComponent<PacmanLavaTileStart>() != null)
			{
				tileItem.GetComponent<PacmanLavaTileStart>().RegisterSurroundingTiles();
			}
		}




		Destroy(explosion);
		Destroy(gameObject);
	}
}
