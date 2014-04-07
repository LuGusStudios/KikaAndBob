using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DanceHeroFeedback : LugusSingletonRuntime<DanceHeroFeedback> {

	//public int scoreValue = 7;	// 0-4 = bad, 5 - 9 = neutral, 10 - 14 = good
	public float maxScoreModifier = 9.0f;

	// Use this if the game needs to display a message the first frame
	public bool hideMessageAtStart = true;

	public delegate void OnDisplayModifier();
	public OnDisplayModifier onDisplayModifier = null;

	public delegate void OnLaneUsed(DanceHeroLane lane);
	public OnLaneUsed onScoreRaised = null;
	public OnLaneUsed onScoreLowered = null;
	public OnLaneUsed onButtonPress = null;
	
	protected int failCount = 0;
	protected int succesCount = 0;
	protected int scorePerHit = 10;
	protected int score = 0;
	protected float scoreModifier = 1;
	protected int scoreModifierStep = 1;
	protected int nextMessageIndex = 1;
	protected float scoreIncreaseStep = 0.2f;
	protected TextMesh message = null;
	protected ILugusCoroutineHandle messageRoutine = null;
	protected List<string> messages = new List<string>();

//	protected string[] messages = new string[]
//	{
//		"Come on, Bob!",
//		"You got it!",
//		"Keep going!",
//		"Great!",
//		"Wow!",
//		"Amazing!"
//	};
	protected string missedMessage = "OUCH!";

	public enum ScoreType
	{
		NONE,
		PRESS_CORRECT,
		PRESS_MISSED,
		PRESS_INCORRECT
	}
	
	void Awake()
	{
		SetupLocal();
	}

	void Start()
	{
		SetupGlobal();
	}

	public void SetupLocal()
	{
		Transform guiParent = GameObject.Find("GUI_Debug").transform;

		if (message == null)
			message = guiParent.FindChild("Message").gameObject.GetComponent<TextMesh>();
		if (message == null)
			Debug.LogError("No message game object found in scene.");
	}

	public void SetupGlobal()
	{
		if (hideMessageAtStart)
		{
			message.gameObject.SetActive(false);
		}

		// read language specific feedback text

		for (int i = 1; i <= 6; i++) 
		{
			messages.Add(LugusResources.use.GetText("dance.feedback.good."+ i.ToString()));
		}

		missedMessage = LugusResources.use.GetText("dance.feedback.bad.1");
	}

	public void ResetGUI()
	{
//		HUDManager.use.CounterLargeLeft1.gameObject.SetActive(true);
//		HUDManager.use.CounterLargeLeft1.commodity = KikaAndBob.CommodityType.Score;
//		HUDManager.use.CounterLargeLeft1.formatting = HUDCounter.Formatting.Int;
//		HUDManager.use.CounterLargeLeft1.SetValue(0);
	}

	public float GetScoreModifier()
	{
		return scoreModifier;
	}

	public int GetScore()
	{
		return score;
	}

	public void UpdateScore(ScoreType type, DanceHeroLane lane, int amount = 1)
	{
		int scoreAdd = 0;

		if (type == ScoreType.PRESS_CORRECT)
		{
			//scoreValue += amount;
			succesCount += amount;

			scoreModifier += scoreIncreaseStep;
			scoreModifier = Mathf.Clamp(scoreModifier, 1, maxScoreModifier);
			scoreAdd = Mathf.RoundToInt((float)scorePerHit * scoreModifier);
			score += scoreAdd;
		//	DisplayScoreGainAtLane(lane, scoreAdd, true);
			ScoreVisualizer.Score(KikaAndBob.CommodityType.Score, scoreAdd).Time(2.0f).Position(lane.actionPoint.transform.position).Color(lane.laneColor).Execute();

			if (scoreModifier >= (maxScoreModifier / 6) * nextMessageIndex)
			{
				DisplayMessage(messages[nextMessageIndex - 1]);
				nextMessageIndex++;
			}
		
			// we only display the modifier above Bob's head when it has crossed a certain threshold, i.e. scoreModifierStep
			if (scoreModifier >= scoreModifierStep && scoreModifier < maxScoreModifier + 1)
			{
				if (onDisplayModifier != null)
					onDisplayModifier();

				scoreModifierStep++;
			}
		}
		else if (type == ScoreType.PRESS_MISSED)
		{
			// only show a "modifier lost" message if some has been built up - otherwise it shows up all the time
			bool showChange = false;
			if (scoreModifier > 2)
			{
				showChange = true;
			}

			if (showChange)
				DisplayMessage(missedMessage);

			failCount += amount;

			scoreModifier = 1;
			scoreModifierStep = 1;
			nextMessageIndex = 1;

			if (showChange)
			{
				if (onDisplayModifier != null)
					onDisplayModifier();
			}
		}
		else if (type == ScoreType.PRESS_INCORRECT)
		{
			Debug.Log("Pressed incorrectly!");

			// incorrect pressing does not lower modifier, but does subtract penalty score

			if (score <= 0)
			{
				return;
			}

			scoreAdd = (int)((scorePerHit * maxScoreModifier) * 0.5f) * -1;
			//DisplayScoreGainAtLane(lane, scoreAdd, false);
			ScoreVisualizer.Score(KikaAndBob.CommodityType.Score, scoreAdd).Time(2.0f).Position(lane.actionPoint.transform.position).Color(Color.red).MinValue(0).Execute();

			score += scoreAdd;	// subtract half of maximum score

			if (score < 0)
			{
				score = 0;
			}
		}
		else
		{
			Debug.LogError("DanceHeroFeedback: Unknown score change type.");
		}


	//	Debug.Log("Updating score to : Failcount: " + failCount + " . Succes count: " + succesCount + ".");

	

		//HUDManager.use.CounterLargeLeft1.SetValue(score);

		if (type == ScoreType.PRESS_CORRECT)
		{
			if (onScoreRaised != null)
				onScoreRaised(lane);
		}
		else
		{
			if (onScoreLowered != null)
				onScoreLowered(lane);
		}
	}

	protected void DisplayScoreGainAtLane(DanceHeroLane lane, int gain, bool positive)
	{
		if (lane.scoreDisplay == null)
		{
			Debug.LogError("Missing score display object on lane: " + lane.name);
			return;
		}

		GameObject scoreDisplay = (GameObject)Instantiate(lane.scoreDisplay.gameObject);
		scoreDisplay.transform.parent = lane.transform;
		scoreDisplay.transform.position = lane.actionPoint.transform.position.z(-1.0f);
		TextMesh textMesh = scoreDisplay.GetComponent<TextMesh>();
		textMesh.text = gain.ToString();

		if (positive)
		{
			scoreDisplay.MoveTo(scoreDisplay.transform.position + new Vector3(0, 2.0f, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		}
		else
		{
			textMesh.color = Color.red;
			scoreDisplay.MoveTo(scoreDisplay.transform.position + new Vector3(0, -2.0f, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		}

		scoreDisplay.transform.localScale = Vector3.zero;
		scoreDisplay.ScaleTo(Vector3.one).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();

		Destroy(scoreDisplay, 0.5f);
	}

	public void DisplayMessage(string messageText, float duration = 0.5f)
	{
		message.text = messageText;

		TextMeshWrapperHelper.use.WrapText(message, 15, false);

		if (messageRoutine != null && messageRoutine.Running)
		{
			messageRoutine.StopRoutine();
		}

		messageRoutine = LugusCoroutines.use.StartRoutine(MessageRoutine(duration));
	}

	protected IEnumerator MessageRoutine(float duration)
	{
		if (!message.gameObject.activeSelf)
		{
			message.gameObject.SetActive(true);
		}

		message.transform.localScale = Vector3.zero;
		message.gameObject.ScaleTo(Vector3.one * 0.25f).EaseType(iTween.EaseType.easeOutBack).IsLocal(true).Time(0.5f).Execute();

		message.color = message.color.a(0);
		while (message.color.a < 1)
		{
			message.color = message.color.a(message.color.a + 2f * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(duration);


		message.gameObject.ScaleTo(Vector3.zero).EaseType(iTween.EaseType.linear).IsLocal(true).Time(0.5f).Execute();
		while (message.color.a > 0)
		{
			message.color = message.color.a(message.color.a - 2f * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
	}
}
