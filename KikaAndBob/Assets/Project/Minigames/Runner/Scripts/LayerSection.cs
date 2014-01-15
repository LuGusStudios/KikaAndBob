using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerSection : MonoBehaviour 
{
	public LayerSpawner spawner = null;
	public ParallaxMover mover = null;

	public List<Transform> baseLayers = new List<Transform>(); 
	//public SpriteRenderer[] baseRenderer = null;

	// NOTE: width and height are in Sprite units
	// so if the sprite has 100 pixels to units, the width/height will probably be 100 times less than you expect
	public float width = 0.0f;
	public float height = 0.0f;

	public List<GameObject> details = new List<GameObject>();

	// makes this into a new (semi-)random section
	// requests the needed info from the LayerSpawner for this
	public void Reset()
	{
		// base layer
		// this one is section-filling, so we just need to swap the textures
		if( spawner.baseLayer.Length > 0 )
		{
			foreach( Transform layer in baseLayers )
			{
				// Here, we need a way to couple various baseLayers to appropriate textures
				// for example, in some levels we have rock sides left and right
				// we do this by naming conventions: the gameObject names need to contain (a part of) the names of the textures
				// and vice versa

				// 2 possibilities: 
				// a. layer is just named "Base" : can use any sprite (wildcard)
				// b. layer is named "BaseXYZ" where XYZ is in the name of the sprite

				string nameID = layer.name.Replace("Base", ""); // figure out the "XYZ"

				if( nameID == "" ) // no "XYZ" : use any texture
				{
					layer.GetComponent<SpriteRenderer>().sprite = spawner.baseLayer[ Random.Range(0, spawner.baseLayer.Length) ];
				}
				else
				{
					//Debug.Log ("Checking for nameID " + nameID);
					List<Sprite> candidates = new List<Sprite>();
					foreach( Sprite sprite in spawner.baseLayer )
					{
						if( sprite.name.Contains( nameID ) )
						{
							candidates.Add ( sprite );
						}
					}

					if( candidates.Count > 0 )
					{
						layer.GetComponent<SpriteRenderer>().sprite = candidates[ Random.Range(0, candidates.Count) ];
					}
					else
					{
						Debug.LogError(name + " : No sprite found for baseLayer with name " + layer.name);
					}
				}
			}


			RecalculateDimensions();
		}



		// interation zones
		RunnerInteractionZone[] zones = transform.GetComponentsInChildren<RunnerInteractionZone>();
		foreach( RunnerInteractionZone zone in zones )
		{
			if( zone.autoDestroy )
			{
				GameObject.Destroy( zone.gameObject );
			}
			else 
			{
				// no autodestroy, but can't keep it in this section either...
				// de-couple and make sure it keeps moving if necessary
				zone.transform.parent = this.transform.parent; // keep it in the system, because when we reset the whole level to prevent x-overflow, this things needs to go along :)
				
				RunnerMover mover = GetComponent<RunnerMover>();
				if( mover != null )
				{
					RunnerMover mover2 = zone.GetComponent<RunnerMover>();
					mover2.speed = mover.speed;
					mover2.direction = mover.direction;
				}
			}
		}


		
		// details
		// TODO: maybe there is a better way for the details than Instantiate and Destroy all the time?
		foreach( GameObject detail in details )
		{
			GameObject.Destroy( detail );
		}

		if( spawner.detailLayer.Length > 0 )
		{
			// ex. intensity is 2.5
			// that means we will sometimes have 2, sometimes 3 details
			// split between whole (2) and fraction (0.5) and use different methods of deciding how to spawn
			int wholeCount = Mathf.FloorToInt(spawner.detailSpawnIntensity);
			float fractionCount = spawner.detailSpawnIntensity - wholeCount;

			for( int i = 0; i < wholeCount; ++i )
				SpawnDetail();

			if( Random.value < fractionCount )
				SpawnDetail();
		}
	}

	protected void SpawnDetail()
	{
		GameObject detail = new GameObject("Detail");
		details.Add( detail );
		
		detail.transform.parent = this.transform;
		// TODO: possibly make this better by just adding a "Detail" parent object in each layer?
		if( baseLayers.Count == 1 )
		{
			detail.transform.position = baseLayers[0].position; 
		}
		else
		{
			detail.transform.position = this.transform.position; 
		}
		
		// place the detail slightly to the front so it's on top of the base layers
		detail.transform.position = detail.transform.position.zAdd( -5.0f );
		
		// /2.0f because position is already in the center 
		// 0.3 to make sure we don't spawn past the right edge (which would pop the elements when the current section would be recycled)
		// TODO: improve this: check the bounds of the sprite renderer to make sure it doesn't exit the section on the right, and not too much on the left
		if( spawner.detailsRandomX )
		{
			detail.transform.position = detail.transform.position.xAdd ( (width / 2.0f) * Random.Range(-1.0f, 0.3f) ); 
		}
		if( spawner.detailsRandomY )
		{
			detail.transform.position = detail.transform.position.yAdd( new DataRange(-2.0f, 0.5f).Random() );
		}
		
		if( baseLayers.Count == 1 )
		{
			detail.transform.localScale = baseLayers[0].localScale;
		}
		
		
		SpriteRenderer srend = detail.AddComponent<SpriteRenderer>();
		srend.sprite = spawner.detailLayer[ Random.Range(0, spawner.detailLayer.Length) ];
		
		// figure out the y position of the detail
		// for this: figure out the pivot point of the sprite
		// if center.y == 0.0f : pivot is center
		// if center.y == extents.y : pivot is BOTTOM (not top, as you might expect : inverted y axis logic)
		// if center.y == -extens.y : pivot is TOP
		
		//if( !spawner.detailsRandomY )
		//{
		if( srend.sprite.bounds.center.y == srend.sprite.bounds.extents.y )
		{
			// pivot is BOTTOM : hug the underside of the layer
			
			detail.transform.position = detail.transform.position.yAdd( -1.0f * (this.height / 2.0f) );
		}
		else if( srend.sprite.bounds.center.y == (-1 * srend.sprite.bounds.extents.y) )
		{
			// pivot is TOP: hug top side of the layer
			detail.transform.position = detail.transform.position.yAdd((height / 2.0f) );
		}
		//}
		
		//Debug.Log ("EXTENTS : " + srend.sprite.name + " -> " + srend.sprite.bounds.center + " // " + srend.sprite.bounds.extents);
		
		ParallaxMover mover = this.GetComponent<ParallaxMover>();
		if( mover != null )
		{
			ParallaxMover moverDetail = detail.AddComponent<ParallaxMover>();
			if( mover.speed.x != 0.0f )
				moverDetail.speed = new Vector3(-1.0f, 0.0f, 0.0f);
			if( mover.speed.y != 0.0f )
				moverDetail.speed = new Vector3(0.0f, -1.0f, 0.0f);
			
		} 
	}

	protected void RecalculateDimensions()
	{
		// we can have multiple base layers here
		// loop over all and take the largest dimensions of the renderers
		// NOTE: this does not mean we take a bounding box or something like that!!!
		// we just get the maximum width and height of INDIVIDUAL elements, not the complete section
		// however, usually, the max width and height of the indiviuals *should* match that of the section in at least x or y
		foreach( Transform layer in baseLayers )
		{
			SpriteRenderer sr = layer.GetComponent<SpriteRenderer>();
			if( sr != null )
			{
				width  = Mathf.Max( width,  sr.bounds.size.x );
				height = Mathf.Max( height, sr.bounds.size.y );
			}
			else
			{
				Debug.LogError(name + " : baseLayer doens't have a SpriteRenderer!");
			}
		}
	}

	public void SetupLocal()
	{
		if( baseLayers.Count == 0 )
		{
			foreach( Transform child in this.transform )
			{
				if( child.name.Contains("Base") )
				{
					baseLayers.Add ( child );
				}
			}
		}

		if( baseLayers.Count == 0 )
		{
			Debug.LogError (name + " : Base layers were not found!");
		}

		RecalculateDimensions();

		if( mover == null )
		{
			mover = GetComponent<ParallaxMover>();
		}
	}
	
	public void SetupGlobal()
	{
		if( spawner == null )
		{
			Debug.LogError(name + " : No spawner known for this LayerSection! ");
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
	
	}
}
