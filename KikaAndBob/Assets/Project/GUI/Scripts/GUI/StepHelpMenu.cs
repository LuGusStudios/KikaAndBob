using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepHelpMenu : IMenuStep 
{
	protected Button leaveButton = null;
	protected Button buttonRight = null;
	protected Button buttonLeft = null;
	protected SpriteRenderer image = null;
	protected int page = 1;				// this will be the page counter. We start from 1 here, because this makes more sense in the text file.
	protected TextMeshWrapper description = null;
	protected bool switchingPages = false;
	protected Vector3 originalPosition = Vector3.zero;

	public void SetupLocal()
	{
		if (leaveButton == null)
		{
			leaveButton = transform.FindChild("LeaveButton").GetComponent<Button>();
		}
		if (leaveButton == null)
		{
			Debug.Log("StepHelpMenu: Missing leave button.");
		}

		if (buttonLeft == null)
		{
			buttonLeft = transform.FindChild("ButtonLeft").GetComponent<Button>();
		}
		if (buttonLeft == null)
		{
			Debug.LogError("StepHelpMenu: Missing left button.");
		}
		
		if (buttonRight == null)
		{
			buttonRight = transform.FindChild("ButtonRight").GetComponent<Button>();
		}
		if (buttonRight == null)
		{
			Debug.LogError("StepHelpMenu: Missing right button.");
		}

		if (description == null)
		{
			description = transform.FindChild("Description").GetComponent<TextMeshWrapper>();
		}
		if (description == null)
		{
			Debug.LogError("StepHelpMenu: Missing description text.");
		}

		if (image == null)
		{
			image = transform.FindChild("Image").GetComponent<SpriteRenderer>();
		}
		if (image == null)
		{
			Debug.LogError("StepHelpMenu: Missing image.");
		}

		originalPosition = transform.position;
	}


	protected IEnumerator ModifyPage(int add, bool startInvisible = false)
	{
		if (switchingPages)
			yield break;

		switchingPages = true;

		page += add;

		if (page < 1)
		{
			page = 1;
		}

		if (!LugusResources.use.Levels.HasText(Application.loadedLevelName+".help." + page.ToString() + ".text"))	// check if there is a page to load at all
		{
			page -= add;
		}

		if (LugusResources.use.Levels.HasText(Application.loadedLevelName+".help." + (page+1).ToString() + ".text"))	// check if there is a next page to load
		{
			buttonRight.gameObject.SetActive(true);
		}
		else
		{
			buttonRight.gameObject.SetActive(false);
		}
		
		if (page == 1)
		{
			buttonLeft.gameObject.SetActive(false);
		}
		else
		{
			buttonLeft.gameObject.SetActive(true);
		}


//		iTween.Stop(gameObject);
	
//		if (!startInvisible)
//		{
//			gameObject.MoveTo(originalPosition + new Vector3(-30, 0, 0)).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();
//			yield return new WaitForSeconds(0.5f);
//		}
//
//		string key = Application.loadedLevelName+".help." + page.ToString();
//
//		description.SetText(LugusResources.use.Levels.GetText(key + ".text"));
//		
//		Sprite newImage = LugusResources.use.Shared.GetSprite(LugusResources.use.Levels.GetText(key + ".image"));
//
//		if (newImage != null && newImage != LugusResources.use.errorSprite)
//		{
//			image.gameObject.SetActive(true);
//			image.sprite = newImage;
//		}
//		else
//		{
//			image.gameObject.SetActive(false); 
//		}
//
//		transform.position = originalPosition + new Vector3(30, 0, 0);
//		
//		gameObject.MoveTo(originalPosition).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();





		float alpha = 1.0f;
		float time = 0.15f;
	
		string key = Application.loadedLevelName+".help." + page.ToString();

		if(startInvisible)
		{
			alpha = 0.0f;

			description.SetText(LugusResources.use.Levels.GetText(key + ".text"));

			print (key + ".image");

			Sprite newImage = LugusResources.use.Shared.GetSprite(LugusResources.use.Levels.GetText(key + ".image"));

			if (newImage != null && newImage != LugusResources.use.errorSprite)
			{
				image.gameObject.SetActive(true);
				image.sprite = newImage;
			}
			else
			{
				image.gameObject.SetActive(false); 
			}

			// set level invisible
			description.textMesh.color = description.textMesh.color.a(alpha);
			image.color = image.color.a(alpha);

			yield return new WaitForSeconds(0.5f);		// we want some time for other menus to disappear first 

			while(alpha < 1)
			{
				description.textMesh.color = description.textMesh.color.a(alpha);
				image.color = image.color.a(alpha);

				alpha += (1/time) * Time.deltaTime;

				yield return null;
			}
		}
		else
		{
			while(alpha > 0)
			{
				description.textMesh.color = description.textMesh.color.a(alpha);
				image.color = image.color.a(alpha);
				
				alpha -= (0.5f/time) * Time.deltaTime;	// 0.5f because we want the time variable defined above to not count double

				yield return null;
			}
			
			description.SetText(LugusResources.use.Levels.GetText(key + ".text"));

			Sprite newImage = LugusResources.use.Shared.GetSprite(LugusResources.use.Levels.GetText(key + ".image"));
			
			if (newImage != null && newImage != LugusResources.use.errorSprite)
			{
				image.gameObject.SetActive(true);
				image.sprite = newImage;
			}
			else
			{
				image.gameObject.SetActive(false);
			}

			while(alpha < 1)
			{
				description.textMesh.color = description.textMesh.color.a(alpha);
				image.color = image.color.a(alpha);
				
				alpha += (1/time) * Time.deltaTime;		// 0.5f because we want the time variable defined above to not count double
				
				yield return null;
			}
		}

		switchingPages = false;
	}
	
	public void SetupGlobal()
	{
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
		if (!activated || switchingPages)
			return;
		
		if (leaveButton.pressed)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu); 
		}
		else if (buttonRight.pressed)
		{
			LugusCoroutines.use.StartRoutine( ModifyPage(1) );
		}
		else if (buttonLeft.pressed)
		{
			LugusCoroutines.use.StartRoutine( ModifyPage(-1) );
		}
	}

	protected void LoadLevelData()
	{
		// TO DO: Set data about levels here (name, description, etc.)
	}

	public override void Activate(bool animate = true)
	{
		activated = true;
		gameObject.SetActive(true);
	
		iTween.Stop(gameObject);
		transform.position = originalPosition + new Vector3(30, 0, 0);
		gameObject.MoveTo(originalPosition).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();

		LugusCoroutines.use.StartRoutine( ModifyPage(0, true) );	// update page once
		LoadLevelData();
	}

	public override void Deactivate(bool animate = true)
	{
		activated = false;

		if (animate)
		{
			iTween.Stop(gameObject);
			gameObject.MoveTo(originalPosition + new Vector3(-30, 0, 0) ).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();
		}
		else
		{
			gameObject.SetActive(false);
		}

	}
}
