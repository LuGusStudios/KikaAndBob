using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemBobbing : MonoBehaviour 
{
	public float dobbingOffset = 1f;			// Maximum height the object will dob from resting state
	public float dobbingSpeed = 1f;				// Seconds to reach the max dobbing offset from resting state

	protected float timer = 0f;

	public void SetupLocal()
	{
		timer = Random.Range(0f, Mathf.PI * 2f);
	}
	
	public void SetupGlobal()
	{
		StartCoroutine(Dobbing());
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	protected IEnumerator Dobbing()
	{
		if (dobbingOffset <= 0f)
		{
			yield break;
		}

		while (true)
		{

			float waveSlice = Mathf.Sin(timer);
			timer += dobbingSpeed * Time.deltaTime;

			if (timer > (Mathf.PI * 2f))
			{
				timer = timer - (Mathf.PI * 2f);
			}

			float offset = waveSlice * dobbingOffset * Time.deltaTime;

			transform.Translate(transform.up.normalized * offset, Space.World);

			yield return new WaitForEndOfFrame();
		}
	}
}
