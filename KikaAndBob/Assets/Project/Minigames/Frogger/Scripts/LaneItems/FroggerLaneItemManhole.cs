using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemManhole : FroggerLaneItemLethal 
{
	protected ParticleSystem poof = null;

	public override void SetUpLocal()
	{
		base.SetUpLocal();
		poof = transform.FindChild("Poof").GetComponent<ParticleSystem>();

		if (poof == null)
		{
			Debug.LogError(name + ": Missing poof particle system.");
		}
	}

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		character.ShowCharacter(false);
		poof.Play();
		FroggerGameManager.use.LoseGame();	
	}
}
