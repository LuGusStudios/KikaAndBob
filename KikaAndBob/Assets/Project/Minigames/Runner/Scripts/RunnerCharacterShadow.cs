using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerCharacterShadow : MonoBehaviour 
{
	public GameObject character = null;
	public float speed = 100.0f;

	public DataRange yRange = new DataRange(-3.982845f, 2.9f);
	public DataRange scaleFactors = new DataRange(1.0f, 0.5f);

	public float xOffset = 0.0f;

	public Vector3 originalScale = Vector3.one;
	
	public void SetupLocal()
	{
		if( character == null )
		{
			character = GameObject.Find("Character");
		}
		
		if( character == null )
		{
			Debug.LogError(name + " : no Character found!");
		}

		originalScale = this.transform.localScale;
	}
	
	public void SetupGlobal()
	{
		transform.position = transform.position.x (character.transform.position.x + xOffset );
		
	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update()
	{
		transform.position = transform.position.x ( Mathf.Lerp(transform.position.x, character.transform.position.x + xOffset, Time.deltaTime * speed) );
	
		float scalePercentage = yRange.PercentageInInterval( character.transform.position.y );

		transform.localScale = Vector3.Lerp( transform.localScale, originalScale * scaleFactors.ValueFromPercentage(scalePercentage), Time.deltaTime * speed );

	}
}
