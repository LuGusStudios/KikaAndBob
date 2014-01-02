using UnityEngine;
using System.Collections;

public class FroggerLaneWin : FroggerLane {

	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		FroggerGameManager.use.WinGame();
		Leave(character);
	}
}
