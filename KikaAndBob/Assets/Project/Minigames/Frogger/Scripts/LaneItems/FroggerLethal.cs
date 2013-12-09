using UnityEngine;
using System.Collections;

public class FroggerLethal : FroggerLaneItem 
{

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		FroggerGameManager.use.LoseGame();	
	}
}
