using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HideOnMobile : MonoBehaviour 
{
	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// hide everything in non-mobile platforms, but keep in editor for testing
		#if UNITY_IPHONE || UNITY_ANDROID
		this.gameObject.SetActive(false);
		#endif
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
