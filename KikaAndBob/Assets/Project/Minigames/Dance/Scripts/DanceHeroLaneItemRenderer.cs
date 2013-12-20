using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroLaneItemRenderer : MonoBehaviour 
{
	protected static int itemCount = 0; // this has no use except for adding a different number to every lane item for debugging
	public static DanceHeroLaneItemRenderer Create(DanceHeroLaneItem item)
	{
		DanceHeroLane lane = item.lane;
		// TODO: make this more official or provide prefabs for ActionPoint visualizers
		GameObject actionIndicatorPrefab = lane.transform.FindChild("ActionPoint").gameObject;

		GameObject container = new GameObject("Item" + itemCount);
		container.transform.parent = lane.transform;
		container.transform.position = lane.transform.position;

		DanceHeroLaneItemRenderer renderer = container.AddComponent<DanceHeroLaneItemRenderer>();
		renderer.item = item;

		if( item.type == KikaAndBob.LaneItemType.SINGLE )
		{
			// single button : just one actionpoint
			
			GameObject actionPoint = (GameObject) GameObject.Instantiate( actionIndicatorPrefab );

			// we're directly instantiating the lane action point
			// don't forget to disable the highlight, because it might still have been active from the highlight effect
			actionPoint.transform.FindChild("Highlight").gameObject.SetActive(false); 

			actionPoint.name = "Item" + itemCount.ToString()+"_Point";
			actionPoint.transform.parent = container.transform;
			actionPoint.transform.position = container.transform.position + new Vector3(0, 0, -1);

			renderer.actionPoints.Add( actionPoint.transform );
		}
		else
		{
			// streak button : 2 actionpoints with a line in between

			// point 1
			GameObject actionPoint = (GameObject) GameObject.Instantiate( actionIndicatorPrefab );
			
			actionPoint.name = "Item" + itemCount.ToString()+"_Point1";
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
			
			actionPoint.name = "Item" + itemCount.ToString()+"_Point2";
			actionPoint.transform.parent = container.transform;
			actionPoint.transform.position = container.transform.position + new Vector3(-1 * units, 0.0f, 0.0f);
			// we're directly instantiating the lane action point
			// don't forget to disable the highlight, because it might still have been active from the highlight effect
			actionPoint.transform.FindChild("Highlight").gameObject.SetActive(false); 


			// TODO: make the line function better... now it's just in the ActionPoint prefab, but often it goes unused, causing overhead...
			Transform line = actionPoint.transform.FindChild("Line");
			line.localScale = line.localScale.x ( units / line.renderer.bounds.size.x );
			Debug.LogError ("Line width " + line.renderer.bounds.size.x );

			
			renderer.actionPoints.Add( actionPoint.transform );

		}

		itemCount++;

		return renderer;
	}

	public List<Transform> actionPoints = new List<Transform>();
	public int currentActionPointIndex = 0;

	protected bool hit = false;
	protected bool missed = false;

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

	protected void CheckAction(Transform actionPoint)
	{
		if(Vector2.Distance(actionPoint.transform.position.v2 (), item.lane.actionPoint.transform.position.v2()) < 0.8f )
		{
			Debug.Log(actionPoint.name, actionPoint);
			if( this.item.actionType == KikaAndBob.LaneItemActionType.BUTTON )
			{
				// TODO: raycast! both down and up
				if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
					DetectSingle(true, actionPoint);
				else
					DetectStreak(true, actionPoint);
			}
			else
			{
				if( LugusInput.use.KeyDown( item.KeyCode ) )
				{
					if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
						DetectSingle(true, actionPoint);
					else
						DetectStreak(true, actionPoint);
				}

				// TODO: for streak, up also needs to be detected if it happens in between: need to keep it going untill the end!
				if( LugusInput.use.KeyUp( item.KeyCode ) )
				{
					if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
						DetectSingle(false, actionPoint);
					else
						DetectStreak(false, actionPoint);
				}
			}
		}
		else
		{
			if (transform.localPosition.x + actionPoint.transform.localPosition.x > item.lane.actionPoint.transform.localPosition.x)
			{
				MissedSingle();
				CheckOffScreen(actionPoint);
			}
		}
	}

	protected void MissedSingle()
	{
		if (missed)
			return;

		missed = true;
		DanceHeroFeedback.use.UpdateScore(false);
	}

	protected void CheckOffScreen(Transform actionPoint)
	{
		float minX = LugusCamera.game.WorldToScreenPoint(actionPoint.FindChild("Background").renderer.bounds.min).x;

		if (minX > Screen.width)
			DeleteActionPoint(actionPoint);
	}

	protected void DetectSingle(bool keyDown, Transform actionPoint)
	{
		DanceHeroFeedback.use.HighLightLane(this.item.lane.actionPoint);
		DanceHeroFeedback.use.UpdateScore(true);
		hit = true;

		GameObject.Destroy(this.gameObject);
		//DeleteActionPoint(actionPoint);
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

	protected void DetectStreak(bool keyDown, Transform actionPoint)
	{
		// TODO: what if point 0 and up... hmz
		if( currentActionPointIndex == 0 && keyDown )
		{
			DanceHeroFeedback.use.HighLightLane(item.lane.actionPoint);

			// we've hit the first actionPoint and pressed button down: ideal
			
			GameObject.Destroy(actionPoints[currentActionPointIndex].gameObject);

			currentActionPointIndex++; 
		}

		if( currentActionPointIndex == 1 && !keyDown ) // keyUp
		{
			hit = true;
			DanceHeroFeedback.use.HighLightLane(item.lane.actionPoint);
			DanceHeroFeedback.use.UpdateScore(true);

			DeleteActionPoint(actionPoint);

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
