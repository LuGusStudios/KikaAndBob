using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanBirdCage : PacmanTileItem 
{
	protected bool found = false;
	
	public override void Initialize ()
	{
		parentTile.tileType = PacmanTile.TileType.Collide;
	}
	
	public override void OnTryEnter (PacmanCharacter character)
	{
		if (found)
			return;
		
		found = true;

		LugusCoroutines.use.StartRoutine(WinRoutine());

		PacmanGameManager.use.WinGame();
	}

	protected IEnumerator WinRoutine()
	{
		Vector3 startPosition = transform.position;
		Vector3 startScale = transform.localScale;

		gameObject.MoveTo(startPosition + new Vector3(0, 0.25f, 0)).EaseType(iTween.EaseType.easeOutSine).Time(0.5f).Execute();
		gameObject.ScaleTo(startScale * 1.1f).EaseType(iTween.EaseType.easeOutSine).Time(0.5f).Execute();

		yield return new WaitForSeconds(0.5f);

		gameObject.MoveTo(startPosition).EaseType(iTween.EaseType.easeInSine).Time(0.5f).Execute();
		gameObject.ScaleTo(startScale).EaseType(iTween.EaseType.easeInSine).Time(0.5f).Execute();
	}
}
