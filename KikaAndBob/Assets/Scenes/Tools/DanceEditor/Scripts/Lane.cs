using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * Maintains a list of LaneItems.
 * Is responsible for spawning and removing LaneItemRenderers.
 * Changes color when selected.
 **/
[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class Lane : MonoBehaviour
{

	#region Public
	public bool Highlight
	{
		get
		{
			return _highlight;
		}
		set
		{
			_highlight = value;
			_changed = true;
		}
	}

	public Sprite normalBorder = null;
	public Sprite highlightBorder = null;
	public Vector2 laneScale = new Vector2(20.0f, 0.5f);	// Scale used for the lane texture
	public Vector2 bounds = new Vector2(-15.0f, 15.0f);		// Bounds used in creating and destroying LaneItemRenderers

	public List<LaneItem> LaneItems
	{
		get
		{
			return _laneItems;
		}
	}
	public List<LaneItemRenderer> LaneItemRenderers
	{
		get
		{
			return _laneItemRenderers;
		}
	}
	#endregion

	#region Protected
	protected List<LaneItem> _laneItems = new List<LaneItem>();
	protected List<LaneItemRenderer> _laneItemRenderers = new List<LaneItemRenderer>();

	protected bool _highlight = false;
	protected bool _changed = false;

	protected LaneItemRenderer _laneItemIntermediate = null;	// Used for creating a visual indication of the new lane item
	#endregion

	void Awake()
	{
		this.transform.localScale = new Vector3(laneScale.x, laneScale.y, 0.0f);
	}

	void Update()
	{
		// Highlight the lane if its selected
		if (_changed)
		{
			if (_highlight)
				GetComponent<SpriteRenderer>().sprite = highlightBorder;
			else
				GetComponent<SpriteRenderer>().sprite = normalBorder;

			_changed = false;
		}

		// Check whether this lane's number-key is pressed
		int index = LaneManager.use.Lanes.IndexOf(this) + 1;
		if (Input.GetKeyDown(index.ToString()))
			OnNumberKeyDown();
		else if (Input.GetKey(index.ToString()))
			OnNumberKeyHold();
		else if (Input.GetKeyUp(index.ToString()))
			OnNumberKeyUp();

		// Destroy LaneItemRenderers for items that are out of bounds
		for (int i = 0; i < _laneItemRenderers.Count; )
		{
			LaneItemRenderer renderer = _laneItemRenderers[i];

			if (((renderer.Item.Time < AudioPlayer.use.SeekTime + (bounds.x / LaneItemRenderer.Speed)) ||
				(renderer.Item.Time > AudioPlayer.use.SeekTime + (bounds.y / LaneItemRenderer.Speed))) &&
				(renderer != _laneItemIntermediate))
			{
				_laneItemRenderers.RemoveAt(i);
				GameObject.Destroy(renderer.gameObject);
			}
			else
				++i;
		}

		// Instantiate LaneItemRenderers for items that are within bounds
		foreach(LaneItem item in _laneItems)
		{
			if ((item.Time < AudioPlayer.use.SeekTime + (bounds.x / LaneItemRenderer.Speed)) ||
				(item.Time > AudioPlayer.use.SeekTime + (bounds.y / LaneItemRenderer.Speed)))
				continue;

			if (_laneItemRenderers.Exists(e => e.Item == item))
				continue;

			_laneItemRenderers.Add(LaneItemRenderer.Create(this, item));
		}
	}

	void OnMouseDown()
	{
		LaneManager.use.SetCurrentLane(this);

		if (_laneItemIntermediate != null)
			return;

		float time = 0.0f;
		if (AudioPlayer.use.IsPlaying)
		{
			time = AudioPlayer.use.Source.time;
		}
		else
		{
			Vector3 worldpos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, this.transform.position.y, 0.0f));
			float xdiff = HitLine.use.transform.position.x - worldpos.x;
			time = AudioPlayer.use.SeekTime + xdiff / LaneItemRenderer.Speed;
		}

		LaneItem item = new LaneItem(time);
		AddLaneItem(item);

		LaneItemRenderer renderer = LaneItemRenderer.Create(this, item);
		_laneItemRenderers.Add(renderer);

		_laneItemIntermediate = renderer;
	}

	void OnMouseDrag()
	{
		if (_laneItemIntermediate == null)
			return;

		if (AudioPlayer.use.IsPlaying)
			_laneItemIntermediate.Duration = AudioPlayer.use.Source.time - _laneItemIntermediate.Time;
		else
			_laneItemIntermediate.Duration = LaneItem.singleDuration;
	}

	void OnMouseUp()
	{
		if (_laneItemIntermediate == null)
			return;

		_laneItemIntermediate = null;
	}

	void OnNumberKeyDown()
	{
		if (_laneItemIntermediate != null)
			return;

		LaneItem item = new LaneItem(AudioPlayer.use.Source.time);
		AddLaneItem(item);

		LaneItemRenderer renderer = LaneItemRenderer.Create(this, item);
		_laneItemRenderers.Add(renderer);

		_laneItemIntermediate = renderer;
	}

	void OnNumberKeyHold()
	{
		if (_laneItemIntermediate == null)
			return;

		if (AudioPlayer.use.IsPlaying)
			_laneItemIntermediate.Duration = AudioPlayer.use.Source.time - _laneItemIntermediate.Time;
		else
			_laneItemIntermediate.Duration = LaneItem.singleDuration;
	}

	void OnNumberKeyUp()
	{
		if (_laneItemIntermediate == null)
			return;

		_laneItemIntermediate = null;
	}

	public void AddLaneItem(float time, float duration = LaneItem.singleDuration)
	{
		AddLaneItem(new LaneItem(time, duration));
	}

	public void AddLaneItem(LaneItem laneItem)
	{
		for (int i = 0; i < _laneItems.Count; ++i)
		{
			if (_laneItems[i].Time > laneItem.Time)
			{
				_laneItems.Insert(i, laneItem);
				return;
			}
		}

		_laneItems.Add(laneItem);
	}
	
	public string ToXML(int depth)
	{
		_laneItems = _laneItems.OrderBy(o => o.Time).ToList();

		string data = string.Empty;

		string lanesTabs = string.Empty;
		for (int i = 0; i < depth; ++i)
			lanesTabs += "\t";

		string laneTabs = lanesTabs + "\t";

		data += lanesTabs + "<Lane>\r\n";

		foreach(LaneItem laneItem in _laneItems)
		{
			data += laneTabs + "<Item>\r\n";
			data += laneTabs + "\t<Time>" + laneItem.Time.ToString() + "</Time>\r\n";
			data += laneTabs + "\t<Duration>" + laneItem.Duration.ToString() + "</Duration>\r\n";
			data += laneTabs + "</Item>\r\n";
		}

		data += lanesTabs + "</Lane>\r\n";

		return data;
	}

	public void FromXML(TinyXmlReader xml)
	{
		if (xml.tagName != "Lane")
			return;

		while(xml.Read("Lane"))
		{
			if ((xml.tagType == TinyXmlReader.TagType.OPENING) && (xml.tagName == "Item"))
			{

				float time = 0.0f;
				float duration = 0.0f;

				while(xml.Read("Item"))
				{
					if (xml.tagType != TinyXmlReader.TagType.OPENING)
						continue;

					if (xml.tagName == "Time")
						time = float.Parse(xml.content.Trim());
					else if (xml.tagName == "Duration")
						duration = float.Parse(xml.content.Trim());
				}

				AddLaneItem(time, duration);
			}
		}
	}
}
