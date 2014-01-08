using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerInteractionZone : MonoBehaviour 
{
	public float sectionSpan = 0.5f; // nr of ground sections this thing spans (default 0.5 = approx. 1 screenwidth/height, 1 = approx. 2 screenwidths)
	public int difficulty = 1; // 0 = very easy, 5 = majorly difficult

	public BackgroundTheme[] themes; // themes in which this block can occur. Empty = any theme


	public void SetupLocal()
	{
		// assign variables that have to do with this class only
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
