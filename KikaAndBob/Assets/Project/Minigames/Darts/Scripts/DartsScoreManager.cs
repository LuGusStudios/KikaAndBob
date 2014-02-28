using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsScoreManager : LugusSingletonExisting<DartsScoreManager> 
{
	public GameObject scoreTextPrefab = null;
	public int totalScore = 0;

	protected List<IDartsHitable> hitStreak = new List<IDartsHitable>();
	protected float streakTimer = 0.0f;
	protected float streakTimeOut = 1.5f;
	protected TextMesh streakCounter = null;
	protected int minStreakLength = 3;


	public void Reset()
	{
		Debug.Log("Darts score reset.");
		streakTimer = 0.0f;
		ClearStreak();
	}

	protected void ApplyStreak()
	{
		if (hitStreak.Count < minStreakLength)	// a certain minimum for streak length
		{
			ClearStreak();
			return;
		}

		Debug.Log("Applying streak.");

		int score = hitStreak[hitStreak.Count - 1].group.score;

		float multiplier = 1 + ((float)hitStreak.Count * 0.1f);	// every item in the streak adds 10 %

		score = Mathf.FloorToInt(score * multiplier);
		
		if (score > 0)
		{
			Vector3 position = HUDManager.use.CounterLargeRight1.transform.position + new Vector3(0, -0.2f, 20);	// a little offset makes the score text align nicer

			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio("Blob01"));

			ScoreVisualizer.Score(KikaAndBob.CommodityType.Score, score).UseGUICamera(true).Position(position).Execute();  
		}

		ClearStreak();
	}

	public void ClearStreak()
	{
		hitStreak.Clear();
		//streakCounter.text = hitStreak.Count.ToString();

		HUDManager.use.CounterLargeRight1.SetValue(hitStreak.Count, false);
	}

	public void AddToStreak(IDartsHitable hitable) 
	{
		if (hitable == null)
		{
			Debug.LogError("DartsScoreManager: Hitable was null!");
			return;
		}

		if (hitStreak.Contains(hitable))	// we don't want duplicates
			return;

		streakTimer = 0.0f;
		hitStreak.Add(hitable);
		//streakCounter.text = hitStreak.Count.ToString();
		HUDManager.use.CounterLargeRight1.SetValue(hitStreak.Count, false);
	}

	public void AddScore( int score, Vector3 position)
	{
		if( score == 0 )
		{
			return;
		}
		
		//position += new Vector3( Random.Range(-2, 2), Random.Range(-2,2), 0);


		ScoreVisualizer.Score(KikaAndBob.CommodityType.Score, score).Color(Color.white).MinValue(0).Time(0.4f).EaseType(iTween.EaseType.linear).Position(position).Execute();

//		GameObject scoreText = (GameObject) GameObject.Instantiate( scoreTextPrefab );
//		scoreText.transform.position = position.z( 0.0f );
//		scoreText.GetComponent<TextMesh>().text = "" + score;
//		scoreText.GetComponent<TextMesh>().color = color;
//
//		scoreText.MoveTo( scoreText.transform.position + new Vector3(0, 2, 0) ).Time ( time ).Execute(); 
//		GameObject.Destroy(scoreText, time);

		totalScore += score;

		if (totalScore < 0)
			totalScore = 0;
	}

	public void SetupLocal()
	{
		if( scoreTextPrefab == null )
		{
			scoreTextPrefab = GameObject.Find("Score");
		}
		
		if( scoreTextPrefab == null )
		{
			Debug.LogError("DartsScoreManager: " + name + " : no ScoreTextPrefab found!");
		}
		
//		if (streakCounter == null)
//		{
//			GameObject streakCounterObject = GameObject.Find("StreakCounter");
//			
//			if (streakCounterObject != null)
//			{
//				streakCounter = streakCounterObject.GetComponent<TextMesh>();
//			}
//		}
//		
//		if (streakCounter == null)
//		{
//			Debug.LogError("DartsScoreManager: Missing streak counter text mesh!");
//		}
	}
	
	public void SetupGlobal()
	{
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
		if (streakTimer < streakTimeOut)
		{
			streakTimer += Time.deltaTime;
		}
		else
		{
			if (hitStreak.Count > 0)
			{
				ApplyStreak();
			}
		}
	}
}
