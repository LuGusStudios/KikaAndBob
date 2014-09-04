using UnityEngine;
using System.Collections;

public class LoadLanguage : MonoBehaviour {

	protected void Start () 
	{
		SetupGlobal();
	}

	public void SetupGlobal()
	{
		string pickedLanguage = LugusConfig.use.User.GetString("main.settings.langID", LugusResources.use.GetSystemLanguageID());
		LugusResources.use.ChangeLanguage(pickedLanguage);
	}

}
