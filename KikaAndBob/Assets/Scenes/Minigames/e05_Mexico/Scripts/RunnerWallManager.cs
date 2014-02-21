using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerWallManager : MonoBehaviour 
{
	public DataRange wallXRange = new DataRange(12.5f, 8.5f);
	public List<RunnerWall> walls = new List<RunnerWall>();

	public List<Color> colors = new List<Color>(); 
	public SpriteRenderer[] caveBackgrounds;

	public RunnerManagerDefault manager = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		walls.AddRange( GameObject.FindObjectsOfType<RunnerWall>() );

		if( walls.Count != 4 )
		{
			Debug.LogError(transform.Path () + " : There should be 4 walls in this level. We can only find " + walls.Count);
		}


		colors.Add( new Color(255 / 255.0f, 255/ 255.0f, 255/ 255.0f) );
		colors.Add( new Color(181/ 255.0f, 155/ 255.0f, 255/ 255.0f) );
		colors.Add( new Color(155/ 255.0f, 254/ 255.0f, 255/ 255.0f) ); 
		colors.Add( new Color(105/ 255.0f, 255/ 255.0f, 181/ 255.0f) ); 
		colors.Add( new Color(30/ 255.0f,  140/ 255.0f, 86/ 255.0f) );

		if( caveBackgrounds.Length != 2 )
		{
			Debug.LogError(transform.Path () + " : Please manually assign the 2 Base cave backgrounds!");
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		
		manager = RunnerManager.use;
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
		if( !manager.GameRunning )
			return;

		float distance = manager.TraveledDistance;
		DataRange distanceRange = new DataRange( 0, manager.targetDistance);
		float distancePercentage = distanceRange.PercentageInInterval( distance );

		DataRange invertedXRange = new DataRange( wallXRange.from * -1.0f, wallXRange.to * -1.0f );

		int colorIndex = Mathf.FloorToInt( (distancePercentage * 100) / 25) + 1;
		colorIndex = Mathf.Min(colorIndex, colors.Count - 1);
		colorIndex = Mathf.Max(1, colorIndex);

		DataRange colorRange = new DataRange( (colorIndex - 1) * 25, colorIndex * 25 );
		float colorPercentage = colorRange.PercentageInInterval( (distancePercentage * 100.0f) );

		Color color = Color.Lerp( colors[ colorIndex - 1 ], colors[colorIndex], colorPercentage );

		//Debug.LogError("COLOR INDEX " + colorIndex + " from " + distancePercentage + " = " + color + " etc " + colorPercentage);

		foreach( RunnerWall wall in walls )
		{
			wall.GetComponent<SpriteRenderer>().color = color;

			if( wall.transform.localPosition.x < 0 ) // left walls
			{
				wall.transform.localPosition = wall.transform.localPosition.x ( invertedXRange.ValueFromPercentage(distancePercentage) );
			}
			else // right walls
			{
				wall.transform.localPosition = wall.transform.localPosition.x ( wallXRange.ValueFromPercentage(distancePercentage) );
			}
		}

		foreach( SpriteRenderer background in caveBackgrounds )
		{
			background.color = color;
		}
	}
}
