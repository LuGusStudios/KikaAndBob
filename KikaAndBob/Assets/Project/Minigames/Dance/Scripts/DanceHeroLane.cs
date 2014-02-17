using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;


public class DanceHeroLane : MonoBehaviour 
{
	public List<DanceHeroLaneItem> items = new List<DanceHeroLaneItem>();
	public float speed = 4;
	public Transform actionPoint = null;
	public Transform scoreDisplay = null;

	public KikaAndBob.LaneItemActionType defaultActionType = KikaAndBob.LaneItemActionType.NONE;

	public Color laneColor = Color.white;
	
//	[HideInInspector]
//	public BoneAnimation characterAnim = null;
//	// public GameObject character = null;

	public delegate void OnItemSpawned(DanceHeroLaneItemRenderer laneItemRenderer);
	public OnItemSpawned onItemSpawned = null;

	public delegate void OnLaneBegin();
	public OnLaneBegin onLaneBegin = null;

	protected float totalDelay = 0.0f;
	protected ILugusCoroutineHandle laneRoutine = null;

	protected int currentLeadingItemIndex = 0;

	protected ILugusCoroutineHandle highlightRoutine = null;



	public void Hide()
	{
		this.gameObject.SetActive( false );
	}

	public void Show()
	{
		this.gameObject.SetActive( true );
	}

	public void AddItem(float delay, KikaAndBob.LaneItemActionType type, float duration = DanceHeroLaneItem.singleDuration)
	{
		if( DanceHeroLevel.use.mode == DanceHeroLevel.TimeProgressionMode.GLOBAL_CUMULATIVE )
		{
			// put the total delay for the full level up with the delay for this item
			DanceHeroLevel.use.cumulativeDelay += delay;
			
			// delays for the current item are always in relation to the previous item (Always "per lane" so to speak)
			delay = DanceHeroLevel.use.cumulativeDelay - this.totalDelay;
		}	

		Debug.Log (name + " : AddItem : " + DanceHeroLevel.use.mode + " @ " + DanceHeroLevel.use.cumulativeDelay + ", lane = " + this.totalDelay + " / " + delay);


		this.totalDelay += delay;

		// we also pass speed here, because that way it remains customizable if ever needed AND is accessible to DanceHeroLevel to calculate the total level length
		items.Add( new DanceHeroLaneItem(this, delay, type, speed, duration) );

	}

	public void AddItem(float delay, float duration = DanceHeroLaneItem.singleDuration )
	{
		AddItem( delay, defaultActionType, duration );
		//items.Add( new DanceHeroLaneItem(this, delay, defaultActionType, duration) );
	}

	public float GetLength()
	{
		return Vector2.Distance(transform.position.v2(), transform.FindChild("ActionPoint").position.v2());
	}

	public void Begin()
	{
		currentLeadingItemIndex = 0;

		if (laneRoutine != null && laneRoutine.Running)
		{
			laneRoutine.StopRoutine();
		}

		laneRoutine = LugusCoroutines.use.StartRoutine( LaneRoutine() );

		if (onLaneBegin != null)
			onLaneBegin();
	}

	protected IEnumerator LaneRoutine()
	{
		int currentItemIndex = 0;
		while( currentItemIndex < items.Count )
		{
			DanceHeroLaneItem item = items[currentItemIndex];

			//Debug.LogError(Time.frameCount + " Lane " + this.name + " waiting for " + item.delay + " seconds");
			yield return new WaitForSeconds( item.delay );
			
			//Debug.LogError(Time.frameCount + " Lane " + this.name + " waited for " + item.delay + " seconds");

			SpawnItem( item );

			++currentItemIndex;
		}
	}

	public float GetTotalDelay()
	{
		return totalDelay;
	}

	public float GetFullDuration()
	{
		float lastItemDuration = 0;

		if (items.Count > 0)
		{
			lastItemDuration = items[items.Count - 1].duration;
		}

		return 
			Vector2.Distance(transform.position.v2(), transform.FindChild("ActionPoint").position.v2()) / speed +
			GetTotalDelay() +
				lastItemDuration;
	}

