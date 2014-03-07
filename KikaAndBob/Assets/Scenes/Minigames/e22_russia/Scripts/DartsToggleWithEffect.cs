using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsToggleWithEffect : DartsToggle 
{
	public string hitSoundKey = "Poof01";
	protected ParticleSystem appearParticles = null;
	

	public override void Show ()
	{
		base.Show();

		if (!string.IsNullOrEmpty(hitSoundKey))
		{
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
		}

		if (appearParticles != null)
		{
			appearParticles.Play();
		}
	}

	public override void Hide ()
	{
		this.Shown = false;

		if (!string.IsNullOrEmpty(hitSoundKey))
		{
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(hitSoundKey));
		}

		// we can just disable everything, because we want the particles to stay visible

		foreach(Transform t in transform)
		{
			if (t != appearParticles.transform)
				t.gameObject.SetActive(false);
		}

		if (appearParticles != null)
		{
			appearParticles.Play();
		}
	}

	public override void Disable ()
	{
		this.Shown = false;
		// don't do effect here
		foreach(Transform t in transform)
		{
			if (t != appearParticles.transform)
				t.gameObject.SetActive(false);
		}
	}


	public void SetupLocal()
	{
		if (appearParticles == null)
		{
			appearParticles = GetComponentInChildren<ParticleSystem>();
		}
		if (appearParticles == null)
		{
			Debug.Log("DartsToggleWithEffect: Missing appear particles.");
		}
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
