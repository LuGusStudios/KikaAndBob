using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerScoreManager : LugusSingletonRuntime<RunnerScoreManager>
{
	public TextMesh scoreText = null;
	public GameObject scoreTextPrefab = null;
	
	public int totalScore = 0;

	public void ProcessPickup(RunnerPickup pickup)
	{
		if( pickup.positive )
		{
			AddScore( 10, pickup.transform.position, 1.0f, LugusResources.use.Shared.GetAudio("Blob01"), Color.white);
		}
		else // negative
		{
			AddScore( -10, pickup.transform.position, 1.0f, LugusResources.use.Shared.GetAudio("Collide01"), Color.red);
		}
	}

	public void AddScore( int score, Vector3 position, float time, AudioClip sound, Color color )
	{
		if( score == 0 )
		{
			if( sound != null )
				LugusAudio.use.SFX().Play( sound );
			
			return;
		}
		
		//position += new Vector3( Random.Range(-2, 2), Random.Range(-2,2), 0);
		
		GameObject scoreText = (GameObject) GameObject.Instantiate( scoreTextPrefab );
		scoreText.transform.position = position.z( 0.0f );
		scoreText.GetComponent<TextMesh>().text = "" + score;
		scoreText.GetComponent<TextMesh>().color = color;
		
		scoreText.MoveTo( scoreText.transform.position + new Vector3(2, 2, 0) ).Time ( time ).Execute(); 
		GameObject.Destroy(scoreText, time);
		
		if( sound != null )
			LugusAudio.use.SFX().Play( sound );

		int oldScore = totalScore;

		int newScore = totalScore + score;
		if( newScore >= 0 )
			totalScore += score;
		else
			totalScore = 0;
		
		ShowTotalScore(oldScore);
	}

	public void ShowTotalScore(int oldScore)
	{
		scoreText.text = "" + totalScore;
	}
	
	public void SetupLocal()
	{
		if( scoreTextPrefab == null )
		{
			scoreTextPrefab = GameObject.Find("Score");
		}
		
		if( scoreTextPrefab == null )
		{
			Debug.LogError(name + " : no ScoreTextPrefab found!");
		}

		if( scoreText == null )
		{
			GameObject scoreTextGO = GameObject.Find ("Score");
			if( scoreTextGO != null )
				scoreText = scoreTextGO.GetComponent<TextMesh>();
		}

		if( scoreText == null )
		{
			Debug.LogError(name + " : no ScoreText found! found!");
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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
		
	}
}
