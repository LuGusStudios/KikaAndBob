using UnityEngine;
using System.Collections;

public class FroggerCameraController : LugusSingletonExisting<FroggerCameraController> {

	protected FroggerPlayer target = null;
	protected Vector3 cameraOffset = Vector3.zero;

	public void FocusOn(FroggerPlayer targetPlayer)
	{
		if (targetPlayer == null)
		{
			Debug.LogError("Player was null.");
			return;
		}

		target = targetPlayer;

		CalculateOffset();
	}

	protected void CalculateOffset()
	{
		cameraOffset = transform.position - target.transform.position;
	}

	protected void Update()
	{
		if (target == null)
			return;


		transform.position =  new Vector3(transform.position.x, target.transform.position.y + cameraOffset.y, transform.position.z);
	}

}
