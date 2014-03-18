using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItemLavaHole : PacmanTileItem 
{
	bool routineRunning = false;

	public override void OnEnter (PacmanCharacter character)
	{
		if (!routineRunning)
			LugusCoroutines.use.StartRoutine(BurnUp());
	}


	protected IEnumerator BurnUp()
	{
		PacmanGameManager.use.gameRunning = false;
		
		routineRunning = true;
		
		GameObject flameObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("FlameAnimation"));
		Vector3 playerPos = PacmanGameManager.use.GetActivePlayer().transform.position.zAdd(-1f);
		flameObject.transform.position = playerPos;
		flameObject.transform.parent = PacmanLevelManager.use.temporaryParent;
		
		PacmanGameManager.use.GetActivePlayer().HideCharacter();
		
		yield return new WaitForSeconds(0.5f);
		
		GameObject ashObject = (GameObject) Instantiate(PacmanLevelManager.use.GetPrefab("AshPile"));
		ashObject.transform.position = playerPos;
		ashObject.transform.parent = PacmanLevelManager.use.temporaryParent;
		
		yield return new WaitForSeconds(1.5f);


		PacmanGameManager.use.LoseLife();

		routineRunning = false;
	}
}
