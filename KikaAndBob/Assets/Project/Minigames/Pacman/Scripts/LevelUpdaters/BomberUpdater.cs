using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BomberUpdater : PacmanLevelUpdater {

	public float bombTime = 5;
	protected ParticleSystem explosionEffect = null;
	protected GameObject bombPrefab = null;
	protected bool running = false;
	protected List<ChargedTile> chargedTiles = new List<ChargedTile>();
	protected PacmanPlayerCharacter player = null;

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

		public bool GetDone()
		{
			return timer >= duration;
		}
	}

	public override void Activate()
	{
		player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));
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

		// place bombs
		if (PacmanInput.use.GetAction1())
		{
			bool currentTileAlreadyCharged = false;
			foreach(ChargedTile ctile in chargedTiles)
			{
				if (ctile.attachedTile == player.currentTile)
				{
					currentTileAlreadyCharged = true;
					break;
				}
			}
			
			if (!currentTileAlreadyCharged)
			{
				GameObject bomb = (GameObject) Instantiate(bombPrefab);
				bomb.transform.parent = PacmanLevelManager.use.effectsParent;
				bomb.transform.localPosition = player.currentTile.location;

				chargedTiles.Add(new ChargedTile(player.currentTile, bombTime, bomb));
			}
		}

		// update placed bombs
		for (int i = 0; i < chargedTiles.Count; i++) 
		{
			ChargedTile ctile = chargedTiles[i];

			ctile.UpdateCharge();

			if (ctile.GetDone())
			{
				// remove bomb icon
				Destroy(ctile.bombItem);

				// display explosion effect and remove it after a few minutes
				ParticleSystem ps = (ParticleSystem) Instantiate(explosionEffect);
				ps.transform.parent = PacmanLevelManager.use.effectsParent;
				ps.transform.localPosition = ctile.attachedTile.location;
				ps.Play();
				Destroy(ps.gameObject, 5);

				// clear tiles around
				foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(ctile.attachedTile))
				{
					Destroy(tile.rendered);
					tile.tileType = PacmanTile.TileType.Open;
				}

				chargedTiles.Remove(ctile);
			}
		}
	}
}


