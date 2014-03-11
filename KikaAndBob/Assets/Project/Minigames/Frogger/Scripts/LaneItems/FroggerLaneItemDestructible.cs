using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class to denote 
public class FroggerLaneItemDestructible : FroggerCollider 
{

	public ParticleSystem destructionParticles = null;
	
	public override void SetupGlobal()
	{
		base.SetupGlobal();

		if (destructionParticles == null)
		{
			Transform dustObj = transform.FindChild("DestructionDust");
			if (dustObj != null)
			{
				destructionParticles = dustObj.GetComponent<ParticleSystem>();
				destructionParticles.Stop();
			}
		}
	}

	private void Start () 
	{
		SetupGlobal();
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
