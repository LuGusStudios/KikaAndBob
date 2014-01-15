using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerInteractionZone : MonoBehaviour 
{
	public float sectionSpan = 0.5f; // nr of ground sections this thing spans (default 0.5 = approx. 1 screenwidth/height, 1 = approx. 2 screenwidths)
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

			Vector3 start = transform.position.xAdd( -0.5f * sectionWidth );
			Vector3 stop = start.xAdd( sectionWidth * sectionSpan );

			Gizmos.DrawLine( start, stop );
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
