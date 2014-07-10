﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCameraMover : MonoBehaviour 
{
	public float scrollScreenArea = 0.1f;
	public float maxSpeed = 1.0f;
	public float swipeMomentumDecay = 0.9f;
	protected Vector2 levelBoundsMin = Vector2.zero;
	protected Vector2 levelBoundsMax = Vector2.zero;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		UpdateLevelBounds();
		CatchingMiceLevelManager.use.OnLevelBuilt += UpdateLevelBounds;
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
		CheckScrollingTouch();
		CheckScrollingArrows();

		ClampPosition();
	}

	protected void CheckScrollingArrows()
	{
		Vector3 movement = Vector3.zero;

		if (LugusInput.use.Key(KeyCode.LeftArrow))
		{
			movement = movement.xAdd(-maxSpeed);
		}
		else if (LugusInput.use.Key(KeyCode.RightArrow))
		{
			movement = movement.xAdd(maxSpeed);
		}

		if (LugusInput.use.Key(KeyCode.DownArrow))
		{
			movement = movement.yAdd(-maxSpeed);
		}
		else if (LugusInput.use.Key(KeyCode.UpArrow))
		{
			movement = movement.yAdd(maxSpeed);
		}

		
		if (movement != Vector3.zero)
			transform.Translate(movement);
	}

	protected Vector3 dragStartPoint = Vector3.zero;
	protected Vector3 dragPoint = Vector3.zero;
	protected float dragTimer = 0;
	protected float maxSwipeTime = 0.25f;
	protected float maxSwipeDistance = 500;

	protected Vector3 swipeMomentum = Vector3.zero;
	protected bool validSwipe = false;

	protected void CheckScrollingTouch()
	{
		if (LugusInput.use.down && !CatchingMiceTrapSelector.use.dragging)
		{
			validSwipe = false;

			Transform hit = LugusInput.use.RayCastFromMouseDown(LugusCamera.game);

			if (hit == null)
			{
				hit = LugusInput.use.RayCastFromMouseDown(LugusCamera.ui);
				
				if (hit == null)
				{
					validSwipe = true;
				}
			}

			dragPoint = Vector3.zero;
			dragTimer = 0;
			swipeMomentum = Vector3.zero;
	
			if (validSwipe)
			{
				dragStartPoint = LugusCamera.game.ScreenToWorldPoint(LugusInput.use.lastPoint);
			}

		}
		else if (LugusInput.use.dragging && validSwipe && !CatchingMiceTrapSelector.use.dragging)
		{
			dragPoint = LugusCamera.game.ScreenToWorldPoint(LugusInput.use.lastPoint);
			Vector3 offset = dragStartPoint - dragPoint;
			transform.Translate(offset.z(0));

			dragTimer += Time.deltaTime;
		}
		else if (LugusInput.use.up && validSwipe && !CatchingMiceTrapSelector.use.dragging)
		{
			swipeMomentum = Vector3.zero;
			if (dragTimer <= maxSwipeTime)
			{
				float dragDistance = 0;
				Vector3 direction = LugusCamera.game.ScreenToWorldPoint(LugusInput.use.lastPoint) - dragPoint;

				for (int i = 0; i < LugusInput.use.inputPoints.Count; i++) 
				{
					if (i == 0)
						continue;

					dragDistance += Vector3.Distance(LugusInput.use.inputPoints[i-1], LugusInput.use.inputPoints[i]);
				}

				swipeMomentum = direction.normalized * (dragDistance / maxSwipeDistance);
			}

			validSwipe = false;
		}

//		if (swipeMomentum != Vector3.zero && !LugusInput.use.dragging)
//		{
//			transform.position += swipeMomentum;
//			swipeMomentum *= swipeMomentumDecay;
//
//			if (swipeMomentum.magnitude < 0.05f)
//			{
//				swipeMomentum = Vector3.zero; 
//			}
//		}



//		if ( !LugusInput.use.down && !LugusInput.use.dragging )
//		{
//			return;	
//		}
//
//		Vector3 lastpoint = LugusInput.use.lastPoint;	// cached purely for readability here
//		Vector3 movement = Vector3.zero;
//		float movementScale = 1;
//
//		if (lastpoint.x < Screen.width * scrollScreenArea)
//		{
//			movementScale = 1 - (lastpoint.x / Screen.width * scrollScreenArea);
//
//			movement = movement.xAdd( (transform.right.x * -1) * maxSpeed * movementScale );
//		}
//		else if (lastpoint.x > Screen.width - (Screen.width * scrollScreenArea))
//		{
//			//movementScale = (lastpoint.x - (Screen.width * ( 1 - scrollScreenArea))) / ( Screen.width * scrollScreenArea);
//
//			movementScale = 1 - ( (Screen.width - lastpoint.x) / (Screen.width * scrollScreenArea) );
//
//			movement = movement.xAdd( transform.right.x * maxSpeed * movementScale );
//		}
//
//		if (lastpoint.y < Screen.height * scrollScreenArea)
//		{
//			movementScale = 1 - (lastpoint.y / Screen.height * scrollScreenArea);
//
//			movement = movement.yAdd( (transform.up.y * -1) * maxSpeed * movementScale );
//		}
//		else if (lastpoint.y > Screen.height - (Screen.height * scrollScreenArea))
//		{
//			movementScale = 1 - ( (Screen.height - lastpoint.y) / (Screen.height * scrollScreenArea) );
//
//			movement = movement.yAdd( transform.up.y * maxSpeed * movementScale );
//		}
//	
//		if (movement != Vector3.zero)
//			transform.Translate(movement);
	}

	protected void ClampPosition()
	{
		if (transform.position.x < levelBoundsMin.x || transform.position.y < levelBoundsMin.y 
		    || transform.position.x > levelBoundsMax.x || transform.position.y > levelBoundsMax.y)
		{
			swipeMomentum = Vector3.zero;
		}

		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x, levelBoundsMin.x, levelBoundsMax.x),
			Mathf.Clamp(transform.position.y, levelBoundsMin.y, levelBoundsMax.y),
			transform.position.z);
	}

	protected void UpdateLevelBounds()
	{
		if ( CatchingMiceLevelManager.use.Tiles == null)
			return;

		levelBoundsMin = CatchingMiceLevelManager.use.Tiles[0, 0].location.v2();

		levelBoundsMax = CatchingMiceLevelManager.use.Tiles[CatchingMiceLevelManager.use.Tiles.GetLength(0) - 1, CatchingMiceLevelManager.use.Tiles.GetLength(1) - 1].location.v2();
	}
}
