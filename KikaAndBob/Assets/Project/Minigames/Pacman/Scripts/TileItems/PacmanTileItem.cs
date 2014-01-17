using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItem : MonoBehaviour 
{
	public PacmanTile parentTile;

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

	public virtual void Initialize()
	{

	}

	public virtual void OnTryEnter()
	{

	}

	public virtual void OnEnter()
	{
		
	}
}
