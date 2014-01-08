﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItemKey : PacmanTileItem 
{
	public string keyID = "Key01";
	public int amount = 1;
	protected bool pickedUp = false;

	protected void Start()
	{
		PacmanPickups.use.RegisterPickup(keyID);
	}

	public override void OnEnter()
	{
		if (pickedUp)
			return;

		pickedUp = true;

		this.gameObject.SetActive(false);

		PacmanPickups.use.ModifyPickups(keyID, amount);
	}
}
