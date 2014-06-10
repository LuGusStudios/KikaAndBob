using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionPadButton : MonoBehaviour 
{
	public Joystick.JoystickDirection direction = Joystick.JoystickDirection.None;

	public float transparency = 0.7f;

	protected SpriteRenderer buttonRenderer = null;

	public virtual void SetupLocal()
	{
		if (buttonRenderer == null)
		{
			buttonRenderer = this.GetComponent<SpriteRenderer>();
		}

		if (buttonRenderer == null)
		{
			Debug.LogError("DirectionPadButton: Missing sprite renderer!");
		}
	}
	
	public virtual void SetupGlobal()
	{
		SetPressed(false);
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	


	public virtual void SetPressed(bool pressed)
	{
		if (buttonRenderer == null)
		{
			Debug.LogWarning("DirectionPadButton: Missing a sprite renderer. Settting transparency has no effect.");
			return;
		}

		if (pressed)
			buttonRenderer.color = buttonRenderer.color.a(1.0f);
		else
			buttonRenderer.color = buttonRenderer.color.a(transparency);
	}
}
