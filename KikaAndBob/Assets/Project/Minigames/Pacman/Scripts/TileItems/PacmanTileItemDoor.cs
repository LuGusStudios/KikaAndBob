﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItemDoor : PacmanTileItem 
{
	public string keyID = "Key01";
	protected bool opened = false;

	public override void Initialize ()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}

	public override void OnTryEnter (PacmanCharacter character)
	{
		if (!opened && PacmanPickups.use.GetPickupAmount(keyID) >= 1 && character is PacmanPlayerCharacter)
		{
			opened = true;
			PacmanPickups.use.ModifyPickupAmount(keyID, -1);
			parentTile.tileType = PacmanTile.TileType.Open;
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("DoorCreak01"));
			this.gameObject.SetActive(false);
		}
	}

}
