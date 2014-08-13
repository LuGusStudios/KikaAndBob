using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayItemHider : MonoBehaviour 
{
	public int index = 0;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		Evaluate();
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start() 
	{
		SetupGlobal();
	}
	
	protected void Update() 
	{
	
	}

	public void Evaluate()
	{
		if (LugusConfig.use.User.GetBool(Application.loadedLevelName + "_unlock_" + index, false))
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	protected void Hide()
	{
		//gameObject.SetActive(false);
//		foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
//		{
//			sprite.color = sprite.color.a(0.3f);
//		}
	}

	protected void Show()
	{
		gameObject.SetActive(true);
//		foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
//		{
//			sprite.color = sprite.color.a(1);
//		}
	}

}
