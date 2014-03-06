using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour 
{
	protected iTweener shakeTween = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		//shakeTween = gameObject.MoveTo(transform.position + new Vector3(0, 10f, 0)).EaseType(iTween.EaseType.easeInOutBounce).Time(0.5f);
		LugusCoroutines.use.StartRoutine(ShakeRoutine());
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

	protected IEnumerator ShakeRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(1.0f, 4.0f));

			if (DartsLevelConfiguration.use.GameRunning)
			{
				iTween.ShakePosition(gameObject, iTween.Hash(
					"y", 0.1f,
					"time", Random.Range(.2f, 1.0f)));

				//shakeTween.Execute();
			}
		}
	}
}
