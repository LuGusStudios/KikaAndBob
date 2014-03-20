using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanDynamite : PacmanTileItem 
{
	public string id = "Dynamite";
	protected bool pickedUp = false;
	protected bool destroyed = false;

	public override void Initialize ()
	{
		PacmanPickups.use.RegisterPickup(id);
	}

	public override void OnEnter (PacmanCharacter character)
	{
		if (pickedUp || destroyed)
			return;
		
		pickedUp = true;
		
		this.gameObject.SetActive(false);
		
		PacmanPickups.use.ModifyPickupAmount(id, 1);
	}

	public override void DestroyTileItem ()
	{
		if (pickedUp || destroyed)
			return;

		if (parentTile == null)
		{
			Debug.LogError("PacmanDynamite: Parent tile was null");
			return;
		}

		destroyed = true;

		// we switch out this script for a 'charged' one
		PacmanDynamiteCharged chargedVersion = this.gameObject.AddComponent<PacmanDynamiteCharged>();
		chargedVersion.parentTile = this.parentTile;
		chargedVersion.IsCounting(true);
	
		// switch tile item scripts out
		parentTile.tileItems.Add(chargedVersion);
		parentTile.tileItems.Remove(this);

		Destroy(this);
	}


}
