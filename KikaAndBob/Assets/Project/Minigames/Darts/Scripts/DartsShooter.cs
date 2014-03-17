using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsShooter : MonoBehaviour 
{
	public GameObject bulletPrefab = null;
	protected DataRange bulletTravelTimeRange = new DataRange(0.2f, 0.4f);

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

		//Vector3 screenPoint = LugusInput.use.lastPoint;
		Vector3 worldTarget = LugusInput.use.ScreenTo3DPoint(LugusInput.use.lastPoint, this.transform.position, LugusCamera.game);

		Transform hit = LugusInput.use.RayCastFromMouse(LugusCamera.game);


		shooting = true;

		GameObject bullet = (GameObject) GameObject.Instantiate( bulletPrefab );
		bullet.transform.position = this.transform.position;
		bullet.transform.eulerAngles = this.transform.eulerAngles;

		float travelTime = bulletTravelTimeRange.Random();
		bullet.MoveTo( worldTarget ).Time( travelTime ).Execute();
		bullet.ScaleTo( bullet.transform.localScale * 0.05f ).Time ( travelTime ).Execute();


		// move the shooter itself backwards to give impression it's the shooter being thrown and quickly replaced afterwards
		Vector3 originalPosition = this.transform.localPosition;
		this.gameObject.MoveTo( originalPosition + (transform.up * -2.0f) ).IsLocal(true).Time (0.1f).Execute();

		yield return new WaitForSeconds(travelTime / 2.0f);
		
		this.gameObject.MoveTo( originalPosition ).IsLocal(true).Time (0.1f).Execute();

		yield return new WaitForSeconds(travelTime / 2.0f);

		shooting = false; // the shooter can be used again for another bullet



		// at this point, we have hit the target : resolution
		//a. if hit was null, we hit nothing of note : keep the bullet around for like 3 seconds, then make it disappear
		//b. hit was not null: remove bullet directly and resolve hit on the object

		IDartsHitable hitable = null;
		if( hit != null )
		{
			hitable = hit.GetComponent<IDartsHitable>();

			if (hitable == null && hit.GetComponent<DartsLookHigher>() != null)
			{
				Transform newParent = hit.transform.parent;

				while (newParent != null && newParent.GetComponent<IDartsHitable>() == null)
				{
					newParent = newParent.parent;
				}

				if (newParent != null)
				{
					hitable = newParent.GetComponent<IDartsHitable>();
				}
			}
		}


		if( hit == null || hitable == null || !hitable.Shown )
		{
			
			//Debug.Log (Time.frameCount + " MISSED ");

			yield return new WaitForSeconds(3.0f);

			GameObject.Destroy( bullet );
		}
		else
		{
			//bullet.transform.localScale *= (1.0f / hit.transform.localScale.x);
			bullet.transform.parent = hit;
			
			//Debug.Log (Time.frameCount + " HIT " + hit.name);

			//yield return new WaitForSeconds(15.0f);

			GameObject.Destroy( bullet );

			// TODO: GetComponent<DartsHitable>.OnHit()
			hitable.OnHit();

		}

	}
}
