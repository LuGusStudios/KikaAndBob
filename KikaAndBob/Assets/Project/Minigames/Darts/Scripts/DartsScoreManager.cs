using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsScoreManager : LugusSingletonRuntime<DartsScoreManager> 
{
	public GameObject scoreTextPrefab = null;
	
	public int totalScore = 0;

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
			Debug.LogError(name + " : no ScoreTextPrefab found!");
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
