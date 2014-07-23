using UnityEngine;
using System.Collections;

public class CatchingMiceCasseroleTrap : CatchingMiceWorldObjectTrapGround {

	protected ILugusCoroutineHandle closeRoutine = null;

	public override void SetupGlobal()
	{
		base.SetupGlobal();
		StartCoroutine(TrapRoutine());
	}

	protected override void AttackEffect ()
	{
		if (closeRoutine != null && closeRoutine.Running)
		{
			closeRoutine.StopRoutine();
			spriteRenderer.sprite = activeSprite;
		}
		
		PlayAttackSound();
		
		closeRoutine = LugusCoroutines.use.StartRoutine(CloseRoutine());
	}
	
	protected IEnumerator CloseRoutine()
	{
		spriteRenderer.sprite = inactiveSprite;
		
		yield return new WaitForSeconds(Interval * 0.4f);
		
		// if the trap is used up, don't reset the sprite anymore
		// the -1 check normally shouldn't be necessary, since the ammo count is immediately decreased, but just in case that ever changes, this should remain functional
		if (ammo > 0)
		{
			spriteRenderer.sprite = activeSprite;
		}
		
		yield break;
	}

}
