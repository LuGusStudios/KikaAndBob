using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class DartsBeanShooter : MonoBehaviour 
{
	public GameObject bulletPrefab = null;
	public GameObject bulletTrailPrefab = null;
	public string shootAnimation = "BobSpitting_Spit";
	public string idleAnimation = "BobSpitting_Idle";
	public string[] shootSoundKeys = null;

	public DataRange bulletTravelTimeRange = new DataRange(0.1f, 0.2f);

	protected BoneAnimation bobAnimation = null;
	protected Transform spitStart = null;
	
	protected bool shooting = false;
	
	public void SetupLocal()
	{
		if( bulletPrefab == null )
		{
			bulletPrefab = GameObject.Find ("Bullet");
		}
		
		if( bulletPrefab == null )
		{
			Debug.LogError(name + " : No bulletPrefab found for this shooter!");
		}

		if ( bulletTrailPrefab == null )
		{
			bulletTrailPrefab = GameObject.Find ("BulletTrail");
		}

		if( bulletTrailPrefab == null )
		{
			Debug.LogError(name + " : No bulletTrailPrefab found for this shooter!");
		}
		
		bobAnimation = GetComponent<BoneAnimation>();
		if( bobAnimation == null )
		{
			Debug.LogError(name + " : No boneanimation found for this shooter!");
		}
		else
		{
			bobAnimation.Play(idleAnimation);
		}

		spitStart = transform.FindChildRecursively("SpitStart");
		if( spitStart == null )
		{
			Debug.LogError(name + " : No spit start transform found for this shooter!");
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
	
	protected void Update () 
	{
		CheckShoot();
	}
	
	protected void CheckShoot()
	{
		if( shooting )
			return;

		// enables Rambob mode
//		if(LugusInput.use.dragging)
//		{
//			LugusCoroutines.use.StartRoutine( ShootRoutine() );
//		}

		if( LugusInput.use.down )
		{
			LugusCoroutines.use.StartRoutine( ShootRoutine() );
		}
	}
	
	protected IEnumerator ShootRoutine()
	{
		// This coroutine does 2 things:
		// - control the shooter itself (bool shooting on/off) and graphical updates
		// - control the bullet : graphical movement, but also resolution of the hit

		if (shootSoundKeys != null && shootSoundKeys.Length > 0)
		{
			string key = shootSoundKeys[Random.Range(0, shootSoundKeys.Length)];
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(key));
		}
		
		//Vector3 screenPoint = LugusInput.use.lastPoint;
		Vector3 worldTarget = LugusInput.use.ScreenTo3DPoint(LugusInput.use.lastPoint, this.transform.position, LugusCamera.game);
		
		Transform hit = LugusInput.use.RayCastFromMouse(LugusCamera.game);

		shooting = true;

		bobAnimation.Play(shootAnimation, PlayMode.StopAll);
		
		GameObject bullet = (GameObject) GameObject.Instantiate( bulletPrefab );
		bullet.transform.position = spitStart.transform.position;
		bullet.transform.eulerAngles = spitStart.transform.eulerAngles;

//		GameObject trailObject = new GameObject("Trail");
//		LineRenderer trail = trailObject.AddComponent<LineRenderer>();
//		trail.SetVertexCount(2);
//		trail.SetPosition(0, spitStart.position);

		//GameObject trail = (GameObject) GameObject.Instantiate( bulletTrailPrefab );
		GameObject trail = bullet.transform.FindChild("BulletTrail").gameObject;
		trail.transform.position = spitStart.transform.position;
		trail.transform.eulerAngles = spitStart.transform.eulerAngles;

		float targetScale = Vector3.Distance(spitStart.transform.position, worldTarget) / trail.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
	
		if (targetScale > 1.0f)
			targetScale = 1.0f;

		trail.transform.localScale = new Vector3(1, 0, 1);
		
		float travelTime = bulletTravelTimeRange.Random();
		bullet.MoveTo( worldTarget ).Time( travelTime ).Execute();
		trail.ScaleTo( new Vector3(1, targetScale, 1) ).Time ( travelTime ).Execute();

		yield return new WaitForSeconds(travelTime);

		GameObject.Destroy( trail );
		GameObject.Destroy( bullet );

		IDartsHitable hitable = null;
		if( hit != null )
			hitable = hit.GetComponent<IDartsHitable>();
		
		
		if( hit != null && hitable != null && hitable.Shown )
		{
			//bullet.transform.localScale *= (1.0f / hit.transform.localScale.x);
			bullet.transform.parent = hit;
			
			//Debug.Log (Time.frameCount + " HIT " + hit.name);
			
			//yield return new WaitForSeconds(15.0f);
			

			
			// TODO: GetComponent<DartsHitable>.OnHit()
			hitable.OnHit();
		}
		
		shooting = false; // the shooter can be used again for another bullet

//		while(bobAnimation.IsPlaying(shootAnimation))
//		{
//			yield return new WaitForEndOfFrame();
//		}
		
		bobAnimation.Play(idleAnimation, PlayMode.StopAll);
	}
}
