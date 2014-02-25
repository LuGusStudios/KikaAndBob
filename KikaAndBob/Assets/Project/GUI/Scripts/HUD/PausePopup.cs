using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PausePopup : MonoBehaviour 
{
	public Button ContinueButton = null;
	public Button QuitButton = null;
	public Button RetryButton = null;
	public Button HelpButton = null;
	public Button SettingsButton = null;

	public Transform screenCollider = null;

	public void Show()
	{
		this.gameObject.SetActive(true);

		IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
		manager.Paused = true;

		HUDManager.use.PauseButton.gameObject.SetActive(false);
	}

	public void Hide()
	{
		this.gameObject.SetActive(false);

		IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
		manager.Paused = false;

		HUDManager.use.PauseButton.gameObject.SetActive(true);
		HUDManager.use.PauseButton.Appear();
	}

	public void SetupLocal()
	{ 
		// assign variables that have to do with this class only
		ContinueButton 	= transform.FindChild("ContinueButton").GetComponent<Button>();
		QuitButton 		= transform.FindChild("QuitButton").GetComponent<Button>();
		RetryButton 	= transform.FindChild("RetryButton").GetComponent<Button>();
		HelpButton 		= transform.FindChild("HelpButton").GetComponent<Button>();
		SettingsButton 	= transform.FindChild("SettingsButton").GetComponent<Button>();

		screenCollider = transform.FindChild("Collider");
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	protected IEnumerator RetryButtonRoutine()
	{
		yield return StartCoroutine(ScreenHideRoutine());
		
		IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
		manager.Paused = false;
		manager.ReloadLevel();
	}
	
	protected IEnumerator QuitButtonRoutine()
	{
		yield return StartCoroutine(ScreenHideRoutine());

		IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
		manager.Paused = false;
		
		IMinigameCrossSceneInfo info = LevelLoaderDefault.GetCrossSceneInfo();
		info.SetLevelIndex(-1);
		
		Application.LoadLevel( Application.loadedLevelName );
	}

	private bool locked = false;

	protected IEnumerator ScreenHideRoutine()
	{
		locked = true;
		
		ScreenFader.use.FadeOut(0.5f);
		
		yield return new WaitForSeconds(0.5f);
		
		locked = false;
	}
	
	protected void Update () 
	{
		if (locked)
			return;

		Transform hit = LugusInput.use.RayCastFromMouseDown();
		if( hit == screenCollider || ContinueButton.pressed )
		{
			Hide ();
		}

		if( RetryButton.pressed )
		{
			//LugusCoroutines.use.StartRoutine(RetryButtonRoutine());

			IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
			manager.Paused = false;
			manager.ReloadLevel();
		}

		if( QuitButton.pressed )
		{
			//LugusCoroutines.use.StartRoutine(QuitButtonRoutine());

			IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
			manager.Paused = false;
			
			IMinigameCrossSceneInfo info = LevelLoaderDefault.GetCrossSceneInfo();
			info.SetLevelIndex(-1);
			
			Application.LoadLevel( Application.loadedLevelName );
		}
		
		
		if( HelpButton.pressed )
		{
			Debug.LogError("TODO : HELP MENU");
		}

		if( SettingsButton.pressed )
		{
			Debug.LogError("TODO : SETTINGS MENU");
		}
	}
}
