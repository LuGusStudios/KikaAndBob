using UnityEngine;
using System.Collections;

public class FroggerLaneItem : FroggerSurface
{
	public bool behindPlayer = false;			// if true, item is placed behind player (e.g. "logs", platforms etc.)
												// if false, item ends up before player (e.g. "cars", obstacles)

	void Update()
	{
//		if (goRight)
//			transform.Translate(transform.right.normalized * speed * Time.deltaTime);
//		else
//			transform.Translate(-1 * transform.right.normalized * speed * Time.deltaTime);
	}

}
