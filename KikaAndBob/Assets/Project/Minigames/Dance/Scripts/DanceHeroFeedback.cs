using UnityEngine;
using System.Collections;
using SmoothMoves;

public class DanceHeroFeedback : LugusSingletonRuntime<DanceHeroFeedback> {

	public int scoreValue = 15;	// 0-10 = bad, 10 - 20 = neutral, 20 - 30 = good
	protected BoneAnimation bobAnim = null;
	protected string animationIdle = "BobBalance_Idle";
	protected string animationStruggle = "BobBalance_Struggle";
	protected string animationWin = "BobBalance_win";
	protected int failCount = 0;
	protected int succesCount = 0;


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

		laneHitSound = LugusResources.use.GetAudio("Blob01");
	}

	public void SetupGlobal()
	{
		bobAnim.Play("BobBalance_Idle", PlayMode.StopAll);
	}
	
	public void UpdateScore(bool succes)
	{
		UpdateScore(succes, 1);
	}

	public void UpdateScore(bool succes, int amount)
	{
		if (succes)
		{
			scoreValue += amount;
			succesCount += amount;
		}
		else
		{
			scoreValue -= amount;
			failCount += amount;
		}

		scoreValue = Mathf.Clamp(scoreValue, 0, 30);

		Debug.Log("Updating score to :" + scoreValue + ". Failcount: " + failCount + " . Succes count: " + succesCount + ".");

		ChangeAnimation();
	}

	// blend three animations for value
	protected void ChangeAnimation()
	{
		// blend win from 0 - 1 for values 20-30
		if (scoreValue >= 20)
		{
			float animWeight = Mathf.Lerp(0, 1, scoreValue - 20 / 10);
			bobAnim.Blend(animationWin, animWeight);
			bobAnim.Blend(animationIdle, 1 - animWeight);
			bobAnim.Blend(animationStruggle, 0);
		}
		// between 10 - 20, idle anim
		else if (scoreValue >= 10)
		{
			bobAnim.Blend(animationWin, 0);
			bobAnim.Blend(animationIdle, 1);
			bobAnim.Blend(animationStruggle, 0);
		
		}
		// blend win from 1 - 0 for values 0-10
		else
		{
			float animWeight = Mathf.Lerp(1, 0, scoreValue / 9);
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

		Debug.Log("Highlighting: " + actionPoint.name, highlight.gameObject);

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
	}
}
