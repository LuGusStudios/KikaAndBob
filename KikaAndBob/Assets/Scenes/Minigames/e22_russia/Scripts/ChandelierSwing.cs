using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChandelierSwing : MonoBehaviour 
{

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		transform.localEulerAngles = transform.localEulerAngles.z(-5.0f);
		
		//iTween.RotateTo(gameObject, transform.localEulerAngles.z(5.0f), 1.0f);
		iTween.RotateTo(gameObject, iTween.Hash(
			"z", 5,
			"time", 1.0f,
			"easetype", iTween.EaseType.easeInOutSine,
			"looptype", iTween.LoopType.pingPong));

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
