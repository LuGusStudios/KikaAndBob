﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerCameraController : LugusSingletonExisting<FroggerCameraController> {

	protected FroggerPlayer target = null;
	protected Vector3 cameraOffset = Vector3.zero;
	protected float levelLengthInPixels = Screen.height;

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
			LugusCamera.game.WorldToScreenPoint(lanes[lanes.Count - 1].transform.position + lastSpriteBounds.max).y	-
			LugusCamera.game.WorldToScreenPoint(lanes[0].transform.position + firstSpriteBounds.min).y;

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

		transform.position = new Vector3(transform.position.x, target.transform.position.y + cameraOffset.y, transform.position.z);
	}

}
