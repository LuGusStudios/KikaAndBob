using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerCameraController : LugusSingletonExisting<FroggerCameraController> {

	protected FroggerPlayer target = null;
	protected Vector3 cameraOffset = Vector3.zero;
	protected float levelLengthInPixels = Screen.height;
	protected float halfScreenHeight = 0;

	void Awake()
	{
		halfScreenHeight = Vector3.Distance(
			LugusCamera.game.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)),
			LugusCamera.game.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)));
	}

	public void FocusOn(FroggerPlayer targetPlayer)
	{
		if (targetPlayer == null)
		{
			Debug.LogError("Player was null.");
			return;
		}

		target = targetPlayer;

		List<FroggerLane> lanes = FroggerLaneManager.use.GetLanes();
		Bounds firstSpriteBounds = lanes[0].GetComponent<SpriteRenderer>().sprite.bounds;
		Bounds lastSpriteBounds = lanes[lanes.Count - 1].GetComponent<SpriteRenderer>().sprite.bounds;

		// NOTE: Bounds of a Sprite (not SpriteRenderer) are in local coordinates, despite what Unity docs claim
		levelLengthInPixels = 
			FroggerLaneManager.use.GetLevelLengthLanePixels();

//			LugusCamera.game.WorldToScreenPoint(lanes[lanes.Count - 1].transform.position + lastSpriteBounds.max).y	-
//			LugusCamera.game.WorldToScreenPoint(lanes[0].transform.position + firstSpriteBounds.min).y;

		CalculateOffset();
	}

	protected void CalculateOffset()
	{
		cameraOffset = transform.position - target.transform.position;
	}

	// called from player movement script to prevent jitter due to incorrect ordering of update loops
	// we send along the FroggerPlayer to check if it is in fact the right player we want to track
	public void UpdateCameraFollow (FroggerPlayer sender)
	{
		if (target == null || sender != target || levelLengthInPixels <= Screen.height)
			return;

		float yPos = sender.transform.position.y;

		yPos = Mathf.Clamp(yPos, 
		                  FroggerLaneManager.use.GetBottomLaneBottomPixel() + halfScreenHeight,
		                  FroggerLaneManager.use.GetTopLaneTopPixel() - halfScreenHeight);

		transform.position = transform.position.y(yPos);
	}

}
