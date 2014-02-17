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
		if( !RunnerManager.use.GameRunning )
			return;


		Vector3 position;// = Vector3.zero;
		//if( pickup != null )
		//	position = pickup.transform.position;
		//else
			position = ( (MonoBehaviour) RunnerCharacterController.use).transform.position;

		float scoreAmount = 10.0f;
		if( pickup != null )
			scoreAmount = pickup.scoreAmount;

		KikaAndBob.CommodityType commodity = KikaAndBob.CommodityType.NONE;

		if( pickup.positive )
		{ 
			//AddScore( 10, position, 1.0f, LugusResources.use.Shared.GetAudio("Blob01"), Color.white);

			if( pickup != null && pickup.commodityType != KikaAndBob.CommodityType.NONE )
				commodity = pickup.commodityType;
			else
				commodity = KikaAndBob.CommodityType.Feather;


			RunnerManager.use.AddPickup( Mathf.RoundToInt(scoreAmount) );
			ScoreVisualizer.Score(commodity, scoreAmount).Time (1.0f).Position( position ).Audio("Blob01").Color(Color.white).Execute();
		}
		else // negative
		{
			/*
			if( pickup != null && pickup.commodityType != KikaAndBob.CommodityType.NONE )
				commodity = pickup.commodityType;
			else
				commodity = KikaAndBob.CommodityType.Time;
			*/

			if( RunnerManager.use.gameType == KikaAndBob.RunnerGameType.Endless )
			{
				commodity = KikaAndBob.CommodityType.Life;
				scoreAmount = 1.0f;

				RunnerManager.use.AddLives( - (Mathf.RoundToInt(scoreAmount)) );
			}
			else
			{
				commodity = KikaAndBob.CommodityType.Time;

				// if time penalty, scoreAmount should not be negative but positive
				RunnerManager.use.AddTime( scoreAmount );
				scoreAmount *= -1;
			}

			ScoreVisualizer.Score(commodity, -scoreAmount).Time (1.0f).Position( position ).Audio("Collide01").Color(Color.red).Execute();
		}


	}

	/*
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
	*/
	
	public void SetupLocal()
	{
		/*
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
		*/
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
