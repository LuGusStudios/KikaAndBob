using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DinnerDashBlender : MonoBehaviour 
{
	public BoneAnimation boneAnimation = null;
	
	public void OnProcessingStart(Consumable consumable)
	{
		string name = consumable.definition.name;

		if( name.Contains("Orange") || name.Contains("orange") )
		{
			boneAnimation.Play("BlenderOnOrange", PlayMode.StopAll);
		}
		else if( name.Contains("Tomato") || name.Contains("tomato") )
		{
			boneAnimation.Play("BlenderOnTomato", PlayMode.StopAll);
		}
	}
	
	public void OnProcessingEnd(Consumable consumable)
	{
		Debug.LogWarning("NO LONGER PROCESSING STUFF");
		boneAnimation.Play("BlenderEmpty", PlayMode.StopAll);
	}
	
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( boneAnimation == null )
		{
			Transform background = transform.FindChild("Background");
			if( background != null )
			{
				boneAnimation = background.GetComponent<BoneAnimation>();
			}
		}
		
		if( boneAnimation == null )
		{
			Debug.LogError(name + " : no BoneAnimation found!");
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
		GetComponent<ConsumableProcessor>().onProcessingStart += OnProcessingStart;
		GetComponent<ConsumableProcessor>().onProcessingEnd += OnProcessingEnd;
		
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
