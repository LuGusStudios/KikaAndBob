using UnityEngine;
using System.Collections;

public class CatchingMiceMouseTrap : CatchingMiceWorldObjectTrapGround {

	protected ILugusCoroutineHandle snapRoutine = null;
	protected Vector3 originalPosition = Vector3.zero;

	public override void SetupGlobal()
	{
		originalPosition = transform.position;

		base.SetupGlobal();
		StartCoroutine(TrapRoutine());
	}

	protected override void AttackEffect ()
	{
		if (snapRoutine != null && snapRoutine.Running)
		{
			snapRoutine.StopRoutine();
			spriteRenderer.sprite = activeSprite;
		}
		
		PlayAttackSound();
		
		snapRoutine = LugusCoroutines.use.StartRoutine(SnapRoutine());
	}
	
	protected IEnumerator SnapRoutine()
	{
		iTween.Stop(gameObject);
		
		transform.position = originalPosition;
		
		gameObject.MoveTo(transform.position.yAdd(0.1f)).Time(0.05f).Execute();
		
		spriteRenderer.sprite = inactiveSprite;
		
		yield return new WaitForSeconds(Interval * 0.1f);
		
		gameObject.MoveTo(transform.position.yAdd(-0.1f)).Time(0.05f).Execute();
		
		// if the trap is used up, don't reset the sprite anymore
		// the -1 check normally shouldn't be necessary, since the ammo count is immediately decreased, but just in case that ever changes, this should remain functional
		if (ammo > 0)
		{
			spriteRenderer.sprite = activeSprite;
		}
		
		yield break;
	}
}
