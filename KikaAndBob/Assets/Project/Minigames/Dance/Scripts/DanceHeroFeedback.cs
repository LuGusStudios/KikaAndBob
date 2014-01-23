using UnityEngine;
using System.Collections;
using SmoothMoves;

public class DanceHeroFeedback : LugusSingletonRuntime<DanceHeroFeedback> {

	public int scoreValue = 7;	// 0-4 = bad, 5 - 9 = neutral, 10 - 14 = good
	public float maxScoreModifier = 9.0f;

	public delegate void OnDisplayModifier();
	public OnDisplayModifier onDisplayModifier = null;

	public delegate void OnScoreRaised(DanceHeroLane lane);
	public OnScoreRaised onScoreRaised = null;

	public delegate void OnScoreLowered(DanceHeroLane lane);
	public OnScoreRaised onScoreLowered = null;
	
	protected int failCount = 0;
	protected int succesCount = 0;
	protected int scorePerHit = 10;
	protected int score = 0;
	protected float scoreModifier = 1;
	protected int scoreModifierStep = 1;
	protected int nextMessageIndex = 1;
	protected float scoreIncreaseStep = 0.2f;
	protected TextMesh scoreDisplay = null;
	protected TextMesh message = null;
	protected ILugusCoroutineHandle messageRoutine = null;
	protected string[] messages = new string[]
	{
		"Come on, Bob!",
		"You got it!",
		"Keep going!",
		"Great!",
		"Wow!",
		"Amazing!"
	};
	
	void Awake()
	{
		SetupLocal();
	}

	void Start()
	{
		SetupGlobal();
		scoreDisplay.text = "0";
		message.gameObject.SetActive(false);
	}

	public void SetupLocal()
	{
		Transform guiParent = GameObject.Find("GUI").transform;

		if (scoreDisplay == null)
			scoreDisplay = guiParent.FindChild("ScoreDisplay").GetComponent<TextMesh>();
		if (scoreDisplay == null)
			Debug.LogError("No score display found in scene.");

		if (message == null)
			message = guiParent.FindChild("Message").gameObject.GetComponent<TextMesh>();
		if (message == null)
			Debug.LogError("No message game object found in scene.");
	}

	public void SetupGlobal()
	{
	}

	public float GetScoreModifier()
	{
		return scoreModifier;
	}

	public int GetScore()
	{
		return score;
	}

	public void UpdateScore(bool succes, DanceHeroLane lane, int amount = 1)
	{
		int scoreAdd = 0;

		if (succes)
		{
			scoreValue += amount;
			succesCount += amount;

			scoreModifier += scoreIncreaseStep;
			scoreModifier = Mathf.Clamp(scoreModifier, 1, maxScoreModifier);
			scoreAdd = Mathf.RoundToInt((float)scorePerHit * scoreModifier);
			score += scoreAdd;
			DisplayScoreGainAtLane(lane, scoreAdd);

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
		else
		{
			// only show a "modifier lost" message if some has been built up - otherwise it shows up all the time
			bool showChange = false;
			if (scoreModifier > 2)
			{
				showChange = true;
			}

			if (showChange)
				DisplayMessage("OUCH!");

			scoreValue -= amount;
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

		scoreValue = Mathf.Clamp(scoreValue, 0, 14);

		Debug.Log("Updating score to : Failcount: " + failCount + " . Succes count: " + succesCount + ".");

		scoreDisplay.text = score.ToString();

		if (succes)
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

	protected void DisplayScoreGainAtLane(DanceHeroLane lane, int gain)
	{
		if (lane.scoreDisplay == null)
		{
			Debug.LogError("Missing score display object on lane: " + lane.name);
			return;
		}

		GameObject scoreDisplay = (GameObject)Instantiate(lane.scoreDisplay.gameObject);
		scoreDisplay.transform.parent = lane.transform;
		scoreDisplay.transform.position = lane.actionPoint.transform.position + new Vector3(0, 0, -1);
		scoreDisplay.GetComponent<TextMesh>().text = gain.ToString();

		scoreDisplay.MoveTo(scoreDisplay.transform.position + new Vector3(0, 2, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		scoreDisplay.transform.localScale = Vector3.zero;

		scoreDisplay.ScaleTo(Vector3.one).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();

		Destroy(scoreDisplay, 0.5f);
	}



	public void DisplayMessage(string messageText)
	{
		message.text = messageText;

		if (messageRoutine != null && messageRoutine.Running)
		{
			messageRoutine.StopRoutine();
		}

		messageRoutine = LugusCoroutines.use.StartRoutine(MessageRoutine());
	}

	protected IEnumerator MessageRoutine()
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

		yield return new WaitForSeconds(0.5f);


		message.gameObject.ScaleTo(Vector3.zero).EaseType(iTween.EaseType.linear).IsLocal(true).Time(0.5f).Execute();
		while (message.color.a > 0)
		{
			message.color = message.color.a(message.color.a - 2f * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
	}
}
