using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HappinessVisualizer : MonoBehaviour 
{
	public GameObject happinessIconPrefab = null;

	//protected List<GameObject> icons = null;
	public GameObject icon = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( happinessIconPrefab == null )
		{
			Debug.LogError(transform.Path () + " : No happinessIconPrefab found!" );
		}
		else
		{
			icon = (GameObject) GameObject.Instantiate( happinessIconPrefab );
			icon.transform.parent = this.transform;
			icon.transform.localPosition = Vector3.zero;

			icon.SetActive( false );
		}

		//PrepareRenderers();
	}

	public void Visualize(float happiness)
	{
		// happiness goes from 0 to 10
		// divide in buckets of 2 wide for now

		icon.SetActive( true );

		int index = 4; // 10 is multi happy already!

		if( happiness < 10 )
		{
			DataRange happinessRange = new DataRange(0,9);
			DataRange indexRange = new DataRange(0, 4);

			float percent = happinessRange.PercentageInInterval( happiness );
			index = Mathf.RoundToInt( indexRange.ValueFromPercentage(percent) );

			//Debug.LogWarning("Happiness viz : index " + index + " from happiness " + happiness + " from percent " + percent + " @ " + indexRange.ValueFromPercentage(percent));
		}

		icon.GetComponent<SpriteRenderer>().sprite = icon.GetComponent<HappinessIcon>().states[ index ];

		/*
		for( int i = 0; i < showCount; ++i )
		{
			icons[i].SetActive(true);
		}

		if( showCount < 5 )
		{
			for( int i = showCount; i < 5; ++i )
			{
				icons[i].SetActive(false);
			}
		}
		*/
	}

	/*
	protected void PrepareRenderers()
	{
		icons = new List<GameObject>();

		float totalWidth = 0.0f;
		float singleWidth = happinessIconPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
		float padding = 10.0f;

		totalWidth = (singleWidth + padding) * 5.0f;

		for( int i = 0; i < 5; ++i )
		{
			GameObject icon = (GameObject) GameObject.Instantiate( happinessIconPrefab );
			icon.name = "Icon" + i;

			icon.transform.parent = this.transform;
			icon.transform.localPosition = Vector3.zero;

			float xPos = i * (singleWidth + padding);
			
			// center the 5 icons around transform.position (subtract totalWidth / 2.0f from each)
			xPos -= totalWidth / 2.0f;

			icon.transform.position += new Vector3( xPos , 0.0f, 0.0f );

			icon.SetActive( false );

			icons.Add ( icon );
		}
	}
	*/

	public void Hide()
	{
		/*
		foreach( GameObject icon in icons )
		{
			icon.SetActive(false);
		}
		*/

		foreach( Transform child in this.transform )
		{
			child.gameObject.SetActive(false);
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
