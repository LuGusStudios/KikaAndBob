using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepGames : IMenuStep 
{
	public float maxDistance = 1.5f;

	protected Button exitButton = null;
	protected Button playButton = null;
	protected Transform locationParent = null;
	protected BoxCollider2D mapCollider = null;
	protected Transform selected = null;
	protected TextMeshWrapper selectedText = null;
	protected List<Transform> selectOptions = new List<Transform>();
	protected Button leftButton = null;
	protected Button rightButton = null;
	protected Transform picture = null;
	protected Transform thumbTack = null;
	protected Vector3 pictureStartPosition = new Vector3(-6.681557f, -4.6338f, -1f);
	protected bool leavingMenu = false;
	protected SpriteRenderer pictureContent = null;

	public void SetupLocal()
	{
		if (thumbTack == null)
			thumbTack = transform.FindChild("Thumbtack");

		if (thumbTack == null)
			Debug.LogError("MenuStepGames: Missing thumbtack");

		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepGames: Missing exit button.");

		if (locationParent == null)
			locationParent = transform.FindChild("LocationParent");

		if (locationParent == null)
			Debug.LogError("MenuStepGames: Missing location parent.");

		if (mapCollider == null)
			mapCollider = transform.FindChild("MapCollider").GetComponent<BoxCollider2D>();

		if (mapCollider == null)
			Debug.LogError("MenuStepGames: Missing map collider.");

		if (picture == null)
			picture = transform.FindChild("Photograph");

		if (picture == null)
			Debug.LogError("MenuStepGames: Missing picture transform.");

		if (pictureContent == null)
			pictureContent = picture.FindChild("PictureContent").GetComponent<SpriteRenderer>();
		
		if (pictureContent == null)
			Debug.LogError("MenuStepGames: Missing picture content sprite renderer.");

		if (selectedText == null)
			selectedText = picture.FindChild("PhotographText").GetComponent<TextMeshWrapper>();
		
		if (selectedText == null)
			Debug.LogError("MenuStepGames: Missing selected option text.");

		if (leftButton == null)
			leftButton = picture.FindChild("ArrowLeft").GetComponent<Button>();
		
		if (leftButton == null)
			Debug.LogError("MenuStepGames: Missing left button.");

		if (rightButton == null)
			rightButton = picture.FindChild("ArrowRight").GetComponent<Button>();
		
		if (rightButton == null)
			Debug.LogError("MenuStepGames: Missing right button.");

		if (playButton == null)
			playButton = picture.FindChild("ButtonExit").GetComponent<Button>();
		
		if (playButton == null)
			Debug.LogError("MenuStepGames: Missing play button.");

		if (selectOptions.Count <= 0)
		{
			foreach(Transform t in locationParent)
			{
				selectOptions.Add(t);
			}
		}
	}
	
	public void SetupGlobal()
	{
		UpdateMapSelection("", false);
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public override void Activate (bool animate)
	{
		activated = true;
		
		this.gameObject.SetActive(true);
		
		//LugusDebug.debug = true;
	}

	public override void Deactivate (bool animate)
	{
		activated = false;
		
		this.gameObject.SetActive(false);
		
		//LugusDebug.debug = false;
	}
	
	protected void Update () 
	{
		if (leavingMenu)
			return;

		if (exitButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);
		}

		if (!activated)
			return;

		MapSelect();

		if (rightButton.pressed)
			ModifyPictureIndex(1);
		else if (leftButton.pressed)
			ModifyPictureIndex(-1);

		if (playButton.pressed)
			StartCoroutine(LoadingGameRoutine());
	}

	protected IEnumerator LoadingGameRoutine()
	{
		if (selected == null)
		{
			leavingMenu = false;
			yield break;
		}

		leavingMenu = true;
		
		ScreenFader.use.FadeOut(0.5f);
		
		yield return new WaitForSeconds(0.5f);

		Resources.UnloadUnusedAssets();
		
		Application.LoadLevel(selected.name);
		
		yield break;
	}

	// this is necessary because the parent of this object scales due an iTween animation - if the wrapper is only calculated once, the results are incorrect after the iTween finishes
	protected IEnumerator UpdateTextSizeWhileAnimating(float time)
	{
		selectedText.autoUpdate = true;

		yield return new WaitForSeconds(time);

		selectedText.autoUpdate = false;

		yield break;
	}

	protected void MapSelect()
	{
		if (!LugusInput.use.down)
			return;

		Transform hit = LugusInput.use.RayCastFromMouseDown();

		if (hit != mapCollider.transform)
			return;

		Vector3 mousePosition = LugusInput.use.ScreenTo3DPoint(mapCollider.transform);

		float smallestDistance = Mathf.Infinity;

		Transform savedSelected = selected;
		selected = null;

		foreach(Transform t in locationParent)
		{
			float currentDistance = Vector3.Distance(t.position, mousePosition);

			if (currentDistance < smallestDistance && currentDistance < maxDistance)
			{
				smallestDistance = currentDistance;
				selected = t;
			}
		}

		if (selected == null)
			selected = savedSelected;

		if (selected != null && savedSelected != selected)
		{
			UpdateMapSelection(selected.name, true);
		}
	}

	protected void UpdateMapSelection(string key, bool animate)
	{
		if (string.IsNullOrEmpty(key))
		{
			picture.gameObject.SetActive(false);

			if (animate)
			{
				picture.transform.localPosition = pictureStartPosition + new Vector3(-15, 20, 0);
				picture.transform.localScale = Vector3.one;
			}
			return;
		}
		else
		{
			picture.gameObject.SetActive(true);

			if (animate)
			{
				iTween.Stop(picture.gameObject);

				picture.transform.localPosition = pictureStartPosition + new Vector3(-15, 20, 0);
				picture.gameObject.MoveTo(pictureStartPosition).Time(0.5f).Delay(0.25f).EaseType(iTween.EaseType.linear).Execute();

				picture.transform.localScale = Vector3.one * 3f;
				picture.gameObject.ScaleTo(Vector3.one).Time(0.5f).Execute();

				StartCoroutine(UpdateTextSizeWhileAnimating(0.5f));

				iTween.Stop(thumbTack.gameObject);
				
				Vector3 targetPosition = selected.position.z(thumbTack.position.z);

				thumbTack.localScale = Vector3.one * 5;
				thumbTack.gameObject.ScaleTo(Vector3.one).Time(0.5f).Execute();
				thumbTack.position = targetPosition + new Vector3(0, 20, 0);
				thumbTack.gameObject.MoveTo(targetPosition).Time(0.5f).Execute();

//				iTween.Stop(leftButton.gameObject);
//				iTween.Stop(rightButton.gameObject);

//					leftButton.transform.localScale = Vector3.zero;
//					rightButton.transform.localScale = Vector3.zero;
//
//				leftButton.gameObject.ScaleTo(Vector3.one).Time(0.5f).Delay(0.5f).EaseType(iTween.EaseType.linear).Execute();
//				rightButton.gameObject.ScaleTo(Vector3.one).Time(0.5f).Delay(0.5f).EaseType(iTween.EaseType.linear).Execute();
			}
			else
			{
				picture.transform.localPosition = pictureStartPosition;
				thumbTack.position = selected.position.z(thumbTack.position.z);

//				leftButton.transform.localScale = Vector3.one;
//				rightButton.transform.localScale = Vector3.one;
			}
		}

		selectedText.SetText(LugusResources.use.Levels.GetText(key + ".main.title"));
		pictureContent.sprite = LugusResources.use.Shared.GetSprite(key + "_Main_Image");

		// NOTE: These lines are just to somewhat scale/reposition the pictures currently being used, i.e. the same ones as on the respective games' main menu pages.
		// They don't have the same aspect ratio as the frame in the picture, so we scale and move them a bit for now.
		// Can be removed if/when more fitting pictures are made.
		pictureContent.transform.localScale = new Vector3(0.8f, 0.85f, 1f);
		pictureContent.transform.localPosition = new Vector3(0, 0.23f, 0.1f);
	}

	protected void ModifyPictureIndex(int amount)
	{
		int index = selectOptions.IndexOf(selected);

		if (index >= 0)
		{
			index += amount;

			if (index < 0)
			{
				index = selectOptions.Count - 1;
			}
			else if (index >= selectOptions.Count)
			{
				index = index - selectOptions.Count;
			}

			selected = selectOptions[index];

			UpdateMapSelection(selected.name, false);
		}
	}
}
