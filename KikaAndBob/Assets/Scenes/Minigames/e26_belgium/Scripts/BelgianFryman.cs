using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BelgianFryman : MonoBehaviour 
{
	public GameObject[] fryers;

	public int activeCounter = 0;

	public ILugusCoroutineHandle fryingHandle = null;

	public ParticleSystem fryParticles = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( fryers == null || fryers.Length == 0 )
		{
			Debug.LogError(this.transform.Path() + " : No fryers assigned!");
		}
		else
		{
			foreach( GameObject fryer in fryers )
			{
				if( fryer.GetComponent<ConsumableProcessor>() != null )
				{
					fryer.GetComponent<ConsumableProcessor>().onProcessingStart += OnProcessorStart;
					fryer.GetComponent<ConsumableProcessor>().onProcessingEnd += OnProcessorEnd;
				}
				else if( fryer.GetComponent<ConsumableConvertor>() != null )
				{
					fryer.GetComponent<ConsumableConvertor>().onProcessingStart += OnProcessorStart;
					fryer.GetComponent<ConsumableConvertor>().onProcessingEnd += OnProcessorEnd;
				}
			}
		}

		if( fryingHandle == null )
		{
			fryingHandle = LugusCoroutines.use.GetHandle();
			fryingHandle.Claim();
		}

		if( fryParticles == null )
		{
			fryParticles = GetComponentInChildren<ParticleSystem>();
		}
		
		if( fryParticles == null )
		{
			Debug.LogError(this.transform.Path() + " : No fry particle system found!");
		}
		else
		{
			fryParticles.Stop();
		}
	}
	
	
	protected IEnumerator FryingRoutine()
	{
		GetComponent<SmoothMoves.BoneAnimation>().CrossFade("FryMan_ShakeUp");

		if( fryParticles != null )
			fryParticles.Play();

		while( activeCounter > 0 )
		{
			yield return null;
		}

		GetComponent<SmoothMoves.BoneAnimation>().CrossFade("FryMan_Idle");
		
		if( fryParticles != null )
			fryParticles.Stop();
	}
	
	public void OnProcessorStart(Consumable consumable) 
	{
		activeCounter++;

		if( !fryingHandle.Running )
		{
			fryingHandle.StartRoutine( FryingRoutine() );
		}
		
		if( activeCounter > fryers.Length ) // shouldn't happen!
		{
			Debug.LogError(this.transform.Path() + " : received OnProcessorStart while we thought all fryers were already progressing!");
		}
	}

	public void OnProcessorEnd(Consumable consumable)
	{
		activeCounter--;

		if( activeCounter < 0 ) // shouldn't happen!
		{
			Debug.LogError(this.transform.Path() + " : received OnProcessorEnd while we didn't think anything was still processing!");
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
