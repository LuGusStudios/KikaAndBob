using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class ConsumableUserToggler : MonoBehaviour 
{
	public IConsumableUser[] targets;
	public DataRange toggleTime = new DataRange(1.0f, 3.0f);

	public string idleAnimation = "";
	public string busyAnimation = "";

	protected int busyCount = 0;

	public ParticleSystem busyParticles = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( targets.Length == 0 )
		{
			Debug.LogError(name + " : No targets found!");
		}
		else
		{
			foreach( IConsumableUser target in targets )
			{
				target.onUsed += OnTargetUsed;
			}
		}

		if( idleAnimation == "" || busyAnimation == "" )
		{
			Debug.LogError(name + " : No idleAnimation and/or no busyAnimation set!");
		}

		if( busyParticles == null )
		{
			busyParticles = transform.GetComponentInChildren<ParticleSystem>();
		}
		else
		{
			Debug.LogError(transform.Path() + " : no busy particles found!");
		}
		
		GetComponent<BoneAnimation>().Play( idleAnimation );
	}

	protected void OnTargetUsed(IConsumableUser target)
	{
		target.gameObject.SetActive(false); 
		
		LugusCoroutines.use.StartRoutine( EnableRoutine(target) );
	}

	protected IEnumerator EnableRoutine(IConsumableUser target)
	{
		busyCount++;
		if( busyCount == 1 )
		{
			//Debug.LogError("BOB DOING STUFF");
			//GetComponent<BoneAnimation>().Play( busyAnimation); //Blend( busyAnimation, 1.0f );
			GetComponent<BoneAnimation>().CrossFade( busyAnimation, 1.0f );
			//GetComponent<BoneAnimation>().Blend( idleAnimation, 0.0f );
		}

		float toggleTimeChosen = toggleTime.Random();

		if( busyParticles != null )
		{
			busyParticles.Play();
			busyParticles.enableEmission = true;
			busyParticles.startLifetime = toggleTimeChosen + 1.0f;
		}


		yield return new WaitForSeconds( toggleTimeChosen );

		target.gameObject.SetActive(true);

		busyCount--;
		if( busyCount <= 0 )
		{
			//Debug.LogError("BOB stopped DOING STUFF");
			busyCount = 0;
			//GetComponent<BoneAnimation>().Play(idleAnimation);//Blend( idleAnimation, 1.0f );
			
			//GetComponent<BoneAnimation>().Blend( busyAnimation, 0.0f );
			GetComponent<BoneAnimation>().CrossFade( idleAnimation, 1.0f );
		}
		
		if( busyParticles != null )
		{
			busyParticles.enableEmission = false;
		}
	}

	public void OnDisable()
	{
		foreach( IConsumableUser target in targets )
			target.onUsed -= OnTargetUsed;
	}
	
	public void SetupGlobal()
	{ 
		foreach( IConsumableUser target in targets )
		{
			target.gameObject.SetActive(false);
			LugusCoroutines.use.StartRoutine( EnableRoutine(target) );
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
}
