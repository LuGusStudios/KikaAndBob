using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DanceHeroFeedbackHandlerIndia : MonoBehaviour 
{
	protected DanceHeroFeedback feedback = null;
	protected GameObject modifierDisplayPrefab = null;
	protected Transform flute = null;
	protected Transform fluteParent = null;
	protected BoneAnimation snake = null;

	protected string snakeStage1 = "Snake_Stage1";
	protected string snakeStage2 = "Snake_Stage2";
	protected string snakeStage3 = "Snake_Stage3";

	protected Vector3 snakePosition1;
	protected Vector3 snakePosition2;
	protected Vector3 snakePosition3;
	protected ILugusCoroutineHandle flutePlayRoutine = null;
	protected float smallFluteAnimDuration = 0.3f;
	protected float largeFluteAnimDuration = 0.5f;
	protected List<Transform> coinPrefabs = new List<Transform>();
	protected List<Transform> coinsDropped = new List<Transform>();
	protected List<BoneAnimation> legs = new List<BoneAnimation>();

	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start() 
	{
		SetupGlobal();
		fluteParent.gameObject.MoveTo(flute.position + new Vector3(0, 0.1f, 0)).Time(1f).EaseType(iTween.EaseType.easeInOutQuad).Looptype(iTween.LoopType.pingPong).Execute();
		LugusCoroutines.use.StartRoutine(RemoveCoinsRoutine());
	}
	
	public void SetupLocal()
	{
		feedback = GetComponent<DanceHeroFeedback>();
		
		if (feedback == null)
		{
			Debug.LogError(name + ": Missing feedback script."); 
		}
		
		feedback.onDisplayModifier += OnDisplayModifier;
		feedback.onScoreRaised += OnScoreRaised;
		feedback.onScoreLowered += OnScoreLowered;
		DanceHeroLevel.use.onLevelStarted += OnLevelStarted;
		DanceHeroLevel.use.onLevelFinished += OnLevelFinished;
		
		Transform guiParent = GameObject.Find("GUI_Debug").transform;
		
		modifierDisplayPrefab = guiParent.FindChild("ModifierDisplay").gameObject;
		if (modifierDisplayPrefab == null)
			Debug.LogError("No modifier display found in scene.");

		fluteParent = GameObject.Find("FluteParent").transform;
		if (fluteParent == null)
		{
			Debug.LogError("No flute parent found!");
		}

		flute = fluteParent.transform.FindChild("Flute");
		if (flute == null)
		{
			Debug.LogError("No flute found!");
		}

		snake = GameObject.Find("Snake").GetComponent<BoneAnimation>();
		if (snake == null)
		{
			Debug.LogError("No snake found!");
		}

		snakePosition1 = snake.transform.position;
		snakePosition2 = snakePosition1 + new Vector3(0, 1, 0);
		snakePosition3 = snakePosition1 + new Vector3(0, -1, 0);

		GameObject money = GameObject.Find("Money");

		foreach(Transform t in money.transform)
		{
			coinPrefabs.Add(t);
		}

		Transform legParent = GameObject.Find("Legs").transform;

		for (int i = 0; i < legParent.childCount; i++) 
		{
			BoneAnimation ba = legParent.GetChild(i).GetComponent<BoneAnimation>();
			legs.Add(ba);
			//ba.Play(ba.GetComponent<DefaultBoneAnimation>().clipName);
		}

		legs.Shuffle();	// randomize legs for variety
	}
	
	public void SetupGlobal()
	{
	}

	public void OnDisplayModifier()
	{
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("Blob01"));

		modifierDisplayPrefab.GetComponent<TextMesh>().text = "X" + Mathf.FloorToInt(feedback.GetScoreModifier()).ToString();
		GameObject modifierDisplay = (GameObject)Instantiate(modifierDisplayPrefab);
		modifierDisplay.transform.position = flute.transform.position + new Vector3(0, 17.0f, 1);
		modifierDisplay.MoveTo(modifierDisplay.transform.position + new Vector3(0, 4.0f, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(1f).Execute();
		modifierDisplay.ScaleTo(modifierDisplay.transform.localScale * 2.0f).Time(1f).Execute();

		Destroy(modifierDisplay, 0.5f);

		AnimateFlute(true);

		UpdateLegs();
	}
	
	protected void UpdateLegs()
	{
		int modifier = Mathf.FloorToInt(DanceHeroFeedback.use.GetScoreModifier());

		foreach (BoneAnimation item in legs) 
		{
			item.gameObject.SetActive(false);			
		}

		for (int i = 0; i < modifier; i++) 
		{
			if (i > legs.Count)
				break;

			legs[i].gameObject.SetActive(true);
		}
	}
	
	protected void OnScoreRaised(DanceHeroLane lane)
	{
		ChangeSnakeAnim();
		AnimateFlute(false);
		DropMoney();
	}
	
	protected void OnScoreLowered(DanceHeroLane lane)
	{
		ChangeSnakeAnim();
		//AnimateFlute();
	}

	protected void AnimateFlute(bool big)
	{
		if (flutePlayRoutine != null && flutePlayRoutine.Running)
		{
			return;
		}

		iTween.Stop(flute.gameObject);

		if (!big)
			flutePlayRoutine = LugusCoroutines.use.StartRoutine(AnimateFluteRoutineSmall());
		else
			flutePlayRoutine = LugusCoroutines.use.StartRoutine(AnimateFluteRoutineBig());
	}
	
	protected IEnumerator AnimateFluteRoutineSmall()
	{
		float halfTime = smallFluteAnimDuration * 0.5f;

		flute.localScale = Vector3.one;

		flute.gameObject.ScaleTo(new Vector3(0.98f, 1.01f, 1.0f)).Time(halfTime).EaseType(iTween.EaseType.linear).Execute();
		yield return new WaitForSeconds(halfTime);

		flute.gameObject.ScaleTo(Vector3.one).Time(halfTime).EaseType(iTween.EaseType.linear).Execute();
		yield return new WaitForSeconds(halfTime);
	}

	protected IEnumerator AnimateFluteRoutineBig()
	{
		float halfTime = largeFluteAnimDuration * 0.5f;

		flute.localScale = Vector3.one;

		flute.gameObject.ScaleTo(new Vector3(0.96f, 1.05f, 1.0f)).Time(halfTime).EaseType(iTween.EaseType.linear).Execute();
		yield return new WaitForSeconds(halfTime);

		flute.gameObject.ScaleTo(Vector3.one).Time(halfTime).EaseType(iTween.EaseType.linear).Execute();
		yield return new WaitForSeconds(halfTime);
	}

	protected void DropMoney()
	{
		int modifier = Mathf.FloorToInt(DanceHeroFeedback.use.GetScoreModifier());

		for (int i = 0; i < modifier; i++) 
		{
			GameObject droppedCoin = (GameObject)Instantiate(coinPrefabs[Random.Range(0, coinPrefabs.Count)].gameObject);
			
			droppedCoin.transform.Translate(new Vector3(Random.Range(-3.0f, 3.0f), 0, 0)); // horizontally vary the starting point of the coin
			
			Vector3 destination = 	droppedCoin.transform.position + 
				new Vector3(0, Random.Range(-8, -11), 0);
			
			droppedCoin.MoveTo(destination).EaseType(iTween.EaseType.easeOutBounce).Time(1).Delay(Random.Range(0.0f, 1.5f)).Execute();	// random delay so coins don't drop simultaneously
			
			coinsDropped.Add(droppedCoin.transform);
		}
	}

	protected IEnumerator RemoveCoinsRoutine()
	{
		while(true)
		{
			if (coinsDropped.Count >= 10)
			{
				Transform oldestCoin = coinsDropped[0];
				coinsDropped.Remove(oldestCoin);
				GameObject.Destroy(oldestCoin.gameObject);
				yield return new WaitForSeconds(1.0f);
			}
			else
				yield return new WaitForEndOfFrame();

		}
	}

	protected void ChangeSnakeAnim()
	{

		float step = feedback.maxScoreModifier / 3.0f;

		float scoreModifier = feedback.GetScoreModifier();

		// between 2/3rd - full , win anim
		if (scoreModifier >= step * 2)
		{
			float animWeight = Mathf.Clamp((scoreModifier - (step * 2)) / step, 0.0f, 1.0f);

			snake.transform.position = snakePosition3;

			snake.Blend(snakeStage3, animWeight);
			snake.Blend(snakeStage2, 1 - animWeight);
			snake.Blend(snakeStage1, 0);
		}
		// between 1/3rd - 2/3rd , idle anim
		 else if (scoreModifier >= step)
		{
			float animWeight = Mathf.Clamp((scoreModifier - step) / step, 0.0f, 1.0f);

			snake.transform.position = snakePosition2;

			snake.Blend(snakeStage3, 0);
			snake.Blend(snakeStage2, 1);
			snake.Blend(snakeStage1, 0);
			
		}
		// blend win from 1 - 0 for values 0-10
		else
		{
			float animWeight = Mathf.Clamp(scoreModifier / step, 0.0f, 1.0f);

			snake.transform.position = snakePosition1;

			snake.Blend(snakeStage3, 0);
			snake.Blend(snakeStage2, 1 - animWeight);
			snake.Blend(snakeStage1, animWeight);
		}
	}
	
	protected void OnLevelStarted()
	{
		HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.TopRight, KikaAndBob.ScreenAnchor.TopRight);
		HUDManager.use.PauseButton.gameObject.SetActive(true);
		
		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.CounterLargeLeft1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterLargeLeft1.SetValue(0);


		snake.Play(snakeStage1);

		foreach (BoneAnimation item in legs) 
		{
			item.gameObject.SetActive(false);			
		}

		legs[0].gameObject.SetActive(true);
	}
	
	protected void OnLevelFinished()
	{
		HUDManager.use.DisableAll();
		
		HUDManager.use.PauseButton.gameObject.SetActive(false);
		
		HUDManager.use.LevelEndScreen.Show(true);
		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(DanceHeroFeedback.use.GetScore());
	}
}
