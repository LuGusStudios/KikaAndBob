using UnityEngine;
using System.Collections;

public class FroggerLaneItemPlayerSlot : FroggerLaneItem {

	public KikaAndBobFrogger.CharacterType characterType = KikaAndBobFrogger.CharacterType.Kika;
	protected bool slotFilled = false;

	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		if (character.characterType == characterType)
		{
			slotFilled = true;
			CheckAllSlots();
		}
		else
		{
			Debug.Log("Wrong character type.");
		}
	}

	public bool GetSlotFilled()
	{
		return slotFilled;
	}

	protected void CheckAllSlots()
	{
		foreach (FroggerLaneItemPlayerSlot slot in (FroggerLaneItemPlayerSlot[])FindObjectsOfType(typeof(FroggerLaneItemPlayerSlot)))
		{
			if (!slot.GetSlotFilled())
			{
				Debug.Log("Not all slots filled.");
				return;
			}
		}

		Debug.Log("All slots filled.");

		FroggerGameManager.use.WinGame();
	}
}
