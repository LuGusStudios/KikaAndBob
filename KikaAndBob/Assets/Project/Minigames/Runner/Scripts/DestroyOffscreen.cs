using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyOffscreen : MonoBehaviour 
{
	protected void OnBecameInvisible() 
	{
		//Debug.LogError("Object became invisible!");
		GameObject.Destroy( this.gameObject );
	}
}
