using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEndScreen : MonoBehaviour 
{
	public Button ContinueButton = null;
	public Button QuitButton = null;
	public Button RetryButton = null;

	public TextMeshWrapper Subtitle = null;
	public TextMeshWrapper Title = null;
	public TextMeshWrapper Message = null;

	public HUDCounter Counter1 = null;
	public HUDCounter Counter2 = null;
	public HUDCounter Counter3 = null;
	public HUDCounter Counter4 = null;
	public HUDCounter Counter5 = null;
	public HUDCounter Counter6 = null;

	public Vector3 originalPosition = Vector3.zero; 

	public RunnerCharacterAnimator Kika = null;

	protected GameObject cameraFade = null;
	
	protected List<IHUDElement> elements = new List<IHUDElement>();
	public IHUDElement GetCounterForCommodity(KikaAndBob.CommodityType commodity)
	{
		foreach( IHUDElement element in elements )
		{
			if( element.gameObject.activeSelf && element.commodity == commodity )
				return element;
		}
		
		return null;
	}

	public void Show(bool success, float delay = 0) 
	{
		HUDManager.use.DisableAll();
		HUDManager.use.PauseButton.gameObject.SetActive(false);

		//LugusAudio.use.Music().StopAll();
		LugusAudio.use.Music().CrossFade(LugusResources.use.Shared.GetAudio("Endscreen01"), 1.0f);

		if( success )
		{
			Kika.PlayAnimation("HAPPY/KikaFront_Victory");

			Title.SetTextKey( "global.levelend.success.title" );
			Message.SetTextKey( "global.levelend.success.message" );
		}
		else
		{
			Kika.PlayAnimation("SAD/KikaSad");

			Title.SetTextKey( "global.levelend.failure.title" );
			Message.SetTextKey( "global.levelend.failure.message" );
		}

		//Debug.LogError("LevelEndScreen SHOW " + originalPosition);

		transform.position = originalPosition + new Vector3(30, 0, 0);	// first put menu closer to the edge of the screen; otherwise it'll jerk into view

		gameObject.MoveTo(originalPosition).Time(0.5f).Delay(delay).EaseType(iTween.EaseType.easeOutBack).Execute();

		//transform.position = originalPosition; 
	}

	public void Hide(bool animate = false)
	{
		//this.gameObject.SetActive(false); 

		if (animate)
			gameObject.MoveTo(originalPosition + new Vector3(-30, 0, 0)).Time(0.5f).EaseType(iTween.EaseType.easeInBack).Execute();
		else
			transform.position = new Vector3(9999.0f, 9999.0f, 9999.0f);
	}

	public void SetupLocal()
	{
		originalPosition = this.transform.position;

		// assign variables that have to do with this class only
		
		Counter1 = transform.FindChild("Counter1").GetComponent<HUDCounter>();
		Counter2 = transform.FindChild("Counter2").GetComponent<HUDCounter>();
		Counter3 = transform.FindChild("Counter3").GetComponent<HUDCounter>();
		Counter4 = transform.FindChild("Counter4").GetComponent<HUDCounter>();
		Counter5 = transform.FindChild("Counter5").GetComponent<HUDCounter>();
		Counter6 = transform.FindChild("Counter6").GetComponent<HUDCounter>();

		Counter1.gameObject.SetActive( false );
		Counter2.gameObject.SetActive( false );
		Counter3.gameObject.SetActive( false );
		Counter4.gameObject.SetActive( false );
		Counter5.gameObject.SetActive( false );
		Counter6.gameObject.SetActive( false );
		
		elements.Add ( Counter1 );
		elements.Add ( Counter2 );
		elements.Add ( Counter3 );
		elements.Add ( Counter4 );
		elements.Add ( Counter5 );
		elements.Add ( Counter6 );

		
		ContinueButton 	= transform.FindChild("ContinueButton").GetComponent<Button>();
		QuitButton 		= transform.FindChild("QuitButton").GetComponent<Button>();
		RetryButton 	= transform.FindChild("RetryButton").GetComponent<Button>();

		Subtitle = transform.FindChild("Subtitle").GetComponent<TextMeshWrapper>();
		Title    = transform.FindChild("Title").GetComponent<TextMeshWrapper>();
		Message    = transform.FindChild("Message").GetComponent<TextMeshWrapper>();

		Kika = transform.FindChild("Kika").GetComponent<RunnerCharacterAnimator>();
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

	private bool locked = false;
	protected IEnumerator ContinueButtonRoutine()
	{
		HUDManager.use.DisableAll();
		HUDManager.use.PauseButton.gameObject.SetActive(false);
		DialogueManager.use.HideAll();

		yield return StartCoroutine(ScreenHideRoutine());

		LugusAudio.use.Music().StopAll();
		MenuManager.use.ActivateMenu( MenuManagerDefault.MenuTypes.LevelMenu );

		locked = false;

		ScreenFader.use.FadeIn(0.5f);
	}

	protected IEnumerator RetryButtonRoutine()
	{
		yield return StartCoroutine(ScreenHideRoutine());
		
		IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
		manager.ReloadLevel();
	}

	protected IEnumerator QuitButtonRoutine()
	{
		yield return StartCoroutine(ScreenHideRoutine());
		
		IMinigameCrossSceneInfo info = LevelLoaderDefault.GetCrossSceneInfo();
		info.SetLevelIndex(-1); 
		
		Application.LoadLevel( Application.loadedLevelName );
	}

	protected IEnumerator ScreenHideRoutine()
	{
		locked = true;

		HUDManager.use.LevelEndScreen.Hide (true); // animate boolean will make this screen fly away

		yield return new WaitForSeconds(0.5f);

		ScreenFader.use.FadeOut(0.5f);

		yield return new WaitForSeconds(0.5f);

		locked = false;
	}
	
	protected void Update () 
	{
		if (locked)
			return;

		if( ContinueButton.pressed ) 
		{
			LugusCoroutines.use.StartRoutine(ContinueButtonRoutine());
		}
		
		if( RetryButton.pressed ) 
		{
			LugusCoroutines.use.StartRoutine(RetryButtonRoutine());
		}
		
		if( QuitButton.pressed )
		{
			LugusCoroutines.use.StartRoutine(QuitButtonRoutine());
		}
	}
}
