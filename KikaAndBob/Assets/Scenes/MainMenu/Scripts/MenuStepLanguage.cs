using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepLanguage : IMenuStep 
{
	public bool doNotHighlightCurrent = false;	// if true, upon rebuilding, current language will not be unclickable - basically only useful for the first run of the game, when the player hasn't selected a language yet

	protected Button exitButton = null;
	protected TextMeshWrapper langNameText = null;
	protected Transform languageSelectorParent = null;
	protected Transform buttonPrefab = null;
	protected Dictionary<Button, string> buttons = new Dictionary<Button, string>();

	public void SetupLocal()
	{
		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepLanguage: Missing exit button.");

		if (languageSelectorParent == null)
		{
			languageSelectorParent = transform.FindChild("LanguageSelector");
		}

		if (languageSelectorParent == null)
		{
			Debug.Log("MenuStepLanguage: Missing selector parent.");
		}

		if (buttonPrefab == null)
		{
			buttonPrefab = transform.FindChild("ButtonPrefab");
		}
		
		if (buttonPrefab == null)
		{
			Debug.Log("MenuStepLanguage: Missing button prefab.");
		}
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
		if (!activated)
			return;

		if (exitButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Settings);
		}

		foreach(KeyValuePair<Button, string> button in buttons)
		{
			if (button.Key.pressed)
			{
				LugusResources.use.ChangeLanguage(button.Value);

				LugusConfig.use.System.SetString("main.settings.langID", button.Value, true);

				BuildLanguageSelector();
				break;
			}
		}
	}

	public override void Activate (bool animate)
	{
		activated = true;
		this.gameObject.SetActive(true);

		BuildLanguageSelector();
	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);

		// save settings
		LugusConfig.use.SaveProfiles();
	}
	
	protected void BuildLanguageSelector()
	{
		foreach(KeyValuePair<Button, string> button in buttons)
		{
			Destroy(button.Key.gameObject);
		}

		buttons.Clear();

		float yOffset = 0;

		for (int i = 1; i <= 32; i++) 
		{
			string nameKey = "lang.lang" + i.ToString() + ".name";

			string name = LugusResources.use.Shared.GetText(nameKey);

			if (name == "[" + nameKey + "]")
				return;

			Transform newButton = (Transform) Instantiate(buttonPrefab);

			newButton.parent = languageSelectorParent;
			newButton.localPosition = Vector3.zero.y(yOffset);

			string idKey = "lang.lang" + i.ToString() + ".id";
			newButton.GetComponentInChildren<TextMeshWrapper>().SetText(name);

			string id = LugusResources.use.Shared.GetText(idKey);

			yOffset -= 1.3f;

			if (id == LugusResources.use.GetLocalizedLangID() && doNotHighlightCurrent == false)		// disable current language button
			{
				newButton.GetComponent<Collider>().enabled = false;
				newButton.transform.localScale = newButton.transform.localScale * 1.1f;
			}
			else
			{
				foreach(SpriteRenderer r in newButton.GetComponentsInChildren<SpriteRenderer>())
				{
					r.color = r.color.a(0.7f);
				}
			}

			buttons.Add(newButton.GetComponent<Button>(), id);

		}
	}
}
