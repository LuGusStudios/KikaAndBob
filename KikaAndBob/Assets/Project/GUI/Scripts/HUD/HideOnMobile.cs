using UnityEngine;
using System.Collections;

public class HideOnMobile : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{

#if (UNITY_IPHONE || UNITY_ANDROID)
		gameObject.SetActive(false);
#endif


	}
}
