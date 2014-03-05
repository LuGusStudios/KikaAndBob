using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsPuddingThrower : MonoBehaviour 
{
	public GameObject bulletPrefab = null;
	public string[] shootSoundKeys = null;
	
	public DataRange bulletTravelTimeRange = new DataRange(0.1f, 0.2f);
	
	protected GameObject bulletHitParticles = null;
	
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
		
		if (bulletHitParticles == null)
		{
			bulletHitParticles = transform.FindChild("HitParticles").gameObject;
		}
		if (bulletHitParticles == null)
		{
			Debug.LogError(name + " : No hit particles found for this shooter!");
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
		if( shooting || !DartsLevelConfiguration.use.GameRunning)
			return;

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
		
		GameObject bullet = (GameObject) GameObject.Instantiate( bulletPrefab );
		bullet.transform.position = transform.position;
		bullet.transform.eulerAngles = transform.eulerAngles;
		
		//		GameObject trailObject = new GameObject("Trail");
		//		LineRenderer trail = trailObject.AddComponent<LineRenderer>();
		//		trail.SetVertexCount(2);
		//		trail.SetPosition(0, spitStart.position);


		float travelTime = bulletTravelTimeRange.Random();

		bullet.MoveTo( worldTarget ).Time( travelTime ).Execute();
		bullet.ScaleTo( bullet.transform.localScale * 0.05f ).Time ( travelTime ).Execute();

		yield return new WaitForSeconds(travelTime);

		//GameObject.Destroy( bullet );
		
		IDartsHitable hitable = null;
		if( hit != null )
			hitable = hit.GetComponent<IDartsHitable>();
		
		
		if( hit != null && hitable != null && hitable.Shown )
		{
			//bullet.transform.localScale *= (1.0f / hit.transform.localScale.x);
			bullet.transform.parent = hit;
			
			//Debug.Log (Time.frameCount + " HIT " + hit.name);
			
			//yield return new WaitForSeconds(15.0f);
			
			
			hitable.OnHit();

			if (bulletHitParticles != null)
			{
				GameObject hitParticlesSpawn = (GameObject)Instantiate(bulletHitParticles); 
				hitParticlesSpawn.transform.position = worldTarget;
				hitParticlesSpawn.transform.localScale = Vector3.one;	// for neatness, the particle effect prefab is now parented to Bob. However this means Bob's scale also affects it, which will screw with the effect when it's instantiated.
				hitParticlesSpawn.GetComponent<ParticleSystem>().Play();
				
				Destroy(hitParticlesSpawn, 1.5f);
			}
		}
		
		shooting = false; // the shooter can be used again for another bullet
	}
}
