using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerManager : LugusSingletonExisting<LayerManagerDefault> 
{
	
}


public class LayerManagerDefault : LugusSingletonExisting<LayerManagerDefault> 
{
	public LayerSpawner skyLayer = null;
	public LayerSpawner groundLayer = null;
	public LayerSpawner frontLayer = null;

	public int currentThemeIndex = 0;
	public BackgroundTheme[] themes;
	public BackgroundTheme[] themeTransitions;

	public BackgroundTheme CurrentTheme
	{
		get{ return themes[currentThemeIndex]; }
	}

	// if camera is this many units from the center of the sky transition, it will transition the ground as well
	// in testing, 20.0f was a good value
	public float transitionSkyOffset = 20.0f; 

	public void SetupLocal()
	{
		if( skyLayer == null )
		{
			skyLayer = GameObject.Find ("LayerSky").GetComponent<LayerSpawner>();
		}

		if( skyLayer == null )
		{
			Debug.LogError(name + " : no LayerSky found!");
		}
		
		if( groundLayer == null )
		{
			groundLayer = GameObject.Find ("LayerGround").GetComponent<LayerSpawner>();
		}
		
		if( groundLayer == null )
		{
			Debug.LogError(name + " : no groundLayer found!");
		}
		
		if( frontLayer == null )
		{
			frontLayer = GameObject.Find ("LayerFront").GetComponent<LayerSpawner>();
		}
		
		if( frontLayer == null )
		{
			Debug.LogError(name + " : no frontLayer found!");
		}

		if( themes.Length == 0 )
		{
			Debug.LogError(name + " : no themes found!");
		}
		
		if( themeTransitions.Length == 0 || themeTransitions.Length != themes.Length )
		{
			Debug.LogError(name + " : wrong number of themeTransitions found! " + themeTransitions.Length + " should be " + themes.Length );
		}
	}
	
	public void SetupGlobal()
	{
		groundLayer.baseLayer = themes[ currentThemeIndex ].ground;
		groundLayer.detailLayer = themes[ currentThemeIndex ].groundDetails;
		groundLayer.StartSpawning();

		skyLayer.baseLayer = themes[ currentThemeIndex ].sky;
		skyLayer.detailLayer = themes[ currentThemeIndex ].skyDetails;
		skyLayer.StartSpawning();


		frontLayer.detailLayer = themes[ currentThemeIndex ].frontDetails;
		frontLayer.detailsRandomY = false;
		frontLayer.StartSpawning();
	}

	protected bool themeTransitionInProgress = false;
	public void NextTheme()
	{
		if( themeTransitionInProgress ) 
		{
			Debug.LogError (name + " : theme transition was already in progress!! " + currentThemeIndex);
			return;
		}

		themeTransitionInProgress = true;

		skyLayer.onSectionSwitch += OnSkyLayerTransitioned;

		skyLayer.baseLayer = themeTransitions[ currentThemeIndex ].sky;
		skyLayer.detailLayer = themeTransitions[ currentThemeIndex ].skyDetails;
	}

	protected void OnSkyLayerTransitioned(LayerSection currentSection, LayerSection nextSection)
	{
		Debug.Log ("SkyLayer transitioned");

		// TODO: add timeout to better time transition of groundlayer with actual rendition of skylayer to transition

		skyLayer.onSectionSwitch -= OnSkyLayerTransitioned;

		LugusCoroutines.use.StartRoutine( GroundTransitionRoutine(nextSection) );
	}

	protected IEnumerator GroundTransitionRoutine(LayerSection transitionSection)
	{
		//yield return new WaitForSeconds( timeBetweenSkyAndGroundTransitions );

		bool transition = false;
		while( !transition )
		{
			yield return new WaitForSeconds(0.1f);

			float distance = Mathf.Abs( LugusCamera.game.transform.position.x - transitionSection.transform.position.x );

			//Debug.Log ("Distance to camera : " + distance);

			if( distance < transitionSkyOffset ) 
				transition = true;
		}

		Debug.Log ("Starting ground transitioning");

		groundLayer.onSectionSwitch += OnGroundLayerTransitioned;
		
		groundLayer.baseLayer = themeTransitions[ currentThemeIndex ].ground;
		groundLayer.detailLayer = themeTransitions[ currentThemeIndex ].groundDetails;
		
		
		frontLayer.detailLayer = themeTransitions[ currentThemeIndex ].frontDetails;
	}

	protected void OnGroundLayerTransitioned(LayerSection currentSection, LayerSection nextSection)
	{
		Debug.Log ("GroundLayer transitioned");
		
		groundLayer.onSectionSwitch -= OnGroundLayerTransitioned;

		currentThemeIndex = (currentThemeIndex + 1) % themes.Length;

		groundLayer.baseLayer = themes[ currentThemeIndex ].ground;
		groundLayer.detailLayer = themes[ currentThemeIndex ].groundDetails;
		skyLayer.baseLayer = themes[ currentThemeIndex ].sky;
		skyLayer.detailLayer = themes[ currentThemeIndex ].skyDetails;
		
		frontLayer.detailLayer = themeTransitions[ currentThemeIndex ].frontDetails;
		
		themeTransitionInProgress = false;
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
		if( LugusInput.use.KeyDown(KeyCode.N) )
		{
			NextTheme();
		}

		if( LugusDebug.debug && !themeTransitionInProgress )
		{
			//NextTheme();
		}
	}
}
