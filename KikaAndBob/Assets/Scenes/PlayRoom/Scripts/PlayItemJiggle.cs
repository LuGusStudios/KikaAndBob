using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayItemJiggle : IPlayItem 
{
	public override void Activate (PlayCatController cat)
	{
		Debug.Log("PlayItemTest: Activated " + this.name);
		
		cat.catAnimation.PlayAnimation(catAnimationPath);
		
		iTween.Stop(gameObject);
		transform.localEulerAngles = Vector3.zero;
		iTween.PunchRotation(gameObject, iTween.Hash(
			"time", 2,
			"amount", new Vector3(0, 0, 10),
			"looptype", iTween.LoopType.loop));
	}
	
	public override void Deactivate (PlayCatController cat)
	{
		iTween.Stop(gameObject);
		transform.localEulerAngles = Vector3.zero;
	}
}
