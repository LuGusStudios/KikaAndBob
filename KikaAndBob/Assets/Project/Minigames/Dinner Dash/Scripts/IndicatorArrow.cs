using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IndicatorArrow : MonoBehaviour 
{
	public Transform arrowRenderer = null;
	
	public void Show( GameObject target )
	{
		Vector3 position = target.transform.position;

		BoxCollider2D box = target.GetComponent<BoxCollider2D>();
		if( box != null )
		{
			Debug.LogWarning("BOX " + box.size + " // " + box.center);

			float yOffset = ((-1.0f * box.size.y) / 2.0f) - box.center.y;
			yOffset *= -1.0f;
			position = new Vector3( box.center.x, yOffset, 0.0f );
			
			position = target.transform.TransformPoint( position );
			
			Debug.LogWarning("BOXPOS " + position);
		}

		Show ( position );
	}

	public void Show( Vector3 position )
	{
		this.transform.position = position;
	}

	public void Hide()
	{
		this.transform.position = new Vector3(9999,9999,9999);
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( arrowRenderer == null )
		{
			arrowRenderer = this.transform.FindChild("Arrow");
		}
	}
	
	public void SetupGlobal()
	{
		Vector3 target = arrowRenderer.transform.localPosition.yAdd( 100.0f );

		// lookup references to objects / scripts outside of this script
		arrowRenderer.gameObject.MoveTo( target ).IsLocal( true ).Looptype(iTween.LoopType.pingPong).EaseType(iTween.EaseType.easeInOutSine).Execute();
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
