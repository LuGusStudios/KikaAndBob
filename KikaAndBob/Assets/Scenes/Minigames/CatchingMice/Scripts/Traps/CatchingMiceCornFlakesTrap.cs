using UnityEngine;
using System.Collections;

public class CatchingMiceCornFlakesTrap : CatchingMiceWorldObjectTrapFurniture {

	public override void SetupGlobal()
	{
		base.SetupGlobal();
		StartCoroutine(TrapRoutine());
	}
}
