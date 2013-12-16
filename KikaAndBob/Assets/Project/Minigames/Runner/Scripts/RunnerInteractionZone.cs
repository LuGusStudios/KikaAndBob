using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerInteractionZone : MonoBehaviour 
{
	public int sectionWidth = 1; // nr of ground sections this thing spans (default 1 = 2 screenwidths)
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
