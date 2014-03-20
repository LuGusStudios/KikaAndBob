using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanBomberUpdater : PacmanLevelUpdater 
{
	public float bombTime = 5;
	protected ParticleSystem explosionEffect = null;
	protected GameObject bombPrefab = null;
	protected bool running = false;
	protected List<PacmanTile> chargedTiles = new List<PacmanTile>();

	protected class ChargedTile
	{
		public PacmanTile attachedTile = null;
		public GameObject bombItem = null;
		private float duration = 0;
		private float timer;

		public ChargedTile(PacmanTile _tile, float _duration, GameObject _bombItem)
		{
			attachedTile = _tile;
			duration = _duration;
			bombItem = _bombItem;
		}

		public void UpdateCharge()
		{
			if (timer < duration)
			{
				timer += Time.deltaTime;
			}
		}

		public bool TimerDone()
		{
			return timer >= duration;
		}
	}

	public override void Activate()
	{
		explosionEffect = PacmanLevelManager.use.GetPrefab("Explosion").GetComponent<ParticleSystem>();
		bombPrefab = PacmanLevelManager.use.GetPrefab("Bomb");
		running = true;
	}
	
	public override void Deactivate()
	{
		running = false;
	}

	private void Update()
	{
		if (!running)
			return;

		PacmanPlayerCharacter player = PacmanGameManager.use.GetActivePlayer();

		// place bombs
		if (PacmanInput.use.GetAction1() && PacmanPickups.use.GetPickupAmount("Dynamite") >= 1)
		{

			PacmanPickups.use.ModifyPickupAmount("Dynamite", -1);

			bool currentTileAlreadyCharged = false;
			foreach(PacmanTile ctile in chargedTiles)
			{
				if (ctile == player.currentTile)
				{
					currentTileAlreadyCharged = true;
					break;
				}
			}
			
			if (!currentTileAlreadyCharged)
			{
				GameObject bomb = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("DynamiteCharged"));
				bomb.transform.position = player.currentTile.GetWorldLocation().v3().zAdd(-5.0f);

				PacmanDynamiteCharged charge = bomb.GetComponent<PacmanDynamiteCharged>();
				charge.parentTile = player.currentTile;
				charge.IsCounting(true);

				player.currentTile.tileItems.Add(charge);

	 			chargedTiles.Add(player.currentTile);
			}
		}

		for (int i = chargedTiles.Count - 1; i >= 0; i--)
		{
			if (chargedTiles[i] == null)
			{
				chargedTiles.Remove(chargedTiles[i]);
			}
		}
				                                         

//		// update placed bombs
//		for (int i = 0; i < chargedTiles.Count; i++) 
//		{
//			ChargedTile ctile = chargedTiles[i];
//
//			ctile.UpdateCharge();
//
//			if (ctile.TimerDone())
//			{
//				// remove bomb icon
//				Destroy(ctile.bombItem);
//
//				// display explosion effect and remove it after a few minutes
//				ParticleSystem ps = (ParticleSystem) Instantiate(explosionEffect);
//				ps.transform.parent = PacmanLevelManager.use.effectsParent;
//				ps.transform.localPosition = ctile.attachedTile.location;
//				ps.Play();
//				Destroy(ps.gameObject, 5);
//
//				// clear tiles around
//				foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(ctile.attachedTile))
//				{
//					Destroy(tile.rendered);
//					tile.tileType = PacmanTile.TileType.Open;
//				}
//
//				chargedTiles.Remove(ctile);
//			}
//		}
	}
}


