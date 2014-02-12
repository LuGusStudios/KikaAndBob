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
		StartCoroutine(TillyFly(character));
	}

	protected IEnumerator TillyFly(FroggerCharacter character)
	{
		FroggerGameManager.use.gameRunning = false;

		if (character is FroggerPlayer) // currently this will never not be the case, but for future compatibility it's not a bad idea to check anyway
		{
			character.characterAnimator.PlayAnimation(character.characterAnimator.idleUp);
		}

		tilly.Play("Tilly_Flying");

		Vector3[] path = new Vector3[]
		{
			transform.position,
			transform.position + new Vector3(5, 2, 0),
			transform.position + new Vector3(13, 5, 0)
		};

		gameObject.MoveTo(path).Time(1.0f).IgnoreTimeScale(true).Execute();

		yield return new WaitForSeconds(1.0f);

		FroggerGameManager.use.WinGame();


		yield break;
	}
	
}
