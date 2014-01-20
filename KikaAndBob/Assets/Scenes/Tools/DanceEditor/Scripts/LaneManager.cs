using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaneManager : LugusSingletonRuntime<LaneManager>
{
	public Vector2 screenOffset = new Vector2(10, 10);
	public float laneSpacing = 0.5f;

	public Lane lanePrefab = null;
	public LaneItemRenderer laneItemRendererPrefab = null;
	public Transform hitLine = null;

	public List<Lane> Lanes
	{
		get
		{
			return _lanes;
		}
	}

	protected List<Lane> _lanes = new List<Lane>();
	protected Lane _currentLane = null;
	protected LaneItemRenderer _currentLaneItem = null;

	void Start()
	{
		RepositionLanes();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Delete) && (_currentLaneItem != null))
		{
			RemoveActionPoint();
		}
	}

	void OnGUI()
	{
		DrawLanesGUI();
		DrawLaneItemGUI();
	}

	void DrawLanesGUI()
	{
		float xpos = screenOffset.x;
		float ypos = screenOffset.y;
		float width = 200;
		float height = 50;

		GUILayout.BeginArea(new Rect(xpos, ypos, width, height), GUI.skin.box);
		GUILayout.BeginVertical();

		// Display title of the box
		GUIStyle centered = new GUIStyle(GUI.skin.label);
		centered.alignment = TextAnchor.UpperCenter;
		GUILayout.Label("Lane Manager", centered);

		if (GUILayout.Button("Create new lane"))
			CreateLane();

		GUILayout.EndVertical();
		GUILayout.EndArea();

		// Add name and remove button at the location of each lane
		for (int i = 0; i < _lanes.Count; ++i)
		{
			Lane lane = _lanes[i];

			float laneposy = Camera.main.WorldToScreenPoint(lane.transform.position).y;
			laneposy = Screen.height - laneposy;

			if (GUI.Button(new Rect(30, laneposy - 10, 70, 20), "Remove"))
			{
				RemoveLane(lane);
				--i;
			}

			GUI.Label(new Rect(110, laneposy - 10, 100, 20), "Lane " + (i + 1).ToString());
		}
	}

	void DrawLaneItemGUI()
	{
		if ((_currentLane == null) || (_currentLaneItem == null))
			return;

		float xpos = screenOffset.x + 210;
		float ypos = screenOffset.y;
		float width = 200;
		float height = 150;
		int itemIndex = _currentLane.LaneItems.FindIndex(l => l == _currentLaneItem.Item);

		GUILayout.BeginArea(new Rect(xpos, ypos, width, height), GUI.skin.box);
		GUILayout.BeginVertical();

		// Display the title of the box
		GUIStyle centered = new GUIStyle(GUI.skin.label);
		centered.alignment = TextAnchor.UpperCenter;
		GUILayout.Label("Action Point " + (itemIndex + 1).ToString(), centered);

		// Display the time at which the action point is located
		GUILayout.BeginHorizontal();
		GUILayoutOption[] options = new GUILayoutOption[1];
		options[0] = GUILayout.Width(95);

		GUILayout.Label("Time", options);

		string newTimeStr = GUILayout.TextField(_currentLaneItem.Item.Time.ToString("0.000"), options);
		float newTime;
		if (float.TryParse(newTimeStr, out newTime))
		{
			if (newTime < 0.0f)
				newTime = 0.0f;
			else if (newTime > AudioPlayer.use.Source.clip.length)
				newTime = _currentLaneItem.Time;

			_currentLaneItem.Time = newTime;
		}
		GUILayout.EndHorizontal();

		// Display the type of the action point
		GUILayout.BeginHorizontal();
		GUILayout.Label("Type", options);
		if (_currentLaneItem.Item.Type == DanceEditor.LaneItemType.SINGLE)
			GUILayout.Label("Single", options);
		else if (_currentLaneItem.Item.Type == DanceEditor.LaneItemType.STREAK)
			GUILayout.Label("Streak", options);

		GUILayout.EndHorizontal();

		// Display the duration of the action point
		GUILayout.BeginHorizontal();
		GUILayout.Label("Duration", options);
		string newDurationStr = GUILayout.TextField(_currentLaneItem.Item.Duration.ToString("0.000"), options);
		float newDuration;
		if (float.TryParse(newDurationStr, out newDuration))
		{
			if (newDuration < LaneItem.singleDuration)
				newDuration = LaneItem.singleDuration;
			else if (newDuration > LaneItem.maxStreakDuration)
				newDuration = _currentLaneItem.Duration;

			_currentLaneItem.Duration = newDuration;
		}
		GUILayout.EndHorizontal();

		// Add a slider to modify the duration of the action point
		_currentLaneItem.Duration = GUILayout.HorizontalSlider(_currentLaneItem.Duration, LaneItem.singleDuration, LaneItem.maxStreakDuration);

		// Remove button for the action point
		if (GUILayout.Button("Remove"))
		{
			RemoveActionPoint();
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	void CreateLane()
	{
		if (_currentLane != null)
			_currentLane.Highlight = false;

		GameObject parent = new GameObject("LaneContainer");
		parent.transform.position = Vector3.zero;
		parent.transform.localScale = Vector3.one;

		Lane lane = GameObject.Instantiate(lanePrefab) as Lane;
		lane.transform.parent = parent.transform;

		_currentLane = lane;
		_currentLane.Highlight = true;

		_lanes.Add(lane);

		RepositionLanes();
	}

	void RemoveLane()
	{
		if (_currentLane == null)
			return;

		_lanes.Remove(_currentLane);
		GameObject.Destroy(_currentLane.transform.parent.gameObject);

		if (_lanes.Count != 0)
		{
			_currentLane = _lanes[0];
			_currentLane.Highlight = true;
		}
		else
		{
			_currentLane = null;
		}

		RepositionLanes();
	}

	void RemoveLane(Lane lane)
	{
		if (lane == _currentLane)
		{
			RemoveLane();
			return;
		}

		_lanes.Remove(lane);
		GameObject.Destroy(lane.transform.parent.gameObject);

		RepositionLanes();
	}

	void RepositionLanes()
	{
		if (_lanes.Count == 0)
		{
			hitLine.renderer.enabled = false;
			return;
		}
	
		// Reposition all lanes so that they are centered around the hit line
		Lane lane = _lanes[0];
		float height = lane.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2.0f * lane.transform.localScale.y;
		Vector2 position = new Vector2(0, hitLine.position.y + ((height + laneSpacing) * (_lanes.Count - 1) / 2));
		float delta = - (height + laneSpacing);

		for (int i = 0; i < _lanes.Count; ++i)
		{
			_lanes[i].transform.position = position;
			position.y += delta;
		}

		// Scale the hit line
		hitLine.renderer.enabled = true;
		Vector3 newScale = hitLine.transform.localScale;
		newScale.y = (height + laneSpacing) * _lanes.Count;
		hitLine.transform.localScale = newScale;
	}

	void RemoveActionPoint()
	{
		_currentLane.LaneItems.Remove(_currentLaneItem.Item);
		_currentLane.LaneItemRenderers.Remove(_currentLaneItem);
		GameObject.Destroy(_currentLaneItem.gameObject);
		ClearCurrentLaneItemData();
	}

	void ClearCurrentLaneItemData()
	{
		_currentLaneItem = null;
	}

	public void SetCurrentLane(Lane currentLane)
	{
		if (_currentLane != null)
			_currentLane.Highlight = false;

		_currentLane = currentLane;
		_currentLane.Highlight = true;
	}

	public void SetCurrentLaneItem(LaneItemRenderer currentLaneItem)
	{
		if (_currentLaneItem != null)
			_currentLaneItem.HighLight = false;

		_currentLaneItem = currentLaneItem;

		if (_currentLaneItem == null)
			return;

		SetCurrentLane(currentLaneItem.Lane);
		_currentLaneItem.HighLight = true;
	}

	public void CreateLane(TinyXmlReader reader)
	{
		CreateLane();
		_currentLane.FromXML(reader);
	}
}
