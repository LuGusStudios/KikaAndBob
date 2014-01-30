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
	
//	[HideInInspector]
//	public BoneAnimation characterAnim = null;
//	// public GameObject character = null;

	public delegate void OnItemSpawned(DanceHeroLaneItemRenderer laneItemRenderer);
	public OnItemSpawned onItemSpawned = null;

	public delegate void OnLaneBegin();
	public OnLaneBegin onLaneBegin = null;

	protected float totalDelay = 0.0f;


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
		LugusCoroutines.use.StartRoutine( LaneRoutine() );

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
		return 
			Vector2.Distance(transform.position.v2(), transform.FindChild("ActionPoint").position.v2()) / speed +
			GetTotalDelay() +
				items[items.Count - 1].duration;
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

	public void HighLightLane(Transform actionPoint)
	{
		LugusCoroutines.use.StartRoutine(LaneHighlight(actionPoint));
	}

	IEnumerator LaneHighlight(Transform actionPoint)
	{
		Transform highlight = actionPoint.FindChild("Highlight");
		
		float alpha = 0;
		float effectTime = 0.5f;
		
		highlight.gameObject.SetActive(true);
		
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
}
