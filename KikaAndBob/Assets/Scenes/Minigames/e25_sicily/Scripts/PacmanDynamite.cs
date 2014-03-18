using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanDynamite : PacmanTileItem 
{
	public string id = "Dynamite";
	protected bool pickedUp = false;

	public override void Initialize ()
	{
		PacmanPickups.use.RegisterPickup(id);
	}

	public override void OnEnter (PacmanCharacter character)
	{
		if (pickedUp)
			return;
		
		//LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("Key01"));
		
		pickedUp = true;
		
		this.gameObject.SetActive(false);
		
		PacmanPickups.use.ModifyPickupAmount(id, 1);
	}
}
