using UnityEngine;
using System.Collections;

public class FroggerLaneWater : FroggerLane 
{
	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		FroggerGameManager.use.LoseGame();
	}
}
