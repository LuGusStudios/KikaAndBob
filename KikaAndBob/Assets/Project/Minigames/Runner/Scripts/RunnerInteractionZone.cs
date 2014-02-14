using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerInteractionZone : MonoBehaviour 
{
	public float sectionSpan = 0.5f; // nr of ground sections this thing spans (default 0.5 = approx. 1 screenwidth/height, 1 = approx. 2 screenwidths)
	public float minimumSectionSpan = 0.5f;

	public int difficulty = 1; // 0 = very easy, 5 = majorly difficult

	public BackgroundTheme[] themes; // themes in which this block can occur. Empty = any theme

	// if the system should destroy this zone when the section it belongs to goes offscreen (for almost everything, this should be yes)
	// if this is false, the zone itself needs to make sure it is destroyed and at an appropriate time
	public bool autoDestroy = true; 

	public void OnDrawGizmos()
	{
		Transform background = transform.FindChild("Background");
		if( background != null )
		{
			float sectionWidth = background.renderer.bounds.size.x;

			Gizmos.color = Color.red;

			float zoneWidth = sectionWidth * sectionSpan;
			float minWidth = sectionWidth * minimumSectionSpan;
			Vector3 start = transform.position.xAdd( -0.5f * zoneWidth );
			Vector3 stop = start.xAdd( zoneWidth );

			Gizmos.DrawLine( start, stop );


			zoneWidth *= 0.8f;
			if( zoneWidth < minWidth )
				zoneWidth = minWidth;

			start = transform.position.xAdd( -0.5f * zoneWidth );
			stop = start.xAdd( zoneWidth );
			
			Gizmos.DrawLine( start.yAdd(0.4f), stop.yAdd(0.4f) );
			
			zoneWidth = (sectionWidth * sectionSpan) * 0.5f;
			if( zoneWidth < minWidth )
				zoneWidth = minWidth;
			
			start = transform.position.xAdd( -0.5f * zoneWidth );
			stop = start.xAdd( zoneWidth );
			
			Gizmos.DrawLine( start.yAdd(0.8f), stop.yAdd(0.8f) );


			
			Gizmos.color = Color.green;

			float sectionHeight =  background.renderer.bounds.size.y;
			float zoneHeight = sectionHeight * sectionSpan;
			float minHeight = sectionHeight * minimumSectionSpan;
			start = transform.position.yAdd( -0.5f * zoneHeight );
			stop = start.yAdd( zoneHeight );

			Gizmos.DrawLine( start, stop );


			zoneHeight *= 0.8f;
			if( zoneHeight < minHeight )
				zoneHeight = minHeight;

			start = transform.position.yAdd( -0.5f * zoneHeight );
			stop = start.yAdd( zoneHeight );

			Gizmos.DrawLine( start.xAdd(0.4f), stop.xAdd(0.4f) );

			
			zoneHeight = (sectionHeight * sectionSpan) * 0.5f;
			if( zoneHeight < minHeight )
				zoneHeight = minHeight;

			start = transform.position.yAdd( -0.5f * zoneHeight );
			stop = start.yAdd( zoneHeight );
			
			Gizmos.DrawLine( start.xAdd(0.8f), stop.xAdd(0.8f) );
		}
	}

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
