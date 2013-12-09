using UnityEngine;
using System.Collections;

public class FroggerLaneItemLethal : FroggerLaneItem 
{

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		FroggerGameManager.use.LoseGame();	
	}
}
