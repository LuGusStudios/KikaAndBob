using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour {
	
	float mouseSensitivity = 0.01f;
	Vector3 lastPosition;

	void Update () {
		
		if (Input.GetMouseButtonDown(0))
   		{
        	lastPosition = Input.mousePosition;
   		}
		
		if (Input.GetMouseButton(0))
    	{
	        Vector3 delta = Input.mousePosition - lastPosition;
	        transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, 0);
	        lastPosition = Input.mousePosition;
    	}
	
	}
}