using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class LaneItemRenderer : MonoBehaviour {

	public static float Speed = 1.0f;

	public static LaneItemRenderer Create (Lane lane, LaneItem item)
	{
		LaneItemRenderer renderer = GameObject.Instantiate(LaneManager.use.laneItemRendererPrefab) as LaneItemRenderer;
		renderer.Lane = lane;
		renderer.Item = item;

		Transform transform = renderer.transform;
		transform.parent = lane.transform.parent;
		transform.position = new Vector3(0.0f, 0.0f, -0.1f);

		// Scale the renderer according to the scale of the lane so it fits between te lines
		transform.localScale = new Vector3(lane.transform.localScale.y, lane.transform.localScale.y, 1.0f);

		renderer.TimeToPosition();

		return renderer;
	}

	public Lane Lane
	{
		get
		{
			return _lane;
		}
		set
		{
			_lane = value;
		}
	}
	public LaneItem Item
	{
		get
		{
			return _item;
		}
		set
		{
			_item = value;
		}
	}
	public bool HighLight
	{
		get
		{
			return _highLight;
		}
		set
		{
			_highLight = value;
			_changed = true;
		}
	}
	public float Time
	{
		get
		{
			return Item.Time;
		}
		set
		{
			Item.Time = value;
		}
	}
	public float Duration
	{
		get
		{
			return Item.Duration;
		}
		set
		{

			if (Item.Duration != value)
				_changed = true;

			Item.Duration = value;
		}
	}

	#region Protected
	protected Lane _lane = null;
	protected LaneItem _item = null;

	protected SpriteRenderer _leftSprite = null;
	protected SpriteRenderer _midSprite = null;
	protected SpriteRenderer _rightSprite = null;

	protected SpriteRenderer _leftSpriteHL = null;
	protected SpriteRenderer _midSpriteHL = null;
	protected SpriteRenderer _rightSpriteHL = null;

	protected bool _highLight = false;
	protected bool _changed = false;

	protected float _xPosOffset = 0.0f;
	#endregion

	void Start()
	{
		_midSprite = this.transform.FindChild("hitcircle_mid").GetComponent<SpriteRenderer>();
		_leftSprite = this.transform.FindChild("hitcircle_left").GetComponent<SpriteRenderer>();
		_rightSprite = this.transform.FindChild("hitcircle_right").GetComponent<SpriteRenderer>();

		_midSpriteHL = this.transform.FindChild("hitcircle_mid_highlight").GetComponent<SpriteRenderer>();
		_leftSpriteHL = this.transform.FindChild("hitcircle_left_highlight").GetComponent<SpriteRenderer>();
		_rightSpriteHL = this.transform.FindChild("hitcircle_right_highlight").GetComponent<SpriteRenderer>();

		this.gameObject.SetActive(true);

		SetHighlight();
		DetermineLength();
	}

	void Update()
	{
		if (_lane == null)
		{
			Debug.LogError("LaneItemRenderer.Update(): No lane is assigned to this hit circle.");
			return;
		}

		if (_item == null)
		{
			Debug.LogError("LaneItemRenderer.Update(): No lane item is assigned to this hit circle.");
			return;
		}

		if (_changed)
		{
			SetHighlight();
			DetermineLength();
			_changed = false;
		}

		TimeToPosition();
	}

	void TimeToPosition()
	{
		// Determine the hit circle's global position in its lane
		float ypos = _lane.transform.position.y;
		float hlxpos = HitLine.use.transform.position.x;
		float diffTime = AudioPlayer.use.SeekTime - _item.Time;
		float xpos = hlxpos + LaneItemRenderer.Speed * diffTime;

		this.transform.position = new Vector3(xpos, ypos, -0.1f);
	}

	void PositionToTime()
	{
		float seekTime = AudioPlayer.use.SeekTime;
		float hlxpos = HitLine.use.transform.position.x;
		float diffPos = hlxpos - transform.position.x;
		float time = seekTime + diffPos / LaneItemRenderer.Speed;

		_item.Time = time;
	}

	void OnMouseDown()
	{
		LaneManager.use.SetCurrentLaneItem(this);

		float xpos = Camera.main.WorldToScreenPoint(this.transform.position).x;
		_xPosOffset = xpos - Input.mousePosition.x;
	}

	void OnMouseDrag()
	{
		Vector3 mousePos = Input.mousePosition;
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos + new Vector3(_xPosOffset, 0.0f, 0.0f));

		float ypos = transform.position.y;
		float zpos = transform.position.z;
		float xpos = worldPos.x;

		this.transform.position = new Vector3(xpos, ypos, zpos);
		PositionToTime();
		SortItem();
	}

	void SortItem()
	{
		int currentIndex = _lane.LaneItems.IndexOf(_item);

		// Check the previous index's time and swap them if necessary
		if (currentIndex > 0)
		{
			LaneItem previtem = _lane.LaneItems[currentIndex - 1];
			if (_item.Time < previtem.Time)
			{
				_lane.LaneItems[currentIndex] = previtem;
				_lane.LaneItems[currentIndex - 1] = _item;
			}
		}

		// Check the next index
		if (currentIndex < (_lane.LaneItems.Count - 1))
		{
			LaneItem nextitem = _lane.LaneItems[currentIndex + 1];
			if (_item.Time > nextitem.Time)
			{
				_lane.LaneItems[currentIndex] = nextitem;
				_lane.LaneItems[currentIndex + 1] = _item;
			}
		}
	}

	void DetermineLength()
	{
		if (Item.Type == DanceEditor.LaneItemType.SINGLE)
		{
			_leftSprite.transform.position = this.transform.position;
			_midSprite.transform.position = this.transform.position;
			_rightSprite.transform.position = this.transform.position;

			_leftSpriteHL.transform.position = this.transform.position;
			_midSpriteHL.transform.position = this.transform.position;
			_rightSpriteHL.transform.position = this.transform.position;

			_midSprite.transform.localScale = Vector3.one;
			_midSpriteHL.transform.localScale = Vector3.one;
			_midSprite.gameObject.SetActive(false);
			_midSpriteHL.gameObject.SetActive(false);

			BoxCollider2D collider = GetComponent<BoxCollider2D>();
			collider.center = Vector2.zero;
			collider.size = Vector2.one;
		}
		else if (Item.Type == DanceEditor.LaneItemType.STREAK)
		{
			float leftxpos = this.transform.position.x - (LaneItemRenderer.Speed * _item.Duration) + LaneItem.singleDuration;
			float midxpos = (this.transform.position.x + leftxpos) / 2.0f;

			_leftSprite.transform.position = new Vector3(leftxpos, this.transform.position.y, this.transform.position.z);
			_midSprite.transform.position = new Vector3(midxpos, this.transform.position.y, this.transform.position.z);			
			_rightSprite.transform.position = this.transform.position;

			_leftSpriteHL.transform.position = _leftSprite.transform.position;
			_midSpriteHL.transform.position = _midSprite.transform.position;
			_rightSpriteHL.transform.position = _rightSprite.transform.position;

			_midSprite.transform.localScale = new Vector3((this.transform.position.x - leftxpos) / this.transform.localScale.x, 1.0f, 1.0f);
			_midSpriteHL.transform.localScale = _midSprite.transform.localScale;

			if (_highLight)
			{
				_midSprite.gameObject.SetActive(false);
				_midSpriteHL.gameObject.SetActive(true);
			}
			else
			{
				_midSprite.gameObject.SetActive(true);
				_midSpriteHL.gameObject.SetActive(false);
			}
			
			BoxCollider2D collider = GetComponent<BoxCollider2D>();
			collider.center = new Vector2((leftxpos - this.transform.position.x), 0.0f);
			collider.size = _midSprite.transform.localScale + new Vector3(1.0f, 0.0f, 0.0f);
		} 
	}

	void SetHighlight()
	{
		if (_highLight)
		{
			_midSprite.gameObject.SetActive(false);
			_leftSprite.gameObject.SetActive(false);
			_rightSprite.gameObject.SetActive(false);

			_midSpriteHL.gameObject.SetActive(true);
			_leftSpriteHL.gameObject.SetActive(true);
			_rightSpriteHL.gameObject.SetActive(true);
		}
		else
		{
			_midSprite.gameObject.SetActive(true);
			_leftSprite.gameObject.SetActive(true);
			_rightSprite.gameObject.SetActive(true);

			_midSpriteHL.gameObject.SetActive(false);
			_leftSpriteHL.gameObject.SetActive(false);
			_rightSpriteHL.gameObject.SetActive(false);
		}
		
	}
}
