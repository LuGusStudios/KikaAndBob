using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerSpawner : MonoBehaviour 
{
	public LayerSection currentSection = null;
	public LayerSection nextSection = null;

	public Sprite[] baseLayer = null;
	public Sprite[] detailLayer = null;

	public delegate void OnSectionSwitch(LayerSection currentSection, LayerSection nextSection);
	public OnSectionSwitch onSectionSwitch;

	public bool detailsRandomY = true; // should the details in the detail layer have a semi-random y position or stick to the default pivots?

	public void SetupLocal()
	{

		if( currentSection == null )
		{
			GameObject section1 = transform.FindChildRecursively ("Section1").gameObject;

			if( section1 == null )
			{
				Debug.LogError(name + " : No Section1 known for the LayerSpawner");
			}
			else
			{
				currentSection = section1.GetComponent<LayerSection>();
				if( currentSection == null )
					currentSection = section1.AddComponent<LayerSection>();
				
				currentSection.spawner = this;
			}
		}
		if( nextSection == null )
		{
			GameObject section2 = transform.FindChildRecursively ("Section2").gameObject;

			if( section2 == null )
			{
				Debug.LogError(name + " : No Section2 known for the LayerSpawner");
			}
			else
			{
				nextSection = section2.GetComponent<LayerSection>();
				if( nextSection == null )
					nextSection = section2.AddComponent<LayerSection>();
				
				nextSection.spawner = this;
			}
		}

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

	public void StartSpawning()
	{
		LugusCoroutines.use.StartRoutine( SpawnRoutine() );
	}

	protected IEnumerator SpawnRoutine()
	{
		while( true )
		{
			// if currentSection is offscreen on the left side:
			// - respawn it on the right side using a new Sprite (if necessary)
			// - switch current to next section
		
			float rightBound = currentSection.transform.position.x + (currentSection.width / 2.0f);
			// TODO: add some pixels to UIWidth so we're 100% sure offscreen? 
			float leftBound = LugusCamera.game.transform.position.x - (LugusUtil.UIWidth / 200.0f); // by 2 to get half, by 100 for pixels to units ratio

			if( rightBound < leftBound ) 
			{
				// load new sprites and content to fill the section (which is now fully offscreen)
				currentSection.Reset();
				
				// put it nicely to the right of the current section
				currentSection.transform.position = nextSection.transform.position.xAdd( (nextSection.width / 2.0f) + (currentSection.width / 2.0f) );


				LayerSection temp = currentSection;
				currentSection = nextSection;
				nextSection = temp; 

				if( onSectionSwitch != null )
					onSectionSwitch( currentSection, nextSection );
			}


			yield return null;
		}
	}
	
	protected void Update () 
	{
		if( baseLayer == null )
		{
			Debug.LogError(name + " : No baseLayer entities known for this LayerSpawner!");
		}
		
		if( detailLayer == null )
		{
			Debug.LogError(name + " : No detailLayer entities known for this LayerSpawner!");
		}
	}
}
