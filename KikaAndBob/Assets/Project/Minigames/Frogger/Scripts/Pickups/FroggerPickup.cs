﻿using UnityEngine;
using System.Collections;

public class FroggerPickup : FroggerLaneItem 
{
	protected bool pickedUp = false;

	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		pickedUp = true;

		GetComponent<BoxCollider2D>().enabled = false;

		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.enabled = false;	
		}

		bool foundAll = true;

		print( FindObjectsOfType(typeof(FroggerPickup)).Length);

		foreach(FroggerPickup p in (FroggerPickup[]) FindObjectsOfType(typeof(FroggerPickup)))
		{
			if (p.GetPickedUp() == false)
			{
				foundAll = false;
				break;
			}
		}

		if (foundAll)
		{
			Debug.Log("All pickups found.");
			FroggerGameManager.use.WinGame();
		}
		else
		{
			Debug.Log("Not all pickups found.");
		}
	}

	public bool GetPickedUp()
	{
		return pickedUp;
	}
}
