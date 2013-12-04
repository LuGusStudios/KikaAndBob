using UnityEngine;
using System.Collections;

public class FroggerLaneItem : FroggerSurface
{
	public float speed = 2;
	public bool goRight = true;

	void Update()
	{
		if (goRight)
			transform.Translate(transform.right.normalized * speed * Time.deltaTime);
		else
			transform.Translate(-1 * transform.right.normalized * speed * Time.deltaTime);
	}

}
