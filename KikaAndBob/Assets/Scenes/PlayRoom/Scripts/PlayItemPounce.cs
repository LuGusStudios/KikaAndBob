using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayItemPounce : IPlayItem 
{
	protected ILugusCoroutineHandle pounceRoutine = null;

	public override void Activate (PlayCatController cat)
	{
		Debug.Log("PlayItemTest: Activated " + this.name);

		iTween.Stop(cat.gameObject);

		if (pounceRoutine != null && pounceRoutine.Running)
		{
			pounceRoutine.StopRoutine();
		}

		pounceRoutine = LugusCoroutines.use.StartRoutine(PounceRoutine(cat));
	}

	protected IEnumerator PounceRoutine(PlayCatController cat)
	{	
		cat.catAnimation.PlayAnimation("LEFT/@Side_Pounce");

		yield return new WaitForSeconds(0.3f);

		cat.gameObject.MoveTo(this.transform.position.z(actionPoint.position.z)).Time(0.4f).Execute();

		yield return new WaitForSeconds(1f);

		cat.catAnimation.PlayAnimation("LEFT/@Side_Idle", 1f);

		yield break;
	}

	public override void Deactivate (PlayCatController cat)
	{
		if (pounceRoutine != null && pounceRoutine.Running)
		{
			pounceRoutine.StopRoutine();
		}

		iTween.Stop(cat.gameObject);
	}
}
