using UnityEngine;
using System.Collections;
using SmoothMoves;

public class DanceHeroFeedback : LugusSingletonRuntime<DanceHeroFeedback> {

	public int scoreValue = 7;	// 0-4 = bad, 5 - 9 = neutral, 10 - 14 = good
	protected BoneAnimation bobAnim = null;
	protected string animationIdle = "BobBalance_Idle";
	protected string animationStruggle = "BobBalance_Struggle";
	protected string animationWin = "BobBalance_win";
	protected int failCount = 0;
	protected int succesCount = 0;
	protected int scorePerHit = 10;
	protected int score = 0;
	protected float scoreModifier = 1;
	protected int scoreModifierStep = 1;
	protected int nextMessageIndex = 1;
	protected float maxScoreModifier = 9;
	protected float scoreIncreaseStep = 0.2f;
	protected TextMesh scoreDisplay = null;
	protected TextMesh message = null;
	protected GameObject modifierDisplayPrefab = null;
	protected AudioClip laneHitSound = null;
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
		if (bobAnim == null)
			bobAnim = GameObject.Find("Bob").GetComponent<BoneAnimation>();
		if (bobAnim == null)
			Debug.LogError("No Bob found in scene.");

		Transform guiParent = GameObject.Find("GUI").transform;

		if (scoreDisplay == null)
			scoreDisplay = guiParent.FindChild("ScoreDisplay").GetComponent<TextMesh>();
		if (scoreDisplay == null)
			Debug.LogError("No score display found in scene.");

		if (modifierDisplayPrefab == null)
			modifierDisplayPrefab = guiParent.FindChild("ModifierDisplay").gameObject;
		if (modifierDisplayPrefab == null)
			Debug.LogError("No modifier display found in scene.");

		if (message == null)
			message = guiParent.FindChild("Message").gameObject.GetComponent<TextMesh>();
		if (message == null)
			Debug.LogError("No message game object found in scene.");


	}

	public void SetupGlobal()
	{
		bobAnim.Play("BobBalance_Idle", PlayMode.StopAll);

		if (laneHitSound == null)
			laneHitSound = LugusResources.use.Shared.GetAudio("Blob01");
		if (laneHitSound == null)
			Debug.Log("Lane hit sound is missing!");
	}

	public void UpdateScore(bool succes, DanceHeroLane lane, int amount = 1)
	{
		int scoreAdd = 0;

		if (succes)
		{
			scoreValue += amount;
			succesCount += amount;

			scoreModifier += (scoreIncreaseStep);
			scoreModifier = Mathf.Clamp(scoreModifier, 1, maxScoreModifier);
			scoreAdd =  Mathf.RoundToInt((float)scorePerHit * scoreModifier);
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
				DisplayModifierAboveBob();
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
				DisplayModifierAboveBob();
		}

		scoreValue = Mathf.Clamp(scoreValue, 0, 14);

		Debug.Log("Updating score to : Failcount: " + failCount + " . Succes count: " + succesCount + ".");

		scoreDisplay.text = score.ToString();

		ChangeBobAnimation();
	}

	protected void DisplayModifierAboveBob()
	{
		GameObject modifierDisplay = (GameObject)Instantiate(modifierDisplayPrefab);
		modifierDisplay.transform.position = bobAnim.transform.position + new Vector3(0, 2, -1);
		modifierDisplay.MoveTo(modifierDisplay.transform.position + new Vector3(0, 3, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		modifierDisplay.GetComponent<TextMesh>().text = "X" + Mathf.FloorToInt(scoreModifier).ToString();
		Destroy(modifierDisplay, 0.5f);
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

	// blend three animations for value
	protected void ChangeBobAnimation()
	{
		float step = maxScoreModifier / 3;

		// between 2/3rd - full , win anim
		if (scoreModifier >= step * 2)
		{
			float animWeight = Mathf.Lerp(0, 1, scoreModifier - 2 / 2);
			bobAnim.Blend(animationWin, animWeight);
			bobAnim.Blend(animationIdle, 1 - animWeight);
			bobAnim.Blend(animationStruggle, 0);
		}
		// between 1/3rd - 2/3rd , idle anim
		else if (scoreModifier >= step)
		{
			bobAnim.Blend(animationWin, 0);
			bobAnim.Blend(animationIdle, 1);
			bobAnim.Blend(animationStruggle, 0);
		
		}
		// blend win from 1 - 0 for values 0-10
		else
		{
			float animWeight = Mathf.Lerp(1, 0, scoreModifier - 1);
			bobAnim.Blend(animationWin, 0);
			bobAnim.Blend(animationIdle, 1 - animWeight);
			bobAnim.Blend(animationStruggle, animWeight);
		}


//		// blend win from 0 - 1 for values 20-30
//		if (scoreValue >= 10)
//		{
//			float animWeight = Mathf.Lerp(0, 1, scoreValue - 10 / 4);
//			bobAnim.Blend(animationWin, animWeight);
//			bobAnim.Blend(animationIdle, 1 - animWeight);
//			bobAnim.Blend(animationStruggle, 0);
//		}
//		// between 10 - 20, idle anim
//		else if (scoreValue >= 5)
//		{
//			bobAnim.Blend(animationWin, 0);
//			bobAnim.Blend(animationIdle, 1);
//			bobAnim.Blend(animationStruggle, 0);
//		
//		}
//		// blend win from 1 - 0 for values 0-10
//		else
//		{
//			float animWeight = Mathf.Lerp(1, 0, scoreValue / 4);
//			bobAnim.Blend(animationWin, 0);
//			bobAnim.Blend(animationIdle, 1 - animWeight);
//			bobAnim.Blend(animationStruggle, animWeight);
//		}
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

		LugusAudio.use.SFX().Play(laneHitSound);
		
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
