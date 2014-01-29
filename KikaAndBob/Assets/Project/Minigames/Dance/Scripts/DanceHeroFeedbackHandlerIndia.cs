using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DanceHeroFeedbackHandlerIndia : MonoBehaviour 
{
	protected DanceHeroFeedback feedback = null;
	protected GameObject modifierDisplayPrefab = null;
	protected Transform flute = null;
	protected BoneAnimation snake = null;

	protected string snakeStage1 = "Snake_Stage1";
	protected string snakeStage2 = "Snake_Stage2";
	protected string snakeStage3 = "Snake_Stage3";

	protected Vector3 snakePosition1;
	protected Vector3 snakePosition2;
	protected Vector3 snakePosition3;
	protected ILugusCoroutineHandle flutePlayRoutine = null;


	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start() 
	{
		SetupGlobal();
		flute.gameObject.MoveTo(flute.position + new Vector3(0, 0.1f, 0)).Time(1f).EaseType(iTween.EaseType.easeInOutQuad).Looptype(iTween.LoopType.pingPong).Execute();
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
		
		Transform guiParent = GameObject.Find("GUI").transform;
		
		modifierDisplayPrefab = guiParent.FindChild("ModifierDisplay").gameObject;
		if (modifierDisplayPrefab == null)
			Debug.LogError("No modifier display found in scene.");

		flute = GameObject.Find("Flute").transform;
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
	}
	
	public void SetupGlobal()
	{
	}
	
	public void OnDisplayModifier()
	{
		GameObject modifierDisplay = (GameObject)Instantiate(modifierDisplayPrefab);
		modifierDisplay.transform.position = flute.transform.position + new Vector3(0, 6.0f, 1);
		modifierDisplay.MoveTo(modifierDisplay.transform.position + new Vector3(0, 3.0f, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		modifierDisplay.GetComponent<TextMesh>().text = "X" + Mathf.FloorToInt(feedback.GetScoreModifier()).ToString();
		Destroy(modifierDisplay, 0.5f);
	}
	
	protected void OnScoreRaised(DanceHeroLane lane)
	{
		ChangeSnakeAnim();
		AnimateFlute();
	}
	
	protected void OnScoreLowered(DanceHeroLane lane)
	{
		ChangeSnakeAnim();
		//AnimateFlute();
	}

	protected void AnimateFlute()
	{
		if (flutePlayRoutine != null && !flutePlayRoutine.Running)
		{
			flutePlayRoutine.StopRoutine();
			iTween.Stop(flute.gameObject);
		}

		flutePlayRoutine = LugusCoroutines.use.StartRoutine(AnimateFluteRoutine());
	}
	
	protected IEnumerator AnimateFluteRoutine()
	{
		flute.localScale = Vector3.one;
		flute.gameObject.ScaleTo(new Vector3(1.0f, 1.02f, 1.0f)).Time(0.25f).EaseType(iTween.EaseType.easeOutBack).Execute();
		yield return new WaitForSeconds(0.25f);
		flute.localScale = Vector3.one;
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
		snake.Play(snakeStage1);
	}
	
	protected void OnLevelFinished()
	{
	
	}
}
