using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class to denote 
public class FroggerLaneItemDestructible : FroggerCollider 
{

	public ParticleSystem destructionParticles = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		if (destructionParticles == null)
		{
			destructionParticles = transform.FindChild("DestructionDust").GetComponent<ParticleSystem>();
			destructionParticles.Stop();
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

	public virtual void Destruct()
	{
		// Base implementation - Do not call base implementation
		// if the object still has to do some stuff before being destroyed
		StartCoroutine(DestructRoutine());
	}

	private IEnumerator DestructRoutine()
	{
		if (destructionParticles != null)
		{
			destructionParticles.Play();

			yield return new WaitForSeconds(destructionParticles.duration * 0.5f);

			SpriteRenderer[] renderers = transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer renderer in renderers)
			{
				renderer.enabled = false;
				collider2D.enabled = false;
			}

			yield return new WaitForSeconds(destructionParticles.startLifetime);
		}

		GameObject.Destroy(this.gameObject);
		yield break;
	}
}
