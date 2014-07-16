using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceBearTrap : CatchingMiceWorldObjectTrapGround {

	public override void SetupGlobal()
	{
		base.SetupGlobal();
		StartCoroutine(TrapRoutine());
	}
}
