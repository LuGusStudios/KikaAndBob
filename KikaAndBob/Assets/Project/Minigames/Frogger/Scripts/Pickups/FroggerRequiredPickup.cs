using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerRequiredPickup : FroggerLaneItem 
{
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

		ScoreVisualizer.Score(KikaAndBob.CommodityType.Custom, 1).Position(this.transform.position).Execute();
	}
}
