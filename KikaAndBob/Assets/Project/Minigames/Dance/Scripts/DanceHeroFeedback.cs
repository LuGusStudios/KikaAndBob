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
	protected float maxScoreModifier = 4;
	protected float scoreIncreaseStep = 0.1f;
	
	protected AudioClip laneHitSound = null;
	
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
		if (bobAnim == null)
			bobAnim = GameObject.Find("Bob").GetComponent<BoneAnimation>();
		if (bobAnim == null)
			Debug.LogError("No Bob found in scene.");
	}

	public void SetupGlobal()
	{
		bobAnim.Play("BobBalance_Idle", PlayMode.StopAll);

		if (laneHitSound == null)
			laneHitSound = LugusResources.use.GetAudio("Blob01");
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

			scoreModifier += (scoreIncreaseStep * maxScoreModifier);
			scoreModifier = Mathf.Clamp(scoreModifier, 1, maxScoreModifier);
			scoreAdd =  Mathf.RoundToInt((float)scorePerHit * scoreModifier);
			score += scoreAdd;
			DisplayScoreGainAtLane(lane, scoreAdd);
		}
		else
		{
			scoreValue -= amount;
			failCount += amount;

			scoreModifier = 1;
		}

		scoreValue = Mathf.Clamp(scoreValue, 0, 14);

		Debug.Log("Updating score to :" + scoreValue + ". Failcount: " + failCount + " . Succes count: " + succesCount + ".");

		ChangeBobAnimation();
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
		// blend win from 0 - 1 for values 20-30
		if (scoreValue >= 10)
		{
			float animWeight = Mathf.Lerp(0, 1, scoreValue - 10 / 4);
			bobAnim.Blend(animationWin, animWeight);
			bobAnim.Blend(animationIdle, 1 - animWeight);
			bobAnim.Blend(animationStruggle, 0);
		}
		// between 10 - 20, idle anim
		else if (scoreValue >= 5)
		{
			bobAnim.Blend(animationWin, 0);
			bobAnim.Blend(animationIdle, 1);
			bobAnim.Blend(animationStruggle, 0);
		
		}
		// blend win from 1 - 0 for values 0-10
		else
		{
			float animWeight = Mathf.Lerp(1, 0, scoreValue / 4);
			bobAnim.Blend(animationWin, 0);
			bobAnim.Blend(animationIdle, 1 - animWeight);
			bobAnim.Blend(animationStruggle, animWeight);
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
