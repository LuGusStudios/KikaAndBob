using UnityEngine;
using System.Collections;

public class FroggerLaneItemLethal : FroggerLaneItem 
{

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		character.Blink(Color.red, 1.0f, 3);
		FroggerGameManager.use.LoseGame();
		character.DoHitAnimation();
	}
}