	protected void SpawnItem(DanceHeroLaneItem item)
	{
		DanceHeroLaneItemRenderer itemRenderer = DanceHeroLaneItemRenderer.Create( item );

		if (onItemSpawned != null)
			onItemSpawned(itemRenderer);
	}

	public void SetupLocal()
	{
		if( actionPoint == null )
		{
			actionPoint = transform.FindChild("ActionPoint");
		}

		if( actionPoint == null )
		{
			Debug.LogError(name + " : no ActionPoint known for this lane!");
		}

		if( scoreDisplay == null )
		{
			scoreDisplay = transform.FindChild("ScoreDisplay");
		}
		
		if( scoreDisplay == null )
		{
			Debug.LogError(name + " : no Score Display found for this lane!");
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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
	
	}

	public void ClearLaneItems()
	{
		if (laneRoutine != null && laneRoutine.Running)
		{
			laneRoutine.StopRoutine();
		}

		for (int i = items.Count - 1; i >= 0; i--)
		{
			DanceHeroLaneItem item = items[i];

			// if the item already hade an associated LaneItemRenderer, destroy it
			if (item.laneItemRenderer == null)
				continue;

			for (int j = item.laneItemRenderer.actionPoints.Count - 1; j >= 0; j--)
			{
				Destroy(item.laneItemRenderer.actionPoints[j].gameObject);
			}

			Destroy(item.laneItemRenderer.gameObject);
		}

		items.Clear();

	}

	public void HighlightLaneNegative()
	{
		if (highlightRoutine != null && highlightRoutine.Running)
		{
			highlightRoutine.StopRoutine();
		}

		highlightRoutine = LugusCoroutines.use.StartRoutine(HighlightLaneNegativeRoutine());
	}

	protected IEnumerator HighlightLaneNegativeRoutine()
	{
		Transform highlight = actionPoint.FindChild("Highlight");

		highlight.renderer.material.color = Color.red;
		highlight.gameObject.SetActive(true);
		
		float alpha = 0;
		float effectTime = 0.35f;
		
		while(alpha < 1)
		{
			highlight.renderer.material.color = highlight.renderer.material.color.a(alpha);
			alpha += (1 / (effectTime * 0.5f)) * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		while(alpha > 0 )
		{
			highlight.renderer.material.color = highlight.renderer.material.color.a(alpha);
			alpha -= (1 / (effectTime * 0.5f)) * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		highlight.gameObject.SetActive(false);
	}

	public void HighLightLanePositive(bool permanent = false)
	{
		if (highlightRoutine != null && highlightRoutine.Running)
		{
			highlightRoutine.StopRoutine();
		}

		if (permanent)
		{
			highlightRoutine = LugusCoroutines.use.StartRoutine(HighlightLanePositivePermanentRoutine());
		}
		else
		{
			highlightRoutine = LugusCoroutines.use.StartRoutine(HighlightLanePositiveRoutine());
		}
	}

	public void StopHighlight()
	{
		if (highlightRoutine != null && highlightRoutine.Running)
		{
			highlightRoutine.StopRoutine();
		}

		GameObject highlight = actionPoint.FindChild("Highlight").gameObject;

		iTween.Stop(highlight);
		highlight.SetActive(false);
	}

	protected IEnumerator HighlightLanePositiveRoutine()
	{
		Transform highlight = actionPoint.FindChild("Highlight");

		highlight.renderer.material.color = Color.white;
		highlight.gameObject.SetActive(true);

		float alpha = 0;
		float effectTime = 0.5f;

		iTween.RotateBy(highlight.gameObject, iTween.Hash(
			"amount", new Vector3(0, 0, -0.5f),
			"time", effectTime,
			"easetype", iTween.EaseType.easeInOutQuad));
		
	//	LugusAudio.use.SFX().Play(laneHitSound);
		
		while(alpha < 1)
		{
			highlight.renderer.material.color = highlight.renderer.material.color.a(alpha);
			alpha += (1 / (effectTime * 0.5f)) * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		while(alpha > 0 )
		{
			highlight.renderer.material.color = highlight.renderer.material.color.a(alpha);
			alpha -= (1 / (effectTime * 0.5f)) * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		highlight.gameObject.SetActive(false);
	}

	protected IEnumerator HighlightLanePositivePermanentRoutine()
	{
		Transform highlight = actionPoint.FindChild("Highlight");
		
		highlight.renderer.material.color = Color.white;
		highlight.gameObject.SetActive(true);
		
		float alpha = 0.5f;
		float effectTime = 1f;
		
		iTween.RotateBy(highlight.gameObject, iTween.Hash(
			"amount", new Vector3(0, 0, -0.2f),
			"time", effectTime,
			"easetype", iTween.EaseType.easeInOutQuad,
			"looptype", iTween.LoopType.loop));
		
		//	LugusAudio.use.SFX().Play(laneHitSound);

		while (true)
		{
			while(alpha < 0.9f)
			{
				highlight.renderer.material.color = highlight.renderer.material.color.a(alpha);
				alpha += (1 / (effectTime * 0.5f)) * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			
			while(alpha > 0.5f )
			{
				highlight.renderer.material.color = highlight.renderer.material.color.a(alpha);
				alpha -= (1 / (effectTime * 0.5f)) * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
	}



	public DanceHeroLaneItem GetCurrentLeadingItem()
	{
		if (items == null || items.Count < 1 || currentLeadingItemIndex >= items.Count)
		{
			Debug.LogError("DanceHeroLane: currentLeadingItemIndex is out of bounds.");
			return null;
		}

		return items[currentLeadingItemIndex];
	}

	public void IncreaseLeadingLaneItem()
	{
		LugusCoroutines.use.StartRoutine(IncreaseLeadingLaneItemRoutine());
	}

	// this is stupid, but it makes stuff work
	// what happens is: if we increase the front most lane item and other lane item renderers still check 
	// for incorrect presses within the same frame, they will detect one, which means you always get a penalty
	// SO: instead do not set the next leading lane item until the next frame
	protected IEnumerator IncreaseLeadingLaneItemRoutine()
	{
		yield return new WaitForEndOfFrame();

		currentLeadingItemIndex++;
		
		if (currentLeadingItemIndex >= items.Count)
		{
			currentLeadingItemIndex = items.Count - 1;
			Debug.Log("Last lane item reached on lane: " + name);
		}
	}

	public void ParseLaneFromXML(TinyXmlReader parser)
	{
		// Example Lane xml:
		/**
		 *	<Lane>
		 *		<Item>
		 *			<Time>4.096</Time>
		 *			<Duration>0.256</Duration>
		 *		</Item>
		 *	</Lane>
		 **/

		// When parsing an item, its time is compared to the
		// previous item's time in this lane and a delay is set.
		// Items whose time is too close to the beginning of the music
		// are ignored and will not spawn in the game.

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) &&
			(parser.tagName != "Lane"))
		{
			Debug.LogError("Cannot start parsing the xml data.");
			return;
		}

		// Delay between the action point and spawning from the character
		float constDelay = Vector2.Distance(this.transform.position.v2(), this.actionPoint.position.v2()) / this.speed;
		float prevTime = 0.0f;

		// While still reading valid lane data
		int itemCount = 0;
		while (parser.Read("Lane"))
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "Item"))
			{
				float time = 0.0f;
				float duration = 0.0f;

				// Parse the lane item
				while (parser.Read("Item"))
				{
					if (parser.tagType != TinyXmlReader.TagType.OPENING)
						continue;

					if (parser.tagName == "Time")
						time = float.Parse(parser.content.Trim());
					else if (parser.tagName == "Duration")
						duration = float.Parse(parser.content.Trim());
				}

				float delay = time - prevTime;

				// If its the first lane item, then it receives the initial constant delay
				if (itemCount == 0)
				{
					delay -= constDelay;
				}

				if (delay > 0f)
				{
					prevTime = time;
					AddItem(delay, duration);
					++itemCount;
				}
			}
		}
	}
}
