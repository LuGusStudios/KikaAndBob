using UnityEngine;
using System.Collections;

public class CatchingMiceCasseroleTrap : CatchingMiceWorldObjectTrapGround {

	public override void SetupGlobal()
	{
		base.SetupGlobal();
		StartCoroutine(TrapRoutine());
	}
}
