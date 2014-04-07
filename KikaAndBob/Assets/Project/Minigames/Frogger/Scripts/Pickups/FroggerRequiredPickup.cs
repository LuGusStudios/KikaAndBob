using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerRequiredPickup : FroggerLaneItem 
{

	public KikaAndBob.CommodityType pickupType = KikaAndBob.CommodityType.NONE;

	public bool PickedUp
	{
		get
		{
			return pickedUp;
		}
	}

	protected bool pickedUp = false;

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		pickedUp = true;

		GetComponent<BoxCollider2D>().enabled = false;

		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.enabled = false;
		}

		if (pickupType != KikaAndBob.CommodityType.NONE)
		{
			ScoreVisualizer.Score(pickupType, 1).Position(this.transform.position).Execute();
		}
	}
}
