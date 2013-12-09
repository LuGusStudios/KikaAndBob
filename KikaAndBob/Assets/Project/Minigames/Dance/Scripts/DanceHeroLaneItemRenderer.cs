using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanceHeroLaneItemRenderer : MonoBehaviour 
{
	public static DanceHeroLaneItemRenderer Create(DanceHeroLaneItem item)
	{
		DanceHeroLane lane = item.lane;
		// TODO: make this more official or provide prefabs for ActionPoint visualizers
		GameObject actionIndicatorPrefab = lane.transform.FindChild("ActionPoint").gameObject;

		GameObject container = new GameObject("Item");
		container.transform.parent = lane.transform;
		container.transform.position = lane.transform.position;

		DanceHeroLaneItemRenderer renderer = container.AddComponent<DanceHeroLaneItemRenderer>();
		renderer.item = item;

		if( item.type == KikaAndBob.LaneItemType.SINGLE )
		{
			// single button : just one actionpoint
			
			GameObject actionPoint = (GameObject) GameObject.Instantiate( actionIndicatorPrefab );
			
			actionPoint.name = "Point";
			actionPoint.transform.parent = container.transform;
			actionPoint.transform.position = container.transform.position;

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


			// TODO: make the line function better... now it's just in the ActionPoint prefab, but often it goes unused, causing overhead...
			Transform line = actionPoint.transform.FindChild("Line");
			line.localScale = line.localScale.x ( units / line.renderer.bounds.size.x );
			Debug.LogError ("Line width " + line.renderer.bounds.size.x );

			
			renderer.actionPoints.Add( actionPoint.transform );

		}

		return renderer;
	}

	public List<Transform> actionPoints = new List<Transform>();
	public int currentActionPointIndex = 0;

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


#if UNITY_IPHONE || UNITY_ANDROID
		item.type = KikaAndBob.LaneItemType.BUTTON;
#endif
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

		CheckAction( actionPoints[currentActionPointIndex] );
	}

	protected void CheckAction(Transform actionPoint)
	{
		// TODO: if the user misses the press and we move offscreen, make sure we're destroyed as well
		
		if( Vector2.Distance(actionPoint.transform.position.v2 (), item.lane.actionPoint.transform.position.v2()) < 0.8f )
		{
			if( this.item.actionType == KikaAndBob.LaneItemActionType.BUTTON )
			{
				// TODO: raycast! both down and up
				if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
					DetectSingle(true);
				else
					DetectStreak(true);
			}
			else
			{
				if( LugusInput.use.KeyDown( item.KeyCode ) )
				{
					if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
						DetectSingle(true);
					else
						DetectStreak(true);
				}

				// TODO: for streak, up also needs to be detected if it happens in between: need to keep it going untill the end!
				if( LugusInput.use.KeyUp( item.KeyCode ) )
				{
					if( this.item.type == KikaAndBob.LaneItemType.SINGLE )
						DetectSingle(false);
					else
						DetectStreak(false);
				}
			}
		}
	}

	protected void DetectSingle(bool keyDown)
	{
		// TODO
		GameObject.Destroy( this.gameObject );
	}

	protected void DetectStreak(bool keyDown)
	{
		// TODO: what if point 0 and up... hmz
		if( currentActionPointIndex == 0 && keyDown )
		{
			// we've hit the first actionPoint and pressed button down: ideal
			
			GameObject.Destroy( actionPoints[currentActionPointIndex].gameObject );

			currentActionPointIndex++; 
		}

		if( currentActionPointIndex == 1 && !keyDown ) // keyUp
		{
			GameObject.Destroy( this.gameObject );
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
