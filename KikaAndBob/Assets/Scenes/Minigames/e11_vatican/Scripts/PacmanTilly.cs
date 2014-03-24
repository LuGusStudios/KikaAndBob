using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class PacmanTilly :  PacmanTileItem
{
	protected BoneAnimation tilly = null;
	protected Vector3 originalLocation = Vector3.zero;
	protected bool destroyed = false;
	protected bool found = false;

	public override void SetupLocal ()
	{
		base.SetupLocal();
		if (tilly == null)
		{
			tilly = GetComponentInChildren<BoneAnimation>();
		}
		if (tilly == null)
		{
			Debug.LogError("PacmanTilly: Missing bone animation!");
		}

	}
	

    public override void OnEnter(PacmanCharacter character)
	{
		found = true;
		PacmanGameManager.use.WinGame();
		TillyFly();
	}

	public override void DestroyTileItem ()
	{
		if (destroyed || found)
			return;

		destroyed = true;

		PacmanGameManager.use.LoseLife();
		TillyFly();
	}

	public override void Reset ()
	{
		destroyed = false;
		iTween.Stop(gameObject);
		transform.position = parentTile.GetWorldLocation().v3() + new Vector3(0, 0, -5.0f);
	}

	protected void TillyFly()
	{
		LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("DoveFly01"));


		tilly.Play("Tilly_Flying");

		// Tilly flies to topright corner of the map, with z distance remaining the same
		Vector3 targetLocation = 
			PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width - 1, PacmanLevelManager.use.height - 1).GetWorldLocation().v3().z(transform.position.z);

		// add a little extra
		targetLocation += new Vector3(10.0f, 10.0f, 0);
	
		
		gameObject.MoveTo(targetLocation).Speed(5.0f).IgnoreTimeScale(true).Execute();
	}
}
