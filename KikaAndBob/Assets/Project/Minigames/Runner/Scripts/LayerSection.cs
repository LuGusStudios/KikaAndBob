using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerSection : MonoBehaviour 
{
	public LayerSpawner spawner = null;
	public ParallaxMover mover = null;

	public Transform baseLayer = null; 
	public SpriteRenderer baseRenderer = null;

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
			baseRenderer.sprite = spawner.baseLayer[ Random.Range(0, spawner.baseLayer.Length) ];
			
			width = baseRenderer.bounds.size.x;
			height = baseRenderer.bounds.size.y;
		}

		// TODO: maybe there is a better way for the details than Instantiate and Destroy all the time?
		foreach( GameObject detail in details )
		{
			GameObject.Destroy( detail );
		}

		RunnerInteractionZone[] zones = transform.GetComponentsInChildren<RunnerInteractionZone>();
		foreach( RunnerInteractionZone zone in zones )
		{
			GameObject.Destroy( zone.gameObject );
		}

		// details
		if( spawner.detailLayer.Length > 0 )
		{
			GameObject detail = new GameObject("Detail");
			details.Add( detail );

			detail.transform.parent = this.transform;
			detail.transform.position = baseLayer.position;
			
			detail.transform.position = detail.transform.position.zAdd( -5.0f );
			// /2.0f because position is already in the center
			// extra /100.0f because the pixel-to-unit ratio is 100 and width is in pixels, while position is in units
			detail.transform.position = detail.transform.position.xAdd ( (width / 2.0f) * Random.Range(-1.0f, 1.0f) ); 
			if( spawner.detailsRandomY )
			{
				detail.transform.position = detail.transform.position.yAdd( new DataRange(-2.0f, 0.5f).Random() );
			}
			
			detail.transform.localScale = baseLayer.localScale;
			
			//detail.transform.localPosition = new Vector3(0, currentSection.transform.FindChild("Base").renderer.bounds.size.y / 2.0f, 0.0f);
			//detail.transform.localPosition = detail.transform.localPosition.zAdd( -5.0f );
			
			//detail.transform.localPosition = detail.transform.localPosition.x ( width * Random.value );
			
			SpriteRenderer srend = detail.AddComponent<SpriteRenderer>();
			srend.sprite = spawner.detailLayer[ Random.Range(0, spawner.detailLayer.Length) ];
			
			// figure out the y position of the detail
			// for this: figure out the pivot point of the sprite
			// if center.y == 0.0f : pivot is center
			// if center.y == extents.y : pivot is BOTTOM (not top, as you might expect : inverted y axis logic)
			// if center.y == -extens.y : pivot is TOP

			if( srend.sprite.bounds.center.y == srend.sprite.bounds.extents.y )
			{
				// pivot is BOTTOM : hug the underside of the layer
				
				detail.transform.position = detail.transform.position.y( baseLayer.position.y - (this.height / 2.0f) );
			}
			else if( srend.sprite.bounds.center.y == (-1 * srend.sprite.bounds.extents.y) )
			{
				// pivot is TOP: hug top side of the layer
				detail.transform.position = detail.transform.position.y( baseLayer.position.y + (height / 2.0f) );
			}
			
			//Debug.Log ("EXTENTS : " + srend.sprite.name + " -> " + srend.sprite.bounds.center + " // " + srend.sprite.bounds.extents);
			
			ParallaxMover mover = this.GetComponent<ParallaxMover>();
			if( mover != null )
			{
				ParallaxMover moverDetail = detail.AddComponent<ParallaxMover>();
				if( mover.speed < 0 )
					moverDetail.speed = /*mover.speed -*/ 0.5f;
				else
					moverDetail.speed = /*mover.speed +*/ 0.5f;
				
			} 
		}
	}


	public void SetupLocal()
	{
		baseLayer = transform.FindChild("Base");

		if( baseLayer == null )
		{
			Debug.LogError (name + " : Base layer was not found!");
		}
		else
		{
			baseRenderer = baseLayer.GetComponent<SpriteRenderer>();
			if( baseRenderer == null )
			{
				Debug.LogError (name + " : Base layer does not have a SpriteRenderer!");
			}
			else
			{
				width = baseRenderer.bounds.size.x;
				height = baseRenderer.bounds.size.y;
			}
		}

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
