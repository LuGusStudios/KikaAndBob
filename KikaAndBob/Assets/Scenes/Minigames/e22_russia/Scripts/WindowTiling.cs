using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowTiling : MonoBehaviour 
{
	public float speed = 3.0f;
	public Vector3 angle = new Vector3(0, 0, 15.0f);

	protected SpriteRenderer image = null;
	protected float coveredDistance = 0.0f;
	protected float imageWidth = 0.0f;


	public void SetupLocal()
	{


	}
	
	public void SetupGlobal()
	{
		if (image == null)
		{
			image = GetComponentInChildren<SpriteRenderer>();
		}
		
		if (image == null)
		{
			Debug.LogError("WindowTiling: Missing sprite renderer!");
			this.enabled = false;
			return;
		}
		
		GameObject child1 = new GameObject("ImageCopy");
		SpriteRenderer imageCopy = child1.AddComponent<SpriteRenderer>();
		imageCopy.sprite = image.sprite;

		child1.transform.parent = image.transform;


		child1.transform.localPosition = Vector3.zero;
		child1.transform.localEulerAngles = Vector3.zero;
		child1.transform.localScale = Vector3.one;

		imageWidth = image.bounds.size.x;
		float offset = imageWidth;


		if (speed >= 0)
		{
			if (image.transform.localScale.x > 0)
				offset *= -1.0f;

		}
		else
		{
			if (image.transform.localScale.x < 0)
				offset *= -1.0f;
		}

		child1.transform.localPosition = new Vector3(offset, 0, 0);

		this.transform.localEulerAngles = angle;
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
		coveredDistance += speed * Time.deltaTime;

		if (Mathf.Abs(coveredDistance) >= imageWidth)
			coveredDistance = 0;

		image.transform.localPosition = image.transform.localPosition.x(coveredDistance);
	}
}
