﻿using UnityEngine;
using System.Collections;

public class FroggerLog : FroggerLaneItem 
{
	public override void SetUpLocal ()
	{
		base.SetUpLocal ();
	}

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

	protected override void AfterMovedEffect ()
	{
//		if (rising)
//		{
//			transform.localPosition = transform.localPosition.y(transform.localPosition.y + speedY * Time.deltaTime);
//
//			if (transform.localPosition.y >= originalY + yMovement)
//			{
//				rising = false;
//				print("switching to false");
//			}
//		}
//		else
//		{
//			transform.localPosition = transform.localPosition.y(transform.localPosition.y - speedY * Time.deltaTime);
//			
//			if (transform.localPosition.y <= originalY)
//			{
//				rising = true;
//				print("switching to true");
//			}
//		}

		//transform.localPosition = transform.localPosition.y(transform.localPosition.y + yMovement * Time.deltaTime * Mathf.Sin(speedY * Time.time));
	}
}
