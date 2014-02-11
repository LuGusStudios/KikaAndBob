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

	public HUDCounter Counter1 = null;
	public HUDCounter Counter2 = null;
	public HUDCounter Counter3 = null;
	public HUDCounter Counter4 = null;

	protected Vector3 originalPosition = Vector3.zero;

	public void Show(bool success)
	{
		if( success )
		{
			// TODO: show happy kika
		}
		else
		{
			// TODO: show angry kika!
		}


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

		Counter1.gameObject.SetActive( false );
		Counter2.gameObject.SetActive( false );
		Counter3.gameObject.SetActive( false );
		Counter4.gameObject.SetActive( false );

		
		ContinueButton 	= transform.FindChild("ContinueButton").GetComponent<Button>();
		QuitButton 		= transform.FindChild("QuitButton").GetComponent<Button>();
		RetryButton 	= transform.FindChild("RetryButton").GetComponent<Button>();

		Subtitle = transform.FindChild("Subtitle").GetComponent<TextMeshWrapper>();
		Title    = transform.FindChild("Title").GetComponent<TextMeshWrapper>();
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
	
	}
}
