using UnityEngine;
using System.Collections;
using SmoothMoves;

public class DanceHeroFeedback : LugusSingletonRuntime<DanceHeroFeedback> {

	public int scoreValue = 15;	// 0-10 = bad, 10 - 20 = neutral, 20 - 30 = good
	protected BoneAnimation bobAnim = null;

	protected string animationIdle = "BobBalance_Idle";
	protected string animationStruggle = "BobBalance_Struggle";
	protected string animationWin = "BobBalance_win";
	
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
	}

	public void SetupGlobal()
	{
		bobAnim.Play("BobBalance_Idle", PlayMode.StopAll);
	}

	public void UpdateScore(int amount)
	{
		Debug.Log("Updating score to :" + scoreValue);
		scoreValue += amount;
		scoreValue = Mathf.Clamp(scoreValue, 0, 30);

		ChangeAnimation();
	}

	protected void ChangeAnimation()
	{
		if (scoreValue >= 20)
		{
			float animWeight = Mathf.Lerp(0, 1, scoreValue - 20 / 10);
			bobAnim.Blend(animationWin, animWeight);
			bobAnim.Blend(animationIdle, 1 - animWeight);
			bobAnim.Blend(animationStruggle, 0);

//			if (!bobAnim.IsPlaying(animationWin))
//				bobAnim.PlayQueued(animationWin, QueueMode.PlayNow, PlayMode.StopAll );
		}
		else if (scoreValue >= 10)
		{
//			if (!bobAnim.IsPlaying(animationIdle))
//				bobAnim.PlayQueued(animationIdle, QueueMode.PlayNow, PlayMode.StopAll );
			bobAnim.Blend(animationWin, 0);
			bobAnim.Blend(animationIdle, 1);
			bobAnim.Blend(animationStruggle, 0);
		
		}
		else
		{
//			if (!bobAnim.IsPlaying(animationStruggle))
//				bobAnim.PlayQueued(animationStruggle, QueueMode.PlayNow, PlayMode.StopAll );
			float animWeight = Mathf.Lerp(1, 0, scoreValue / 9);
			bobAnim.Blend(animationWin, 0);
			bobAnim.Blend(animationIdle, 1 - animWeight);
			bobAnim.Blend(animationStruggle, animWeight);
		}
	}

}
