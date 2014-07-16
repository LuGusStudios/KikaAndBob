using UnityEngine;
using System.Collections;

public class CatchingMiceMouseTrap : CatchingMiceWorldObjectTrapGround {

	public override void SetupGlobal()
	{
		base.SetupGlobal();
		StartCoroutine(TrapRoutine());
	}
}
