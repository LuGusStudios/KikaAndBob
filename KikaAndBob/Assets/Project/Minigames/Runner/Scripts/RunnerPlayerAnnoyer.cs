using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerPlayerAnnoyer : MonoBehaviour 
{
	public RunnerPickup pickup = null;
	public GameObject character = null;

	public void SetupLocal()
	{
		pickup = GetComponent<RunnerPickup>();
		if( pickup == null )
		{
			Debug.LogError(name + " : RunnerPickup was null!");
		}
		else
		{
			pickup.onHit += OnHit;
		}

		character = GameObject.Find ("Character");
		if( character == null )
		{
			Debug.LogError(name + " : Character was null!");
		}
	}

	protected ILugusCoroutineHandle handle = null;
	
	public void SetupGlobal()
	{
		//RunnerInteractionManager.use.Deactivate(); 

		// de-parent from a section and parent to the character
		transform.parent = LugusCamera.game.transform; //null; //GameObject.Find ("Character").transform;

		handle = LugusCoroutines.use.StartRoutine( AnnoyerRoutine() );
	}

	public void OnHit(RunnerPickup pickup) 
	{
		// player hit us: de-activate and go out of the screen
		
		if( handle != null )
		{
			handle.StopRoutine();
			handle = null;

			LugusCoroutines.use.StartRoutine( MoveOffscreenRoutine() );
		}
	}

	protected Vector3 currentOffscreen = Vector3.zero;
	protected IEnumerator MoveOffscreenRoutine()
	{
		this.collider2D.enabled = false;

		// face towards the offscreen point!
		transform.localScale = Vector3.Scale (transform.localScale, new Vector3(-1.0f, 1.0f, 1.0f) );
		
		gameObject.MoveTo( currentOffscreen ).IsLocal(true).Time (1.0f).Execute();
		yield return new WaitForSeconds(1.0f);


		//RunnerInteractionManager.use.Activate();
		
		GameObject.Destroy(this.gameObject);
	}

	protected IEnumerator AnnoyerRoutine()
	{
		// Annoyer enters the screen from the left or right side
		// then moves a couple of times up and down (vertically)
		// leaves the screen vertically (top or bottom)

		// if player hits the pickup -> immediate de-activate and leave screen 


		
		// if character most leftside: spawn on right side
		// if character most rightside: spawn left side
		Vector3 characterScreenPos = LugusCamera.game.WorldToScreenPoint( character.transform.position );

		//Debug.Log ("CHARACTER SCREEN POS " + characterScreenPos);

		/*
		bool left = true;
		if( characterScreenPos.x < (Screen.width / 2.0f) )
			left = false;
		
		// screen coords: bottom left is 0,0  top right is width,height

		// starting = somewhere offscreen. character is in top-side of screen. so y is in bottom 66% (/1.5f)
		Vector3 offscreen = new Vector3(0.0f, Random.Range(0, Screen.height / 1.5f),  LugusCamera.game.nearClipPlane);
		// somewhere between starting pos and the character (x place where the annoyer will do vertical hover)
		Vector3 target1 = new Vector3( 0.0f, offscreen.y, LugusCamera.game.nearClipPlane);
		if( left )
		{
			offscreen = offscreen.x( - this.renderer.bounds.extents.x * 100 );

			DataRange interval = new DataRange(0, characterScreenPos.x - this.renderer.bounds.extents.x * 100); // starting at 0
			target1 = target1.x ( interval.ValueFromPercentage( Random.Range(0.8f, 1.0f) ) ); // not too close to the left edge (avoid the rocks there) 
		}
		else
		{
			offscreen = offscreen.x( Screen.width + this.renderer.bounds.extents.x * 100 );
			transform.localScale.Scale( new Vector3(-1.0f, 1.0f, 1.0f) );
			
			DataRange interval = new DataRange(characterScreenPos.x + this.renderer.bounds.extents.x * 100, Screen.width );
			target1 = target1.x ( interval.ValueFromPercentage( Random.Range(0.0f, 0.2f) ) ); // not too close to the right edge (avoid the rocks there) 
		}
		*/

		//bool left = true;
		//if( Random.value < 0.5f )
		//	left = false;

		
		bool left = true;
		if( characterScreenPos.x < (Screen.width / 2.0f) )
			left = false;


		// somewhere offscreen, beneath the player
		Vector3 offscreen = new Vector3(0.0f, Random.Range((this.renderer.bounds.extents.y * 100.0f * 2.0f), characterScreenPos.y - (this.renderer.bounds.extents.y * 100.0f * 2.0f)),  LugusCamera.game.nearClipPlane);

		if( left )
		{
			offscreen = offscreen.x( - this.renderer.bounds.extents.x * 100 );
		}
		else
		{
			offscreen = offscreen.x( Screen.width + (this.renderer.bounds.extents.x * 100) );
			transform.localScale = Vector3.Scale (transform.localScale, new Vector3(-1.0f, 1.0f, 1.0f) );
		}

		// target should be somewhere in the area of the character, so player is more or less forced to move 
		float targetX = characterScreenPos.x + Random.Range( -1.0f * Screen.width / 4.0f, Screen.width / 4.0f );
		Vector3 target1 = new Vector3( targetX, offscreen.y, LugusCamera.game.nearClipPlane);

		DataRange yInterval2 = new DataRange( Screen.height / 1.125f, Screen.height - (this.renderer.bounds.extents.x * 100) ); // top 15% of the screen
		Vector3 target2 = target1.y (  yInterval2.Random()   );

		//Debug.Log ("FROM " + offscreen + " TO " + target1 + " AND " + target2 + " // extents.x " + this.renderer.bounds.extents.x );

		//Vector3 worldPos = LugusCamera.game.ScreenToWorldPoint( offscreen ).z( this.transform.position.z );
		//transform.position = worldPos;

		offscreen = LugusCamera.game.ScreenToWorldPoint( offscreen ).z( this.transform.position.z );
		offscreen = LugusCamera.game.transform.InverseTransformPoint( offscreen );
		offscreen = this.transform.localPosition.x ( offscreen.x ).y ( offscreen.y );

		transform.localPosition = offscreen;
		currentOffscreen = offscreen;

		// to world, then we need the local version as viewed by the game camera (iTweener.IsLocal) and then make sure we use the y and z of the current position and only change x
		target1 = LugusCamera.game.ScreenToWorldPoint( target1 );
		target1 = LugusCamera.game.transform.InverseTransformPoint( target1 );
		target1 = this.transform.localPosition.x ( target1.x ).y ( target1.y );

		target2 = LugusCamera.game.ScreenToWorldPoint( target2 );
		target2 = LugusCamera.game.transform.InverseTransformPoint( target2 );
		target2 = this.transform.localPosition.x ( target2.x ).y ( target2.y );
		
		//Debug.Log ("LOCALS " + target1 + " TO " + target2 );


		gameObject.MoveTo( target1 ).IsLocal(true).Time (1.0f).Execute();

		yield return new WaitForSeconds(3.0f); 

		int count = Random.Range (1,3);
		for( int i = 0; i < count; ++i )
		{
			gameObject.MoveTo( target2 ).IsLocal(true).Time (1.0f).Execute();
			yield return new WaitForSeconds(1.0f);

			gameObject.MoveTo( target1 ).IsLocal(true).Time (2.0f).Execute();
			yield return new WaitForSeconds(2.0f);
		}

		LugusCoroutines.use.StartRoutine( MoveOffscreenRoutine() );
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
