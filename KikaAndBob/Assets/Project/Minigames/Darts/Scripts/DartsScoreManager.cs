using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsScoreManager : LugusSingletonExisting<DartsScoreManager> 
{
	public GameObject scoreTextPrefab = null;
	public int totalScore = 0;

	protected List<IDartsHitable> hitStreak = new List<IDartsHitable>();
	protected float streakTimer = 0;
	protected float streakTimeOut = 0.5f;
	protected TextMesh streakCounter = null;


	public void Reset()
	{
		streakTimer = 0;
		streakTimeOut = 0;
		hitStreak.Clear();
		streakCounter.text = hitStreak.Count.ToString();
	}

	protected void ApplyStreak()
	{
		Debug.Log("Applying streak.");
		ClearStreak();
		streakCounter.text = hitStreak.Count.ToString();
	}

	public void ClearStreak()
	{
		hitStreak.Clear();
	}

	public void AddToStreak(IDartsHitable hitable)
	{
		streakTimer = 0.0f;
		hitStreak.Add(hitable);
		print (streakCounter);
		streakCounter.text = hitStreak.Count.ToString();
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
	}
	
	public void SetupGlobal()
	{
		if( scoreTextPrefab == null )
		{
			scoreTextPrefab = GameObject.Find("Score");
		}
		
		if( scoreTextPrefab == null )
		{
			Debug.LogError(name + " : no ScoreTextPrefab found!");
		}

		if (streakCounter == null)
		{
			GameObject streakCounterObject = GameObject.Find("StreakCounter");

			if (streakCounterObject != null)
			{
				streakCounter = streakCounterObject.GetComponent<TextMesh>();
			}
		}

		if (streakCounter == null)
		{
			Debug.LogError("DartsScoreManager: Missing streak counter text mesh!");
		}
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
