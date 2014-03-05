using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanCameraFollower : LugusSingletonExisting<PacmanCameraFollower> 
{
	public Vector3 playerOffset = new Vector3(0, 0, -500);
	public Vector3 defaultPosition = new Vector3(768, 384, -500);
	public bool track = false;
	protected float halfScreenWidth = 0;
	protected float halfScreenHeight= 0;

	public void SetupLocal()
	{
		halfScreenWidth = Vector3.Distance(
			LugusCamera.game.ViewportToWorldPoint(new Vector3(0.0f, 0.5f, 0)),
			LugusCamera.game.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)));

		halfScreenHeight = Vector3.Distance(
			LugusCamera.game.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)),
			LugusCamera.game.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)));
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

	public void ResetCamera()
	{
		transform.position = defaultPosition;
	}

	protected void Update () 
	{
		//FollowCamera();	// we're not having this called automatically anymore - an unpredictable order of Update calls between this and the characters can cause jitter	
	}						// instead, it's called from the character scripts

	public void FollowCamera()
	{
		if (!track)
			return;

		if (PacmanGameManager.use.gameRunning && track) 
		{
			transform.position = Vector3.Lerp ( this.transform.position, PacmanGameManager.use.GetActivePlayer().transform.position + playerOffset, 100.0f *Time.deltaTime);
		}
	}
}
