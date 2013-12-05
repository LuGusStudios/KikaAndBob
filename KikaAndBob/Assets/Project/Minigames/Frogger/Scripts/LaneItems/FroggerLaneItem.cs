using UnityEngine;
using System.Collections;

public class FroggerLaneItem : FroggerSurface
{
	public bool goRight = true;

	void Update()
	{
//		if (goRight)
//			transform.Translate(transform.right.normalized * speed * Time.deltaTime);
//		else
//			transform.Translate(-1 * transform.right.normalized * speed * Time.deltaTime);
	}

}
