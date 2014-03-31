using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanDonkey : PacmanTileItem 
{
	protected Transform key = null;
	protected bool found = false;

	public override void SetupLocal ()
	{
		if (key == null)
			key = transform.FindChild("Key");

		if (key == null)
			Debug.LogError("PacmanDonkey: Missing key!");
	}

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
	}

	protected IEnumerator WinRoutine()
	{
		PacmanGameManager.use.gameRunning = false;	// already turn this off so lava won't flow etc.

	
		key.gameObject.SetActive(true);
		Vector3 screenCenter = LugusCamera.game.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)).z(-200);

		key.gameObject.MoveTo(screenCenter).EaseType(iTween.EaseType.easeOutBack).Time(1.0f).Execute();
		key.gameObject.ScaleTo(key.transform.localScale * 3.0f).Time(1.0f).Execute();

		yield return new WaitForSeconds(1.0f);

		key.gameObject.ScaleTo(Vector3.zero).Time(0.25f).Execute();

		yield return new WaitForSeconds(0.25f);

		PacmanGameManager.use.WinGame();
	}
}
