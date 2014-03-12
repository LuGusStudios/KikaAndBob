using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class FroggerSurface : MonoBehaviour {

	public bool OnSurface
	{
		get
		{
			return onSurface;
		}
	}

	public List<string> enterSoundKeys = new List<string>();
	protected bool onSurface = false;
	protected BoxCollider2D surfaceCollider = null;

	private void Awake()
	{
		SetUpLocal();
	}
	
	public virtual void SetUpLocal()
	{
		surfaceCollider = GetComponent<BoxCollider2D>();

		if (surfaceCollider == null)
		{
			Debug.LogError("FroggerSurface: " + name + " is missing surface collider.", gameObject);
		}
	}

	public void Enter(FroggerCharacter character)
	{
		if (onSurface)
			return;

		onSurface = true;

		if (enterSoundKeys.Count > 0)
		{
			int randomIndex = Random.Range(0, enterSoundKeys.Count);

			if (!string.IsNullOrEmpty(enterSoundKeys[randomIndex]))
			{
				LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(enterSoundKeys[randomIndex]));
			}
		}

		EnterSurfaceEffect(character);
	}

	public void Leave(FroggerCharacter character)
	{
		if (!onSurface)
			return;
		
		onSurface = false;

		LeaveSurfaceEffect(character);
	}

	// Basically gets the size of the collider on this sprite, which is usually the 'active' part of that sprite (i.e. interactive or not completely transparent)
	// NOTE: We have multiply the collider's size with scale to allow for sprites with various scales
	public Vector2 GetSurfaceSize()
	{
		Vector3 colliderSize = GetComponent<BoxCollider2D>().size;
		Vector3 localScale = transform.localScale;

		return new Vector2(colliderSize.x * Mathf.Abs(localScale.x), colliderSize.y * Mathf.Abs(localScale.y));
	}
	
	protected virtual void EnterSurfaceEffect(FroggerCharacter character)
	{

	}

	protected virtual void LeaveSurfaceEffect(FroggerCharacter character)
	{
	
	}
}
