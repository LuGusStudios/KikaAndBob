using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Button : MonoBehaviour 
{
	public float scaleDownFactor = 0.9f;
	public string pressSoundKey;
	public float clickAnimationTime = 0.3f;
	
	protected Vector3 originalScale = Vector3.zero;
	
	protected bool _pressed = false;
	public bool pressed
	{
		get
		{
			bool temp = _pressed;
			_pressed = false;
			return temp; 
		}
		
		set{ _pressed = value; }
	}	
	
	protected void Start() 
	{
		 GetOriginalScale();
	}
	
	public void OnEnable()
	{
		if( originalScale != Vector3.zero )
			transform.localScale = originalScale;
	}
	
	protected void Update() 
	{
		if( LugusInput.use.RayCastFromMouseUp() == this.transform )
		{
			if(!string.IsNullOrEmpty(pressSoundKey))
			{
				LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(pressSoundKey));
			}
			
			StartCoroutine( PressRoutine() );	
		}
	}
	
	// postpone the pressed-event by 1 frame
	// this makes sure the button is not triggered on the same frame as the actual MouseUp event
	protected IEnumerator PressRoutine()
	{
		transform.localScale = originalScale * scaleDownFactor;
		gameObject.ScaleTo(originalScale).IgnoreTimeScale(true).Time(clickAnimationTime).EaseType(iTween.EaseType.easeOutBack).Execute();

		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + clickAnimationTime) 
		{
			yield return null;
		}

		//yield return new WaitForSeconds(clickAnimationTime); // does not play nicely with TimeScale pause :)

		pressed = true;
	}
	
	public void Appear()
	{
		transform.localScale = originalScale * scaleDownFactor;
		
		foreach(Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.enabled = true;
		}
		
		foreach(Collider c in GetComponentsInChildren<Collider>())
		{
			c.enabled = true;
		}
		
		gameObject.ScaleTo(originalScale).IgnoreTimeScale(true).Time(clickAnimationTime).EaseType(iTween.EaseType.easeOutBack).Execute();
	}
	
	public void HideImmediately()
	{	
		foreach(Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.enabled = false;
		}
		
		foreach(Collider c in GetComponentsInChildren<Collider>())
		{
			c.enabled = false;
		}
	}
	
	protected void GetOriginalScale()
	{
		if( originalScale == Vector3.zero && transform.localScale != Vector3.zero)
			originalScale = transform.localScale;
	}
}
