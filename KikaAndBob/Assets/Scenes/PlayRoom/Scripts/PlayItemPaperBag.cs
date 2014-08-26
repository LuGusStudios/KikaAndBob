using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayItemPaperBag : IPlayItem 
{
	protected ILugusCoroutineHandle pounceRoutine = null;
	
	public override void Activate (PlayCatController cat)
	{
		Debug.Log("PlayItemTest: Activated " + this.name);
		
		iTween.Stop(cat.gameObject);

		transform.localScale = Vector3.one;
		
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

		List<Vector3> path = new List<Vector3>();
		path.Add(cat.transform.position);
		path.Add(Vector3.Lerp(cat.transform.position, this.transform.position.z(actionPoint.position.z), 0.5f) + new Vector3(0, 1, 0));
		path.Add(this.transform.position.z(actionPoint.position.z));
		
		cat.gameObject.MoveTo(path.ToArray()).Time(0.4f).Execute();
		
		yield return new WaitForSeconds(0.15f);

		gameObject.ScaleTo(new Vector3(1, 0.2f, 1)).Time(0.2f).Execute();

		yield return new WaitForSeconds(0.5f);
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

		iTween.Stop(this.gameObject);

		transform.localScale = Vector3.one;
	}
}
