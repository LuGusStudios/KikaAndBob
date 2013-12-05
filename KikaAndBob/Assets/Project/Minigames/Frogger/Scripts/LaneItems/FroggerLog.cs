using UnityEngine;
using System.Collections;

public class FroggerLog : FroggerLaneItem 
{

	protected Transform originalParent = null;

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		//Debug.Log("Character: " + character.name + " entered item: " + this.gameObject.name);
		originalParent = character.transform.parent;
		character.transform.parent = this.transform;
	}

	protected override void LeaveSurfaceEffect(FroggerCharacter character)
	{
		//Debug.Log("Character: " + character.name + " left item: " + this.gameObject.name);	
		character.transform.parent = originalParent;
	}
}
