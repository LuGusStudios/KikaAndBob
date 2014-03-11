using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItemKey : PacmanTileItem 
{
	public string keyID = "Key01";
	public int amount = 1;
	protected bool pickedUp = false;

	public override void Initialize ()
	{
		PacmanPickups.use.RegisterPickup(keyID);
	}

	public override void OnEnter()
	{
		if (pickedUp)
			return;

		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("Key01"));

		pickedUp = true;

		this.gameObject.SetActive(false);

		PacmanPickups.use.ModifyPickupAmount(keyID, amount);
	}
}
