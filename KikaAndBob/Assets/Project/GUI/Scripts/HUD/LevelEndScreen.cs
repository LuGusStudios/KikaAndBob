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

	public void Show(bool success) 
	{
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

		Debug.LogError("LevelEndScreen SHOW " + originalPosition);

		transform.position = originalPosition; 
	}

	public void Hide()
	{
		//this.gameObject.SetActive(false); 
		
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
	
	protected void Update () 
	{
		if( ContinueButton.pressed )
		{
			MenuManager.use.ActivateMenu( MenuManagerDefault.MenuTypes.LevelMenu );
		}
		
		if( RetryButton.pressed ) 
		{
			IGameManager manager = GameObject.FindObjectOfType<IGameManager>();
			manager.ReloadLevel();
		}
		
		if( QuitButton.pressed )
		{
			
			IMinigameCrossSceneInfo info = LevelLoaderDefault.GetCrossSceneInfo();
			info.SetLevelIndex(-1); 
			
			Application.LoadLevel( Application.loadedLevelName );
		}
	}
}
