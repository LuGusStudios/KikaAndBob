using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DanceHeroFeedbackHandlerChina : MonoBehaviour 
{
	protected BoneAnimation bobAnim = null;
	protected DanceHeroFeedback feedback = null;
	protected GameObject modifierDisplayPrefab = null;

	protected string animationIdle = "BobBalance_Idle";
	protected string animationStruggle = "BobBalance_Struggle";
	protected string animationWin = "BobBalance_win";

	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
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

		if (modifierDisplayPrefab == null)
			modifierDisplayPrefab = guiParent.FindChild("ModifierDisplay").gameObject;
		if (modifierDisplayPrefab == null)
			Debug.LogError("No modifier display found in scene.");

		if (bobAnim == null)
			bobAnim = GameObject.Find("Bob").GetComponent<BoneAnimation>();
		if (bobAnim == null)
			Debug.LogError("No Bob found in scene.");
	}

	public void SetupGlobal()
	{
		bobAnim.Play("BobBalance_Idle", PlayMode.StopAll);
	}

	public void OnDisplayModifier()
	{
		modifierDisplayPrefab.GetComponent<TextMesh>().text = "X" + Mathf.FloorToInt(feedback.GetScoreModifier()).ToString();
		GameObject modifierDisplay = (GameObject)Instantiate(modifierDisplayPrefab);
		modifierDisplay.transform.position = bobAnim.transform.position + new Vector3(0, 2, -1);
		modifierDisplay.MoveTo(modifierDisplay.transform.position + new Vector3(0, 3, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		Destroy(modifierDisplay, 0.5f);
	}

	protected void ChangeBobAnim()
	{
		float step = feedback.maxScoreModifier / 3;
		float scoreModifier = feedback.GetScoreModifier();
		
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

	protected void OnScoreLowered(DanceHeroLane lane)
	{
		ChangeBobAnim();
	}

	// blend three animations for value
	protected void OnScoreRaised(DanceHeroLane lane)
	{
		ChangeBobAnim();
	}

	protected void OnLevelStarted()
	{
		HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.Top, KikaAndBob.ScreenAnchor.Top);
		HUDManager.use.PauseButton.gameObject.SetActive(true);
	}

	protected void OnLevelFinished()
	{
		if (DanceHeroLevel.use.currentLevel < DanceHeroLevel.use.levels.Length - 1)	
		{
//			DanceHeroLevel.use.currentLevel++;
//			DanceHeroLevel.use.CreateLevel();
		}
	}
}
