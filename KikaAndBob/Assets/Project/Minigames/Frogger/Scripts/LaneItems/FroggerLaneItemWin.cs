using UnityEngine;
using System.Collections;
using SmoothMoves;

public class FroggerLaneItemWin : FroggerLaneItem {
	
	protected bool slotFilled = false;
	protected BoneAnimation tilly = null;

	public override void SetUpLocal ()
	{
		base.SetUpLocal ();

		if (tilly == null)
		{
			tilly = GetComponentInChildren<BoneAnimation>();
		}
		if (tilly == null)
		{
			Debug.LogError("FroggerLaneItemWin: Missing bone animation!");
		}
	}

	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		TillyFly(character);
	}

	protected void TillyFly(FroggerCharacter character)
	{
		LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("DoveFly01"));

		FroggerGameManager.use.gameRunning = false;
		
		if (character is FroggerPlayer) // currently this will never not be the case, but for future compatibility it's not a bad idea to check anyway
		{
			character.characterAnimator.PlayAnimation(character.characterAnimator.idleUp);
		}
		
		tilly.Play("Tilly_Flying");
		
		Vector3[] path = new Vector3[]
		{
			transform.position,
			new Vector3(transform.position.x + 13, FroggerLaneManager.use.levelTopY + 2, transform.position.z)
		};
		
		gameObject.MoveTo(path).Time(1.0f).IgnoreTimeScale(true).Execute();
		
		FroggerGameManager.use.WinGame();
	}

}
