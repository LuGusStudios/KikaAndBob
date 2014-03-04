using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FailScreen : MonoBehaviour 
{
	public Vector3 originalPosition = Vector3.zero; 

	public void Show(float time = 3.0f) 
	{
		gameObject.StopTweens ();

		// pivot is at (0,0), so offscreen is just width or -width :)

		// moves take up 2/4th of the time, stand still in center of screen for 2/4th
		float moveTime = time * 0.33333f;
		float showTime = moveTime;// * 2;

		transform.position = originalPosition.xAdd ( LugusUtil.UIWidth ); // right offscreen

		gameObject.MoveTo( originalPosition ).Time( moveTime ).EaseType(iTween.EaseType.easeOutBack).Execute();

		gameObject.MoveTo( originalPosition.xAdd ( - LugusUtil.UIWidth ) ).Delay( moveTime + showTime ).Time( moveTime ).EaseType( iTween.EaseType.easeInBack ).Execute();
	}
	
	public void Hide()
	{
		//this.gameObject.SetActive(false); 
		
		transform.position = new Vector3(9999.0f, 9999.0f, 9999.0f);
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		originalPosition = this.transform.position;
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
