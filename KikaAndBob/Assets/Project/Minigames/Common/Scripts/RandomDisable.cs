using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomDisable : MonoBehaviour 
{
	public float disableChance = 0.5f;

	public void SetupLocal()
	{
		if( Random.value < disableChance )
		{
			this.gameObject.SetActive(false);
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
}
