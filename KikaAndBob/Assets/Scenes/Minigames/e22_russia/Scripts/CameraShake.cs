using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour 
{
	protected iTweener shakeTween = null;
	protected Hashtable shakeHash = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		//shakeTween = gameObject.MoveTo(transform.position + new Vector3(0, 10f, 0)).EaseType(iTween.EaseType.easeInOutBounce).Time(0.5f);
		shakeHash = iTween.Hash(
			"y", 0.05f,
			"time", Random.Range(.1f, .2f));

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
				iTween.ShakePosition(gameObject, shakeHash);

				yield return new WaitForSeconds(0.3f);

				iTween.ShakePosition(gameObject, shakeHash);

				//shakeTween.Execute();
			}
		}
	}
}
