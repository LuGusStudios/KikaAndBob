using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class FroggerSurface : MonoBehaviour {

	public List<AudioClip> enterSounds = new List<AudioClip>();
	protected bool onSurface = false;
	protected BoxCollider2D boxCollider2D = null;

	protected void Awake()
	{
		SetUpLocal();
	}
	
	public virtual void SetUpLocal()
	{
		boxCollider2D = GetComponent<BoxCollider2D>();
	}

	public void Enter(FroggerCharacter character)
	{
		if (onSurface)
			return;

		onSurface = true;

		if (enterSounds.Count > 0)
		{
			LugusAudio.use.SFX().Play(enterSounds[Random.Range(0, enterSounds.Count)]);
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
