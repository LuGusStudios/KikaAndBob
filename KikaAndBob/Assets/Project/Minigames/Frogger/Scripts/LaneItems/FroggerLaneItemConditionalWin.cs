using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemConditionalWin : FroggerLaneItem 
{
	public Sprite pickupIcon = null;

	protected List<FroggerRequiredPickup> requiredPickups = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// Find all of the required pickups in the scene
		requiredPickups = new List<FroggerRequiredPickup>(GameObject.FindObjectsOfType<FroggerRequiredPickup>());
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	// TODO: Some form of animation?
	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		bool gameWon = true;
		foreach (FroggerRequiredPickup pickUp in requiredPickups)
		{
			if (!pickUp.PickedUp)
			{
				gameWon = false;
				break;
			}
		}

		if (gameWon)
		{
			character.characterAnimator.PlayAnimation(character.characterAnimator.idleUp);

			FroggerGameManager.use.WinGame();
		}
	}
}
