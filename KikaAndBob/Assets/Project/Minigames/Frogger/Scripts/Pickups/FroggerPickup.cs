using UnityEngine;
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

		FroggerGameManager.use.ModifyPickUpCount(1);
		ScoreVisualizer.Score(KikaAndBob.CommodityType.Feather, 1).Position(this.transform.position).Execute();
	}

	public bool GetPickedUp()
	{
		return pickedUp;
	}
}
