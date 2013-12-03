using UnityEngine;
using System.Collections;

public class FroggerLaneTest : FroggerLane 
{
	public override void EnterSurfaceEffect ()
	{
		Debug.Log("on layer");
	}

}
