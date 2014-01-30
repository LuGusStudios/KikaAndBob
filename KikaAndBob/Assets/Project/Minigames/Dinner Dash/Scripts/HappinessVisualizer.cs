using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HappinessVisualizer : MonoBehaviour 
{
	public GameObject happinessIconPrefab = null;

	protected List<GameObject> icons = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( happinessIconPrefab == null )
		{
			Debug.LogError(transform.Path () + " : No happinessIconPrefab found!" );
		}

		PrepareRenderers();
	}

	public void Visualize(float happiness)
	{
		// happiness goes from 0 to 10
		// divide in buckets of 2 wide for now
		int showCount = Mathf.FloorToInt( happiness / 2.0f );
		
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
	}

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

	public void Hide()
	{
		foreach( GameObject icon in icons )
		{
			icon.SetActive(false);
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
