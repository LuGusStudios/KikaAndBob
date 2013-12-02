using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerSpawner : MonoBehaviour 
{
	public GameObject currentSection = null;
	public GameObject nextSection = null;

	public Sprite[] baseLayer = null;
	public Sprite[] detailLayer = null;

	public delegate void OnSectionSwitch(GameObject currentSection, GameObject nextSection);
	public OnSectionSwitch onSectionSwitch;

	public void SetupLocal()
	{

		if( currentSection == null )
		{
			currentSection = transform.FindChildRecursively ("Section1").gameObject;
		}
		if( nextSection == null )
		{
			nextSection = transform.FindChildRecursively ("Section2").gameObject;
		}

		if( currentSection == null )
		{
			Debug.LogError(name + " : No Section1 known for the LayerSpawner");
		}
		if( nextSection == null )
		{
			Debug.LogError(name + " : No Section2 known for the LayerSpawner");
		}
	}

	public GameObject CurrentSection = null;
	
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


	/*
	protected void GenerateNextSection()
	{
		GameObject nextSection = (GameObject) GameObject.Instantiate( baseLayer );
		nextSection.transform.parent = baseLayer.transform.parent;
		nextSection.transform.position = baseLayer.transform.position; // correct z and y

		float xPos = CurrentSection.transform.position.x + CurrentSection.renderer.bounds.size.x;
		nextSection.transform.position = nextSection.transform.position.xAdd( xPos );
	}
	*/

	protected IEnumerator SpawnRoutine()
	{
		while( true )
		{
			// if currentSection is offscreen on the left side:
			// - respawn it on the right side using a new Sprite (if necessary)
			// - switch current to next section
			float width = currentSection.transform.FindChild("Base").renderer.bounds.size.x;

			float rightBound = currentSection.transform.position.x + (width / 2.0f);
			float leftBound = LugusCamera.game.transform.position.x - (LugusUtil.UIWidth / 200.0f); // by 2 to get half, by 100 for pixels to units ratio

			if( rightBound < leftBound ) 
			{
				// base layer
				currentSection.transform.FindChild("Base").GetComponent<SpriteRenderer>().sprite = baseLayer[ Random.Range(0, baseLayer.Length) ];

				width = currentSection.transform.FindChild("Base").renderer.bounds.size.x;
				float otherWidth = nextSection.transform.FindChild("Base").renderer.bounds.size.x; 

				// put it nicely to the right of the current section
				currentSection.transform.position = nextSection.transform.position.xAdd( (otherWidth / 2.0f) + (width / 2.0f) );


				// details
				if( detailLayer.Length > 0 )
				{
					GameObject detail = new GameObject("Detail");
					detail.transform.parent = currentSection.transform;
					detail.transform.position = currentSection.transform.FindChild("Base").position;

					detail.transform.position = detail.transform.position.zAdd( -5.0f );
					// /2.0f because position is already in the center
					// extra /100.0f because the pixel-to-unit ratio is 100 and width is in pixels, while position is in units
					detail.transform.position = detail.transform.position.xAdd ( (width / 2.0f) * Random.Range(-1.0f, 1.0f) ); 
					detail.transform.position = detail.transform.position.yAdd( new DataRange(-2.0f, 0.5f).Random() );

					//detail.transform.localPosition = new Vector3(0, currentSection.transform.FindChild("Base").renderer.bounds.size.y / 2.0f, 0.0f);
					//detail.transform.localPosition = detail.transform.localPosition.zAdd( -5.0f );

					//detail.transform.localPosition = detail.transform.localPosition.x ( width * Random.value );

					SpriteRenderer srend = detail.AddComponent<SpriteRenderer>();
					srend.sprite = detailLayer[ Random.Range(0, detailLayer.Length) ];

					ParallaxMover mover = currentSection.GetComponent<ParallaxMover>();
					if( mover != null )
					{
						ParallaxMover moverDetail = detail.AddComponent<ParallaxMover>();
						if( mover.speed < 0 )
							moverDetail.speed = mover.speed - 4.0f;
						else
							moverDetail.speed = mover.speed + 4.0f;

					}
				}


				GameObject temp = currentSection;
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
