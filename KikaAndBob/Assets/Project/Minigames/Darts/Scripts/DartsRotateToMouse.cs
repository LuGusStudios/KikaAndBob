using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsRotateToMouse : MonoBehaviour 
{
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
		Vector3 mouse = LugusInput.use.ScreenTo3DPoint(this.transform);


		float xDiff = mouse.x - this.transform.position.x;
		float yDiff = this.transform.position.y - mouse.y;

		Debug.DrawLine( mouse, this.transform.position );

		float angle = Mathf.Atan2( yDiff, xDiff )  * 180.0f / Mathf.PI;
		angle *= -1;
		angle -= 90.0f;

		transform.eulerAngles = transform.eulerAngles.z( angle );

		//Debug.Log ("ANGLE : " + angle + "");
	}
}
