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
		FollowCamera();
	}

	protected void FollowCamera()
	{
		if (track)
			transform.position = PacmanGameManager.use.GetActivePlayer().transform.position + playerOffset;
	}
}
