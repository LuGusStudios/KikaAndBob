using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTileItem : MonoBehaviour 
{
	public PacmanTile parentTile;
    public string uniqueId;
    public string linkedId;
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public virtual void SetupLocal()
	{
	}

	public virtual void SetupGlobal()
	{
	}

	protected void Update () 
	{
	
	}

	public virtual void Initialize()
	{

	}

	public virtual void OnTryEnter(PacmanCharacter character)
	{

	}

	public virtual void OnEnter(PacmanCharacter character)
	{
		
	}
}
