using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroLaneItemRenderer : MonoBehaviour 
{
	public delegate void OnUpdateScore();
	public OnUpdateScore onUpdateScore;

	public static DanceHeroLaneItemRenderer Create(DanceHeroLaneItem item)
	{
		DanceHeroLane lane = item.lane;
		// TODO: make this more official or provide prefabs for ActionPoint visualizers
		GameObject actionIndicatorPrefab = lane.transform.FindChild("LaneItemPrefab").gameObject;

		GameObject container = new GameObject("Item");
		container.transform.parent = lane.transform;
		container.transform.position = lane.transform.position;

		DanceHeroLaneItemRenderer renderer = container.AddComponent<DanceHeroLaneItemRenderer>();
		renderer.item = item;
		item.laneItemRenderer = renderer;

		if( item.type == KikaAndBob.LaneItemType.SINGLE )
		{
			// single button : just one actionpoint
			
			GameObject actionPoint = (GameObject) GameObject.Instantiate( actionIndicatorPrefab );

			// we're directly instantiating the lane action point
			// don't forget to disable the highlight, because it might still have been active from the highlight effect
			actionPoint.transform.FindChild("Highlight").gameObject.SetActive(false); 

			actionPoint.name = "Point";
			actionPoint.transform.parent = container.transform;
			actionPoint.transform.position = container.transform.position + new Vector3(0, 0, -1);

			renderer.actionPoints.Add( actionPoint.transform );
		}
		else
		{
			// streak button : 2 actionpoints with a line in between

			// point 1
			GameObject actionPoint = (GameObject) GameObject.Instantiate( actionIndicatorPrefab );
			
			actionPoint.name = "Point1";
			actionPoint.transform.parent = container.transform;
			actionPoint.transform.position = container.transform.position;
			// we're directly instantiating the lane action point
			// don't forget to disable the highlight, because it might still have been active from the highlight effect
			actionPoint.transform.FindChild("Highlight").gameObject.SetActive(false); 
			
			renderer.actionPoints.Add( actionPoint.transform );

			// line in between
			// need to calculate how long this needs to be
			// we have the duration of the section in seconds + how many units it's going to move per SECOND (item.speed) : easy
			float units = item.duration * item.speed;

			// point 2
			actionPoint = (GameObject) GameObject.Instantiate( actionIndicatorPrefab );
			
			actionPoint.name = "Point2";
			actionPoint.transform.parent = container.transform;
			actionPoint.transform.position = container.transform.position + new Vector3(-1 * units, 0.0f, 0.0f);
			// we're directly instantiating the lane action point
			// don't forget to disable the highlight, because it might still have been active from the highlight effect
			actionPoint.transform.FindChild("Highlight").gameObject.SetActive(false); 


			// TODO: make the line function better... now it's just in the ActionPoint prefab, but often it goes unused, causing overhead...
			Transform line = actionPoint.transform.FindChild("Line");
			line.localScale = line.localScale.x ( units / line.renderer.bounds.size.x );
			//Debug.LogError ("Line width " + line.renderer.bounds.size.x );

			
			renderer.actionPoints.Add( actionPoint.transform );

		}

		return renderer;
	}

	public List<Transform> actionPoints = new List<Transform>();
	public int currentActionPointIndex = 0;

	protected bool hit = false;
	protected bool missed = false;
	protected float offScreenTime = 0;
	protected float offScreenRemoveTime = 10;
	protected ILugusCoroutineHandle streakRoutine = null;

	public DanceHeroLaneItem item = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		if( item == null )
		{
			Debug.LogError(name + " : LaneItem is null!");
		}
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
		transform.position += new Vector3(item.speed * Time.deltaTime, 0.0f, 0.0f);

		// make sure we aren't checking any points that don't exist anymore
		if (actionPoints.Count > 0 && currentActionPointIndex >= 0 && currentActionPointIndex < actionPoints.Count)
			CheckAction( actionPoints[currentActionPointIndex] );
	}

	// this will be the new check
	protected void CheckAction(Transform actionPoint)
	{
		// we're only interested in the frontmost item
		if (this.item.lane.GetCurrentLeadingItem().laneItemRenderer != this)	
			return;

		// within range of press point
		if(Vector2.Distance( actionPoint.transform.position.v2(), item.lane.pressPoint.position.v2() ) < 0.8f )
		{
			if (Input.touchCount > 0)
			{
				CheckTouchInputCorrect(actionPoint);
			}
			else
			{
				CheckKeyInputCorrect(actionPoint);
			}
		}
		// in front of press point
		else if (transform.localPosition.x + actionPoint.transform.localPosition.x < item.lane.pressPoint.localPosition.x)
		{
			if (Input.touchCount > 0)
			{
				CheckTouchInputEarly(actionPoint);
			}
			else
			{
				CheckKeyInputEarly(actionPoint);
			}
		}
		// past press point
		else if (transform.localPosition.x + actionPoint.transform.localPosition.x > item.lane.pressPoint.transform.localPosition.x)
		{
			// by definition, missing the first point of a streak means missing the entire thing, so it's pointless to check for streaks separately
			MissedSingle();

			// once point is offscreen, remove it 
			CheckOffScreen(actionPoint);
		}
	}

	protected void CheckTouchInputCorrect(Transform actionPoint)
	{
		Touch pressPointTouch;
		if (WasPressPointTouched(out pressPointTouch))
		{
			RegisterLaneChange();

			if (pressPointTouch.phase == TouchPhase.Began)
			{
				// player hit action point or first action point of streak
				if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
					DetectSingle(true, actionPoint);
				else
					DetectStreak(true, actionPoint);
			}
			else if (pressPointTouch.phase == TouchPhase.Ended)
			{
				// player released touch on last action point of streak
				if ( this.item.type == KikaAndBob.LaneItemType.STREAK && currentActionPointIndex > 0)
					DetectStreak(false, actionPoint);
			}
		}
	}

	protected void CheckTouchInputEarly(Transform actionPoint)
	{
		Touch pressPointTouch;
		if (WasPressPointTouched(out pressPointTouch))
		{
			RegisterLaneChange();

			if (pressPointTouch.phase == TouchPhase.Began)
			{
				this.item.lane.HighlightLaneNegative();
				DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
			}
			else if (pressPointTouch.phase == TouchPhase.Ended)
			{
				if (this.item.type == KikaAndBob.LaneItemType.STREAK && currentActionPointIndex > 0)
				{
					this.item.lane.HighlightLaneNegative();
					DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
					MissedSingle();
				}
			}
		}
	}

	protected void CheckKeyInputCorrect(Transform actionPoint)
	{
		if( LugusInput.use.KeyDown( item.KeyCode ) )
		{
			if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
				DetectSingle(true, actionPoint);
			else
				DetectStreak(true, actionPoint);

			RegisterLaneChange();
		}

		if( LugusInput.use.KeyUp( item.KeyCode ) )
		{
			if( this.item.type == KikaAndBob.LaneItemType.STREAK )
				DetectStreak(false, actionPoint);

			RegisterLaneChange();
		}
	}

	protected void CheckKeyInputEarly(Transform actionPoint)
	{
		if (this.item.type == KikaAndBob.LaneItemType.STREAK && currentActionPointIndex > 0)
		{
			if( LugusInput.use.KeyUp( item.KeyCode ) )
			{
				this.item.lane.HighlightLaneNegative();
				DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
				MissedSingle();

				RegisterLaneChange();
			}
		}
		else
		{
			if( LugusInput.use.KeyDown( item.KeyCode ) )
			{
				this.item.lane.HighlightLaneNegative();
				DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);

				RegisterLaneChange();
			}
		}
	}

	protected bool WasPressPointTouched(out Touch pressPointTouch)
	{
		pressPointTouch = new Touch();
		print ("----------------------------");
		
		for (int i = 0; i < Input.touchCount; i++) 
		{
			Touch touch = Input.GetTouch(i);

			if (this.item.lane.name == "Lane1")
			{
				print (touch.position);
			}

			RaycastHit2D rayHit = Physics2D.Raycast(LugusCamera.game.ScreenToWorldPoint(touch.position).v2(), Vector3.forward.v2());
	
			if (rayHit.collider != null && rayHit.collider.transform == this.item.lane.pressPoint)
			{
				pressPointTouch = touch;
				return true;
			}
		}

		return false;
	}


