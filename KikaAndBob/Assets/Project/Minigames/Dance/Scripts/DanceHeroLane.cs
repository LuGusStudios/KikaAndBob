using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;


public class DanceHeroLane : MonoBehaviour 
{
	public List<DanceHeroLaneItem> items = new List<DanceHeroLaneItem>();

	public Transform actionPoint = null;
	public string attackAnimation = null;
	public string idleAnimation = null;

	public KikaAndBob.LaneItemActionType defaultActionType = KikaAndBob.LaneItemActionType.NONE;

	// public GameObject character = null;

	protected float totalDelay = 0.0f;
	protected BoneAnimation characterAnim = null;

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

		items.Add( new DanceHeroLaneItem(this, delay, type, duration) );

	}

	public void AddItem(float delay, float duration = DanceHeroLaneItem.singleDuration )
	{
		AddItem( delay, defaultActionType, duration );
		//items.Add( new DanceHeroLaneItem(this, delay, defaultActionType, duration) );
	}

	public void Begin()
	{
		LugusCoroutines.use.StartRoutine( LaneRoutine() );
		characterAnim.Play(idleAnimation);
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


			// only play a new fight anim if the previous one isn't still playing
			if (!characterAnim.IsPlaying(attackAnimation))
			{
				characterAnim.PlayQueued(attackAnimation, QueueMode.PlayNow, PlayMode.StopAll);
				characterAnim.PlayQueued(idleAnimation, QueueMode.CompleteOthers);
			}

			++currentItemIndex;
		}
	}

	protected void SpawnItem(DanceHeroLaneItem item)
	{
		DanceHeroLaneItemRenderer.Create( item );
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

		if( characterAnim == null )
		{
			characterAnim = transform.FindChild("Character").GetComponent<BoneAnimation>();
		}
		
		if( characterAnim == null )
		{
			Debug.LogError(name + " : no character known for this lane!");
		}

		if (string.IsNullOrEmpty(attackAnimation))
		{
			Debug.LogError(name + "Attack animation name is not entered.");
		}

		if(string.IsNullOrEmpty(idleAnimation))
		{
			idleAnimation = characterAnim.GetComponent<DefaultBoneAnimation>().clipName;
		}

		if(string.IsNullOrEmpty(idleAnimation))
		{
			Debug.LogError(name + "Idle animation name is not entered or default bone animation component is missing.");
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
}
