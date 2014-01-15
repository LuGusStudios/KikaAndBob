using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsFunctionalityGroup : MonoBehaviour 
{
	public List<IDartsHitable> hitables = new List<IDartsHitable>();

	public float itemsOnScreen = 1.0f;
	public float minTimeBetweenShows = 1.0f;
	public DataRange autoHideTimes = new DataRange(2.0f, 4.0f);

	public void SetupLocal()
	{
		hitables.AddRange( transform.GetComponentsInChildren<IDartsHitable>() );

		if( hitables.Count == 0 )
		{
			Debug.LogError(name + " : group has no hitables!");
		}
		
		foreach( IDartsHitable hitable in hitables )
		{
			hitable.group = this;
		}

	}
	
	public void SetupGlobal()
	{
		foreach( IDartsHitable hitable in hitables )
		{
			hitable.Hide();
		}


		LugusCoroutines.use.StartRoutine( SpawnRoutine() );
	}

	public int shownCount = 0;

	public void HitableHit(IDartsHitable hitable)
	{
		Debug.Log ("HIT POSITION " + hitable.transform.position);
		DartsScoreManager.use.ShowScore(hitable.GetScore(), hitable.transform.position, 1.0f, null, Color.red);
	}

	public void HitableHidden(IDartsHitable hitable)
	{
		shownCount--;

		if( shownCount < 0 )
			shownCount = 0;

		Debug.Log (name + " : HitableHidden" + shownCount);
	}

	public void HitableShown(IDartsHitable hitable)
	{
		shownCount++;
		Debug.Log (name + " : HitableShown " + shownCount);
	}

	protected IDartsHitable NextHitable()
	{
		IDartsHitable output = null;

		// before, we just had a foreach on the hitables
		// with a high-frequency group however, with a low timeBetweenShows
		// we would never reach the end of the list. So those items were never shown
		// best solution would be to randomize the list before searching, but that might be performance heavy
		// another approach is to reverse the list about 50% of the time, so the latest items will get their chance to shine
		// downside: items in the middle are still much more likely to be chosen

		if( Random.value < 0.5f )
			hitables.Reverse ();

		foreach( IDartsHitable hitable in hitables )
		{
			if( hitable.Shown )
				continue;

			// make sure hitable has been hidden long enough (not hit recently) before re-showing
			if( (Time.time - hitable.lastHideTime) < minTimeBetweenShows )
				continue;

			// TODO: possibly add a distance check to other shown items here
			// this would require another foreach loop though... or a spherical raycast using the physics system... 

			output = hitable;
			break;
		}


		return output;
	}

	protected IEnumerator SpawnRoutine()
	{
		while( true )
		{
			//TODO: werk met geheel deel en na de komma deel van itemsOnScreen (ipv enkel > 1 en < 1 daarvoor) bijv. 2.5 on screen
			// bijv. (int) itemsOnScreen is geheel deel. als shownCount tussen dat en dat + 1 (kleiner dan) ligt doen we random afweging
			// dat werkt dan ook direct als < 1 (als showncount minder dan 1 moeten we beslissen)
			bool spawn = (shownCount < itemsOnScreen);

			if( itemsOnScreen < 1.0f )
			{
				spawn = spawn && (Random.value < itemsOnScreen);
			}

			if( spawn )
			{
				IDartsHitable next = NextHitable();
				if( next != null )
				{
					next.Show();
					next.AutoHide( autoHideTimes.Random() );
				}
			}

			yield return new WaitForSeconds( 0.1f );
		}
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
