using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsumableListVisualizer : MonoBehaviour 
{
	public Transform background = null;

	public List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	public void Visualize(List<Consumable> consumables)
	{
		if( consumables == null || consumables.Count == 0 )
		{
			Hide ();
			return;
		}

		List<ConsumableDefinition> definitions = new List<ConsumableDefinition>();

		for( int i = 0; i < consumables.Count; ++i )
		{
			definitions.Add( consumables[i].definition );
		}

		Visualize (definitions, consumables[0].State );
	}

	public void Visualize(List<ConsumableDefinition> definitions, Lugus.ConsumableState state = Lugus.ConsumableState.Processed )
	{
		if( definitions == null || definitions.Count == 0 )
		{
			Hide ();
			return;
		}


		if( background != null )
			background.renderer.enabled = true;

		PrepareRenderers( definitions );


		// assumptions:
		// - all sprites are of equal width
		// - all sprites have their pivot in the center
		// - the parent object(s) have a scale of 1

		float padding = 10.0f;// extra padding of 5 px between items

		float spriteWidth = definitions[0].textureProcessed.bounds.size.x;
		float totalWidth = (spriteWidth * definitions.Count) + ((definitions.Count - 1) * padding); 
		float leftStart = transform.position.x - (totalWidth / 2.0f ) + (spriteWidth / 2.0f);

		for( int i = 0; i < definitions.Count; ++i )
		{
			renderers[i].transform.position = renderers[i].transform.position.x ( leftStart + (i * spriteWidth) + ( i * padding ) );
			renderers[i].sprite = definitions[i].TextureForState( state );
		}
	}

	public void Hide()
	{
		if( background != null )
			background.renderer.enabled = false;

		for( int i = 0; i < renderers.Count; ++i )
		{
			renderers[i].gameObject.SetActive(false);
		}
	}

	protected void PrepareRenderers(List<ConsumableDefinition> definitions)
	{
		// a. make sure we have enough renderers for the current order
		// b. if we have too many, hide the excess

		int newCount = definitions.Count - renderers.Count;
		for( int i = 0; i < newCount; ++i )
		{
			GameObject definitionRenderer = new GameObject("Consumable");
			SpriteRenderer rend = definitionRenderer.AddComponent<SpriteRenderer>();

			definitionRenderer.transform.parent = this.transform;
			definitionRenderer.transform.localPosition = Vector3.zero.z( -10 ); // a little in front of the background, which is at local z 0

			renderers.Add( rend );
		}

		for( int i = 0; i < definitions.Count; ++i )
		{
			renderers[i].gameObject.SetActive(true);
		}

		for( int i = definitions.Count; i < renderers.Count; ++i )
		{
			renderers[i].gameObject.SetActive(false);
		}
	}

	public void SetupLocal()
	{
		if( background == null )
			background = transform.FindChild("Background");

		if( background == null )
			Debug.LogWarning(name + " : no Background found for ListVisualizer");
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
