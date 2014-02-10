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
	protected List<Transform> sculptures = new List<Transform>();
	protected int sculptureIndex = 0;
	protected int previousBatchScore = 0;

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
		feedback.onScoreRaised += OnScoreRaised;
		feedback.onScoreLowered += OnScoreLowered;
		DanceHeroLevel.use.onLevelStarted += OnLevelStarted;
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

		GameObject sculptures = GameObject.Find("Sculptures");

		foreach(Transform t in sculptures.transform)
		{
			sculpturePrefabs.Add(t);
		}
	}
	
	public void SetupGlobal()
	{
	}
	
	public void OnDisplayModifier()
	{
		modifierDisplayPrefab.GetComponent<TextMesh>().text = "X" + Mathf.FloorToInt(feedback.GetScoreModifier()).ToString();
		GameObject modifierDisplay = (GameObject)Instantiate(modifierDisplayPrefab);
		modifierDisplay.transform.position = bobAnim.transform.position + new Vector3(0, 2, -1);
		modifierDisplay.MoveTo(modifierDisplay.transform.position + new Vector3(0, 3, 0)).EaseType(iTween.EaseType.easeOutQuad).Time(0.5f).Execute();
		Destroy(modifierDisplay, 0.5f);
	}

	protected void OnScoreRaised(DanceHeroLane lane)
	{
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

	protected void OnScoreLowered(DanceHeroLane lane)
	{
	
	}

	protected void OnLevelStarted()
	{
		sculptures = new List<Transform>();
		List<Transform> sculpturePrefabsSelect = new List<Transform>(sculpturePrefabs);
		sculptureIndex = 0;

		for (int i = 0; i < 3; i++) 
		{
			Transform randomItem = sculpturePrefabsSelect[Random.Range(0, sculpturePrefabsSelect.Count)];
			sculpturePrefabsSelect.Remove(randomItem);

			GameObject newItem = (GameObject)Instantiate(randomItem.gameObject);

			newItem.transform.position = DanceHeroLevel.use.lanes[i].transform.FindChild("SculpturePosition").position;

			sculptures.Add(newItem.transform);
		}

		UpdateSculptures(sculptureIndex);
	}

	protected void OnLevelFinished()
	{
		if (DanceHeroFeedback.use.GetScore() - previousBatchScore >= 300)
		{
			if (sculptureIndex < 4)
				sculptureIndex++;

			DanceHeroFeedback.use.DisplayMessage("Great job!");
		}
		else
		{
			if (sculptureIndex > 0)
				sculptureIndex--;

			DanceHeroFeedback.use.DisplayMessage("Try again!");
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

		UpdateSculptures(sculptureIndex);
		previousBatchScore = DanceHeroFeedback.use.GetScore();
	}

	protected void UpdateSculptures(int index)
	{
		LugusCoroutines.use.StartRoutine(StartSpinRoutine(index));
	}

	protected IEnumerator StartSpinRoutine(int index)
	{

		// enable right index sculpture and scale it up
		foreach(Transform t in sculptures)
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
		foreach(Transform t in sculptures)
		{
			foreach(Transform child in t)
			{
				if (child.name == index.ToString())
				{
					child.gameObject.ScaleTo(child.transform.localScale / 1.3f).Time(0.5f).Execute();
				}
			}
		}

		yield return new WaitForSeconds(5.0f);

		// re-enable twist animation
		foreach(Transform t in sculptures)
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
	}
}
