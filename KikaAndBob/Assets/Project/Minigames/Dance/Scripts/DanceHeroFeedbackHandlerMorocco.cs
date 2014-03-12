using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DanceHeroFeedbackHandlerMorocco : MonoBehaviour 
{
	protected BoneAnimation bobAnim = null;
	protected DanceHeroFeedback feedback = null;
	protected GameObject modifierDisplayPrefab = null;
	protected Transform poofParent = null;
	protected ParticleSystem poof = null;
	protected GameObject trail = null;
	protected List<Transform> sculpturePrefabs = new List<Transform>();
	protected List<Transform> sculpturesOnScreen = new List<Transform>();
	protected int sculptureIndex = 0;
	protected int previousBatchScore = 0;
	protected Transform tillySculpturePrefab = null;
	protected Transform tillySculpture = null;


	protected DanceHeroLane currentLane = null;
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start() 
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
//		feedback.onScoreRaised += OnScoreRaised;
//		feedback.onScoreLowered += OnScoreLowered;
		feedback.onButtonPress += OnButtonPress;
		DanceHeroLevel.use.onLevelStarted += OnLevelStarted;
		DanceHeroLevel.use.onLevelRestart += OnLevelRestart;
		DanceHeroLevel.use.onLevelFinished += OnLevelFinished;
		
		Transform guiParent = GameObject.Find("GUI_Debug").transform;

		modifierDisplayPrefab = guiParent.FindChild("ModifierDisplay").gameObject;
		if (modifierDisplayPrefab == null)
			Debug.LogError("No modifier display found in scene.");

		bobAnim = GameObject.Find("Bob").GetComponent<BoneAnimation>();
		if (bobAnim == null)
			Debug.LogError("No Bob found in scene.");

		poofParent = GameObject.Find("Poof").transform;
		if (poofParent == null)
			Debug.LogError("No 'poof' game object found.");

		poof = poofParent.GetComponentInChildren<ParticleSystem>();
		if (poof == null)
			Debug.LogError("No 'poof' particle system found.");

		trail = GameObject.Find("Trail");
		if (trail == null)
			Debug.LogError("No trail game object find.");

		GameObject sculpturesParent = GameObject.Find("Sculptures");

		tillySculpturePrefab = sculpturesParent.transform.FindChild("1");

		foreach(Transform t in sculpturesParent.transform)
		{
			if (t.name == "1")	// 1 is the Tilly sculpture, which is always added first
				continue;

			sculpturePrefabs.Add(t);
		}
	}
	
	public void SetupGlobal()
	{
	}
	
	public void OnDisplayModifier()
	{
		HUDManager.use.CounterSmallLeft1.SetValue(Mathf.FloorToInt(feedback.GetScoreModifier()), false);

		modifierDisplayPrefab.GetComponent<TextMesh>().text = "X" + Mathf.FloorToInt(feedback.GetScoreModifier()).ToString();

		GameObject modifierDisplay = (GameObject)Instantiate(modifierDisplayPrefab);
		modifierDisplay.transform.position = bobAnim.transform.position + new Vector3(0, 2, -1);
		modifierDisplay.MoveTo(modifierDisplay.transform.position + new Vector3(0, 3, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		Destroy(modifierDisplay, 0.5f);
	}

	protected void OnButtonPress(DanceHeroLane lane)
	{
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("Blob01"));
		
		if (currentLane == lane)
			return;

		ParticleSystem oldClayParticles = null;

		if (currentLane != null)
		{
			oldClayParticles = currentLane.transform.FindChild("ClayParticles").GetComponent<ParticleSystem>();
			if (oldClayParticles != null)
			{
				oldClayParticles.Stop();
			}
			else
			{
				Debug.LogError("Could not find clay particles!");
			}
		}

		Vector3 originalPosition = bobAnim.transform.position;

		currentLane = lane;
		poofParent.transform.position = bobAnim.transform.position;
		bobAnim.transform.position = lane.transform.FindChild("Stool/SitPosition").position + new Vector3(0, 0, -1);

		Vector3 newPosition = bobAnim.transform.position;

		poof.Play();

		iTween.Stop(trail);
		trail.transform.position = originalPosition;
		trail.MoveTo(newPosition).Speed(400.0f).Execute();

		ParticleSystem newClayParticles = null;

		if (currentLane != null)
		{
			newClayParticles = currentLane.transform.FindChild("ClayParticles").GetComponent<ParticleSystem>();
			if (newClayParticles != null)
			{
				newClayParticles.Play();
			}
			else
			{
				Debug.LogError("Could not find clay particles!");
			}
		}
	}

	protected void OnLevelStarted()
	{
		HUDManager.use.RepositionPauseButton(KikaAndBob.ScreenAnchor.BottomRight, KikaAndBob.ScreenAnchor.BottomRight);
		HUDManager.use.PauseButton.gameObject.SetActive(true);

		HUDManager.use.CounterLargeBottomLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterLargeBottomLeft1.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.CounterLargeBottomLeft1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterLargeBottomLeft1.SetValue(0);

		HUDManager.use.CounterSmallLeft1.gameObject.SetActive(true);
		HUDManager.use.CounterSmallLeft1.commodity = KikaAndBob.CommodityType.Custom;
		HUDManager.use.CounterSmallLeft1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.CounterSmallLeft1.prefix = "X";
		HUDManager.use.CounterSmallLeft1.SetValue(1);

		// we move this counter around a bit so we don't have to create another GUI element
		HUDManager.use.CounterSmallLeft1.transform.position += new Vector3(0, -12.4f, 0);


		HUDManager.use.ProgressBarCenter.gameObject.SetActive(true);
		HUDManager.use.ProgressBarCenter.commodity = KikaAndBob.CommodityType.Time;
		HUDManager.use.ProgressBarCenter.SetTimer(DanceHeroLevel.use.GetTotalLevelDuration());
		// we move this progress bar around a bit so we don't have to create another GUI element: align it with the score counter
		HUDManager.use.ProgressBarCenter.transform.position = HUDManager.use.ProgressBarCenter.transform.position.y(HUDManager.use.CounterLargeBottomLeft1.transform.position.y);

		sculptureIndex = 0;
			
		sculpturesOnScreen.Clear();

		GameObject tillySculptureClone = (GameObject)Instantiate(tillySculpturePrefab.gameObject);
		sculpturesOnScreen.Add(tillySculptureClone.transform);

		tillySculpture = tillySculptureClone.transform;

		List<Transform> alreadyPicked = new List<Transform>();	// this list just prevents randomly picking the same object twice

		for (int i = 0; i < 2; i++) 
		{
			Transform randomItem = null;
			int counter = 0;

			while(randomItem == null || alreadyPicked.Contains(randomItem))
			{
				randomItem = sculpturePrefabs[Random.Range(0, sculpturePrefabs.Count)];

				if (counter >= 20)
				{
					break;
				}

				counter++;
			}

			alreadyPicked.Add(randomItem);

			GameObject newItem = (GameObject)Instantiate(randomItem.gameObject);

			sculpturesOnScreen.Add(newItem.transform);
		}

		sculpturesOnScreen.Shuffle();

		for (int i = 0; i <  DanceHeroLevel.use.lanes.Count; i++) 
		{
			if ( i >= sculpturesOnScreen.Count)
				break;

			sculpturesOnScreen[i].transform.position = DanceHeroLevel.use.lanes[i].transform.FindChild("SculpturePosition").position;
		}

		UpdateSculptures(sculptureIndex, true);
	}

	protected void OnLevelRestart()
	{
		LugusCoroutines.use.StartRoutine(PauseRoutine());
	}

	protected IEnumerator PauseRoutine()
	{
		Debug.Log("DanceHeroFeedbackHandlerMorocco: Started break.");
		DanceHeroLevel.use.SetGameRunning(false);

		yield return new WaitForSeconds(0.5f); // delay this just a bit, otherwise the last keypress might still turn on particles after they're turned on below

		if (DanceHeroFeedback.use.GetScore() - previousBatchScore >= DanceHeroLevel.use.GetTargetBatchScore())
		{
			if (sculptureIndex < 4)
			{
				sculptureIndex ++;
				
				sculptureIndex =
					Mathf.Clamp(sculptureIndex, 4 - (DanceHeroLevel.use.GetLevelRepeatAmount() - 1), 4);	// skip steps as a function of how many times the level is repeated
																											// e.g. if we don't repeat the level, we'll immediately see the final scultpture stageultpture stage

				AudioClip clip = LugusResources.use.Shared.GetAudio("CrowdAah");
				
				if (clip != LugusResources.use.errorAudio)
				{
					LugusAudio.use.Music().Play(clip);
				}

				bobAnim.Play("BobSculpting_IdleHappy");
			}

			DanceHeroFeedback.use.DisplayMessage(LugusResources.use.GetText("dance.feedback.positive"));
		}
		else
		{
//			if (sculptureIndex > 0)
//				sculptureIndex--;
			
			DanceHeroFeedback.use.DisplayMessage(LugusResources.use.GetText("dance.feedback.negative"));

			bobAnim.Play("BobSculpting_IdleSad");
		}
		
		foreach (DanceHeroLane lane in DanceHeroLevel.use.lanes)
		{
			ParticleSystem particles = lane.transform.FindChild("ClayParticles").GetComponent<ParticleSystem>();
			if (particles != null)
			{
				particles.Stop();
			}
		}
		
		UpdateSculptures(sculptureIndex, true);
		previousBatchScore = DanceHeroFeedback.use.GetScore();

		yield return new WaitForSeconds(3.0f);

		DanceHeroFeedback.use.DisplayMessage(LugusResources.use.GetText("dance.feedback.repeat")); 

		yield return new WaitForSeconds(1.0f);

		// reset timer
		HUDManager.use.ProgressBarCenter.SetTimer(DanceHeroLevel.use.GetTotalLevelDuration());

		Debug.Log("DanceHeroFeedbackHandlerMorocco: Ended break.");
		DanceHeroLevel.use.SetGameRunning(true);
	}

	protected void OnLevelFinished()
	{
		LugusCoroutines.use.StartRoutine(FinishRoutine());
	}

	protected IEnumerator FinishRoutine()
	{
		yield return new WaitForSeconds(0.5f); // delay this just a bit, otherwise the last keypress might still turn on particles after they're turned on below
		
		DanceHeroFeedback.use.DisplayMessage(LugusResources.use.GetText("dance.feedback.result"));	
		
		if (DanceHeroFeedback.use.GetScore() - previousBatchScore >= DanceHeroLevel.use.GetTargetBatchScore())
		{
			if (sculptureIndex < 4)
			{
				sculptureIndex ++;
				
				sculptureIndex =
				Mathf.Clamp(sculptureIndex, 4 - (DanceHeroLevel.use.GetLevelRepeatAmount() - 1), 4);	// skip steps as a function of how many times the level is repeated
																										// e.g. if we don't repeat the level, we'll immediately see the final scultpture stageultpture stage
			}
		}
		
		foreach (DanceHeroLane lane in DanceHeroLevel.use.lanes)
		{
			ParticleSystem particles = lane.transform.FindChild("ClayParticles").GetComponent<ParticleSystem>();
			if (particles != null)
			{
				particles.Stop();
			}
			else
			{
				Debug.LogError("Could not find clay particles!");
			}
		}

		if (sculptureIndex == 4)	// if we finished sculptures, show the Tilly sculpture and show Bob is happy
		{
			bobAnim.Play("BobSculpting_IdleHappy");
		}
		else
		{
			bobAnim.Play("BobSculpting_IdleSad");
		}

		yield return new WaitForSeconds(1.0f);

		
		UpdateSculptures(sculptureIndex, false);
		previousBatchScore = DanceHeroFeedback.use.GetScore();

		bool success = false;

		if (sculptureIndex == 4)	// if we finished any sculptures, show the Tilly sculpture in the center of the screen
		{
			success = true;

			Vector3 startPosition = tillySculpture.transform.position;

			tillySculpture.gameObject.MoveTo(LugusUtil.ScreenCenter(tillySculpture.position) + new Vector3(0, 1, 0)).Time(1.0f).Execute();
			tillySculpture.gameObject.ScaleTo(tillySculpture.localScale * 3.0f).Time(1.0f).Execute();
	
			yield return new WaitForSeconds(1.0f);

			AudioClip clip = LugusResources.use.Shared.GetAudio("CrowdAah");
			
			if (clip != LugusResources.use.errorAudio)
			{
				LugusAudio.use.Music().Play(clip);
			}

			yield return new WaitForSeconds(0.5f);

			tillySculpture.gameObject.MoveTo(startPosition).Time(1.0f).Execute();
			tillySculpture.gameObject.ScaleTo(tillySculpture.localScale / 3.0f).Time(1.0f).Execute();

			yield return new WaitForSeconds(1.0f);
		}

		yield return new WaitForSeconds(2.0f);
		
		HUDManager.use.DisableAll();
		
		HUDManager.use.PauseButton.gameObject.SetActive(false);
		
		HUDManager.use.LevelEndScreen.Show(success);	// "success" depends on whether Bob made any finished sculptures (i.e. reached sculpture index 4). This shouldn't be very hard.
		HUDManager.use.LevelEndScreen.Counter1.gameObject.SetActive(true);
		HUDManager.use.LevelEndScreen.Counter1.commodity = KikaAndBob.CommodityType.Score;
		HUDManager.use.LevelEndScreen.Counter1.formatting = HUDCounter.Formatting.Int;
		HUDManager.use.LevelEndScreen.Counter1.SetValue(DanceHeroFeedback.use.GetScore());
	}

	protected void UpdateSculptures(int index, bool restart, float showTime = 5.0f)
	{
		LugusCoroutines.use.StartRoutine(ShowSculptureRoutine(index, restart, showTime));
	}

	protected IEnumerator ShowSculptureRoutine(int index, bool restart, float showTime)
	{
		// enable right index sculpture and scale it up
		foreach(Transform t in sculpturesOnScreen)
		{
			foreach(Transform child in t)
			{
				if (child.name == index.ToString())
				{
					child.gameObject.SetActive(true);
					child.gameObject.ScaleTo(child.transform.localScale * 1.3f).Time(0.5f).Execute();
				}
				else
				{
					child.gameObject.SetActive(false);
				}
			}
		}

		yield return new WaitForSeconds(0.5f);

		// scale it down again
		foreach(Transform t in sculpturesOnScreen)
		{
			foreach(Transform child in t)
			{
				if (child.name == index.ToString())
				{
					child.gameObject.ScaleTo(child.transform.localScale / 1.3f).Time(0.5f).Execute();
				}
			}
		}

		if (!restart)
			yield break;

		yield return new WaitForSeconds(showTime);

		// re-enable twist animation
		foreach(Transform t in sculpturesOnScreen)
		{
			foreach(Transform child in t)
			{
				if (child.name == "Twist")
				{
					child.gameObject.SetActive(true);
				}
				else
				{
					child.gameObject.SetActive(false);
				}
			}
		}

		bobAnim.Play("BobSculpting_Sculpting");
	}
}