//	protected void CheckAction(Transform actionPoint)
//	{
//		if(Vector2.Distance(actionPoint.transform.position.v2 (), item.lane.actionPoint.transform.position.v2()) < 0.8f )
//		{
//			if (this.item.lane.GetCurrentLeadingItem().laneItemRenderer == this)	// i.e. if this the most forward lane item renderer
//			{
//				if( this.item.actionType == KikaAndBob.LaneItemActionType.BUTTON )
//				{
//					// TODO: raycast! both down and up
//					if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
//						DetectSingle(true, actionPoint);
//					else
//						DetectStreak(true, actionPoint);
//				}
//				else
//				{
//					if (Input.touchCount >= 1 && LugusInput.use.down)
//					{
//						RaycastHit2D rayHit = Physics2D.Raycast(LugusCamera.game.ScreenToWorldPoint(Input.GetTouch(0).position).v2(), Vector3.forward.v2());
//						Transform hitTransform = null;
//
//						if (rayHit.collider != null)
//						{
//							hitTransform = rayHit.collider.transform;
//						}
//
//						if (hitTransform == this.item.lane.actionPoint)
//						{
//							if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
//								DetectSingle(true, actionPoint);
//							else
//								DetectStreak(true, actionPoint);
//							
//							RegisterLaneChange();
//						}
//					}
//					else if (Input.touchCount >= 1 && LugusInput.use.up)
//					{
//						RaycastHit2D rayHit = Physics2D.Raycast(LugusCamera.game.ScreenToWorldPoint(Input.GetTouch(0).position).v2(), Vector3.forward.v2());
//						Transform hitTransform = null;
//
//						if (rayHit.collider != null)
//						{
//							hitTransform = rayHit.collider.transform;
//						}
//
//						if (hitTransform == this.item.lane.actionPoint)
//						{
//							if( this.item.type == KikaAndBob.LaneItemType.STREAK )
//								DetectStreak(false, actionPoint);
//
//							RegisterLaneChange();
//						}
//					}
//					else
//					{
//						if( LugusInput.use.KeyDown( item.KeyCode ) )
//						{
//							if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
//								DetectSingle(true, actionPoint);
//							else
//								DetectStreak(true, actionPoint);
//
//							RegisterLaneChange();
//						}
//
//						if( LugusInput.use.KeyUp( item.KeyCode ) )
//						{
//							if( this.item.type == KikaAndBob.LaneItemType.STREAK )
//								DetectStreak(false, actionPoint);
//
//							// No point to hitting single points on key up
//
//	//						if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
//	//							DetectSingle(false, actionPoint);
//	//						else
//	//							DetectStreak(false, actionPoint);
//
//							RegisterLaneChange();
//						}
//					}
//				}
//			}
//		}
//		// in front of action point
//		// transform.localPosition.x + actionPoint.transform.localPosition.x = rightmost point of this item or streak
//		else if (transform.localPosition.x + actionPoint.transform.localPosition.x < item.lane.actionPoint.transform.localPosition.x)
//		{
//			// we only want this checked for the currently frontmost lane item; 
//			// if not, this will always trigger (even while hitting an action point) as long as there are spawned LaneItemRenders past the first one
//			// this means we also want to keep track of which LaneItem is currently the leading one for each lane
//			// DanceHeroLane.use.IncreaseLeadingLaneItem() should therefore be called both from succesful hits as missed hits
//			if (this.item.lane.GetCurrentLeadingItem().laneItemRenderer == this)
//			{
//				if (Input.touchCount >= 1 && LugusInput.use.down)
//				{
//					RaycastHit2D rayHit = Physics2D.Raycast(LugusCamera.game.ScreenToWorldPoint(Input.GetTouch(0).position).v2(), Vector3.forward.v2());
//					Transform hitTransform = null;
//					
//					if (rayHit.collider != null)
//					{
//						hitTransform = rayHit.collider.transform;
//					}
//
//					if (hitTransform == this.item.lane.actionPoint)
//					{
//						if ( LugusInput.use.down )
//						{
//							 if ( this.item.type == KikaAndBob.LaneItemType.SINGLE || 
//							 	( this.item.type == KikaAndBob.LaneItemType.STREAK && currentActionPointIndex <= 0) )
//							{
//								this.item.lane.HighlightLaneNegative();
//								DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
//								
//								RegisterLaneChange();
//							}
//						}
//						else if ( LugusInput.use.up && this.item.type == KikaAndBob.LaneItemType.STREAK && currentActionPointIndex > 0)
//						{
//							this.item.lane.HighlightLaneNegative();
//							DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
//							MissedSingle();
//							
//							RegisterLaneChange();
//						}
//					}
//				}
//				else
//				{
//					if (this.item.type == KikaAndBob.LaneItemType.STREAK && currentActionPointIndex > 0)
//					{
//						if( LugusInput.use.KeyUp( item.KeyCode ) )
//						{
//							this.item.lane.HighlightLaneNegative();
//							DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
//							MissedSingle();
//
//							RegisterLaneChange();
//						}
//					}
//					else
//					{
//						if( this.item.actionType == KikaAndBob.LaneItemActionType.BUTTON )
//						{
//							this.item.lane.HighlightLaneNegative();
//							DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
//						}
//						else
//						{
//							if( LugusInput.use.KeyDown( item.KeyCode ) )
//							{
//								this.item.lane.HighlightLaneNegative();
//								DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_INCORRECT, item.lane);
//
//								RegisterLaneChange();
//							}
//						}
//					}
//				}
//			}
//		}
//		// past action point
//		// transform.localPosition.x + actionPoint.transform.localPosition.x = rightmost point of this item or streak
//		else if (transform.localPosition.x + actionPoint.transform.localPosition.x > item.lane.actionPoint.transform.localPosition.x)
//		{
//			// by definition, missing the first point of a streak means missing the entire thing, so it's pointless to check for streaks separately
//			MissedSingle();
//			CheckOffScreen(actionPoint);
//		}
//	}

	protected void MissedSingle()
	{
		if (missed)
			return;

		this.item.lane.StopHighlight();

		missed = true;
		this.item.lane.IncreaseLeadingLaneItem();
		DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_MISSED, item.lane);

		foreach(Transform t in actionPoints)
		{
			if (t == null)
				continue;

			foreach(SpriteRenderer sr in t.GetComponentsInChildren<SpriteRenderer>())
			{
				// darken sprite, slighty transparent
				sr.color = (sr.color * 0.5f).a(sr.color.a * 0.8f);
			}

			if (item.type == KikaAndBob.LaneItemType.SINGLE)
				t.gameObject.ScaleTo(t.localScale * 0.6f).Time(0.25f).EaseType(iTween.EaseType.easeOutQuad).Execute();
			else
			{
				if (streakRoutine != null && streakRoutine.Running)
				{
					streakRoutine.StopRoutine();
				}


				// the line objects need to be scaled up to compensate for scaling the whole thing down!
				t.gameObject.ScaleTo(t.localScale * 0.6f).Time(0.25f).EaseType(iTween.EaseType.easeOutQuad).Execute();
		
				Transform line = t.FindChild("Line");

				if (line == null)
				{
					Debug.LogError("Could not find Line game object under this action point.");
					continue;
				}

				line.gameObject.ScaleTo(new Vector3(line.localScale.x / 0.6f, line.localScale.y, line.localScale.z)).Time(0.25f).EaseType(iTween.EaseType.easeOutQuad).Execute();

			}
		}
	}

	protected void RegisterLaneChange()
	{
		if (DanceHeroFeedback.use.onButtonPress != null)
		{
			DanceHeroFeedback.use.onButtonPress(item.lane);
		}
	}

	// after the player has missed the action point, check if it is off screen
	// if yes, allow some time for the particle effect to fade out, then remove the game object
	protected void CheckOffScreen(Transform actionPoint)
	{
		float minX = LugusCamera.game.WorldToScreenPoint(actionPoint.FindChild("Color").renderer.bounds.min).x;
		//float minX = LugusCamera.game.WorldToScreenPoint(actionPoint.FindChild("Background").renderer.bounds.min).x;

		if (minX > Screen.width)
		{
			offScreenTime += Time.deltaTime;
		}

		if (offScreenTime >= offScreenRemoveTime)
			DeleteActionPoint(actionPoint);
	}

	protected void DetectSingle(bool keyDown, Transform actionPoint)
	{
		item.lane.HighLightLanePositive();
	//	DanceHeroFeedbackChina.use.HighLightLane(this.item.lane.actionPoint);
		DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_CORRECT, item.lane);

		hit = true;

		this.item.lane.IncreaseLeadingLaneItem();

		GameObject.Destroy(this.gameObject);
	}

	protected void DeleteActionPoint(Transform actionPoint)
	{
		actionPoints.Remove(actionPoint);

		if (actionPoints.Count == 0)
		{
			Destroy(this.gameObject);
			return;
		}

		Destroy( actionPoint.gameObject );
	}

	protected Transform GetActionPoint(string actionPointName)
	{
		foreach(Transform t in actionPoints)
		{
			if (t.name == actionPointName)
			{
				return t;
			}
		}

		return null;
	}

	protected IEnumerator LineFlashRoutine()
	{
		Transform firstPoint = GetActionPoint("Point1");
		if (firstPoint == null)
		{
			Debug.LogError("DanceHeroLaneItemRenderer: First point missing!");
			yield break;
		}

		Transform secondPoint = GetActionPoint("Point2");
		if (secondPoint == null)
		{
			Debug.LogError("DanceHeroLaneItemRenderer: Second point missing!");
			yield break;
		}

		Transform firstLine = firstPoint.FindChild("Line");
		Transform secondLine = secondPoint.FindChild("Line");
		SpriteRenderer secondLineRenderer = secondLine.GetComponent<SpriteRenderer>();
	
		float start = firstPoint.position.x;

		float line1Scale = firstLine.localScale.x;
		float line2Scale = secondLine.localScale.x;

		float distanceToCover = Mathf.Abs(firstPoint.position.x - secondLine.position.x);
		float distanceCovered = 0;

		float previousLocation = 0;

		firstLine.Rotate(new Vector3(0.0f, 0.0f, 180.0f));

		float alphaFade = 0.6f;
		float alphaFadeSpeed = 10f;

		this.item.lane.HighLightLanePositive(true);

		// take the two lines under the two action points and gradually switch their scale to create the impression of a continuous
		// line, split into two halves with different effects
		while (secondPoint != null && secondPoint.transform.position.x < start)
		{
			float lerp = 1 - ((start - secondPoint.transform.position.x) / distanceToCover);

			firstLine.transform.localScale = firstLine.transform.localScale.x(Mathf.Lerp(line1Scale, line2Scale, lerp));
			secondLine.transform.localScale = secondLine.transform.localScale.x(Mathf.Lerp(line2Scale, line1Scale, lerp));

			// second line fades in and out
			secondLineRenderer.color = secondLineRenderer.color.a( 1 - (alphaFade * Mathf.Sin(alphaFadeSpeed * Time.time)));

			yield return new WaitForEndOfFrame();
		}



		yield break;
	}

	protected void DetectStreak(bool keyDown, Transform actionPoint)
	{
		// TODO: what if point 0 and up... hmz
		if( currentActionPointIndex == 0 && keyDown )
		{
			//item.lane.HighLightLanePositive(item.lane.actionPoint);


			if (streakRoutine != null && streakRoutine.Running)
			{
				streakRoutine.StopRoutine();
			}

			streakRoutine = LugusCoroutines.use.StartRoutine(LineFlashRoutine());

			//we've hit the first actionPoint and pressed button down: ideal

			currentActionPointIndex++; 
		}

		if( currentActionPointIndex == 1 && !keyDown ) // keyUp
		{
			hit = true;

			this.item.lane.StopHighlight();

			item.lane.HighLightLanePositive();

			DeleteActionPoint(actionPoint);

			this.item.lane.IncreaseLeadingLaneItem();

			DanceHeroFeedback.use.UpdateScore(DanceHeroFeedback.ScoreType.PRESS_CORRECT, item.lane);

			GameObject.Destroy(this.gameObject);
		}
	}

	protected void OnGUI()
	{
		if( !LugusDebug.debug )
			return;

		//Vector2 screenPoint = LugusCamera.game.WorldToScreenPoint(this.transform.position);

		//GUI.TextField( new Rect(screenPoint.x, Screen.height - screenPoint.y, 50, 50), "" + Vector2.Distance(this.transform.position.v2 (), item.lane.actionPoint.transform.position.v2()));
	}
}
