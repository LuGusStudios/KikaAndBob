using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerManager : LugusSingletonExisting<LayerManagerDefault> 
{
	
}


public class LayerManagerDefault : MonoBehaviour
{
	public LayerSpawner skyLayer = null;
	public LayerSpawner groundLayer = null;
	public LayerSpawner frontLayer = null;

	public int currentThemeIndex = 0;
	public BackgroundTheme[] themes;
	public BackgroundTheme[] themeTransitions;

	public bool autoThemeTransitions = true;
	public DataRange timeBetweenThemes = new DataRange(20.0f, 25.0f);

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
		
		if( themes.Length > 1 && themeTransitions.Length != themes.Length )
		{
			Debug.LogError(name + " : wrong number of themeTransitions found! " + themeTransitions.Length + " should be " + themes.Length );
		}
	}
	
	public void SetupGlobal()
	{
		groundLayer.baseLayer = themes[ currentThemeIndex ].ground;
		groundLayer.detailLayer = themes[ currentThemeIndex ].groundDetails;
		groundLayer.detailSpawnIntensity = themes[ currentThemeIndex ].groundDetailsIntensity; 
		groundLayer.StartSpawning();

		skyLayer.baseLayer = themes[ currentThemeIndex ].sky;
		skyLayer.detailLayer = themes[ currentThemeIndex ].skyDetails;
		skyLayer.detailSpawnIntensity = themes[ currentThemeIndex ].skyDetailsIntensity; 
		skyLayer.StartSpawning();


		frontLayer.detailLayer = themes[ currentThemeIndex ].frontDetails;
		frontLayer.detailsRandomY = false;
		frontLayer.detailSpawnIntensity = themes[ currentThemeIndex ].frontDetailsIntensity; 
		frontLayer.StartSpawning();

		LugusCoroutines.use.StartRoutine( NextThemeRoutine() );
	}

	public bool themeTransitionInProgress = false;
	public void NextTheme()
	{
		if( themeTransitionInProgress ) 
		{
			Debug.LogError (name + " : theme transition was already in progress!! " + currentThemeIndex);
			return;
		}

		if( themes.Length == 1 )
		{
			// just 1 theme : nothing to switch to...
			return;
		}

		themeTransitionInProgress = true;

		skyLayer.onSectionSwitch += OnSkyLayerTransitioned;

		skyLayer.baseLayer = themeTransitions[ currentThemeIndex ].sky;
		skyLayer.detailLayer = themeTransitions[ currentThemeIndex ].skyDetails;
		skyLayer.detailSpawnIntensity = themeTransitions[ currentThemeIndex ].skyDetailsIntensity; 
	}

	protected void OnSkyLayerTransitioned(LayerSection currentSection, LayerSection nextSection)
	{
		Debug.Log ("SkyLayer transitioned");

		// TODO: add timeout to better time transition of groundlayer with actual rendition of skylayer to transition

		skyLayer.onSectionSwitch -= OnSkyLayerTransitioned;


		// make sure sky doesn't show the transition again
		// shouldn't happen if the transitionSkyOffset is set correctly, but you can never be too sure :)
		int oneAhead = (currentThemeIndex + 1) % themes.Length;
		skyLayer.baseLayer = themes[ oneAhead ].sky;
		skyLayer.detailLayer = themes[ oneAhead ].skyDetails;
		skyLayer.detailSpawnIntensity = themes[ oneAhead ].skyDetailsIntensity; 


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
		groundLayer.detailSpawnIntensity = themeTransitions[ currentThemeIndex ].groundDetailsIntensity; 
		
		
		frontLayer.detailLayer = themeTransitions[ currentThemeIndex ].frontDetails;
		frontLayer.detailSpawnIntensity = themeTransitions[ currentThemeIndex ].frontDetailsIntensity; 
	}

	protected void OnGroundLayerTransitioned(LayerSection currentSection, LayerSection nextSection)
	{
		Debug.Log ("GroundLayer transitioned");
		
		groundLayer.onSectionSwitch -= OnGroundLayerTransitioned;

		currentThemeIndex = (currentThemeIndex + 1) % themes.Length;

		groundLayer.baseLayer = themes[ currentThemeIndex ].ground;
		groundLayer.detailLayer = themes[ currentThemeIndex ].groundDetails;
		groundLayer.detailSpawnIntensity = themes[ currentThemeIndex ].groundDetailsIntensity; 

		skyLayer.baseLayer = themes[ currentThemeIndex ].sky;
		skyLayer.detailLayer = themes[ currentThemeIndex ].skyDetails;
		skyLayer.detailSpawnIntensity = themes[ currentThemeIndex ].skyDetailsIntensity; 
		
		frontLayer.detailLayer = themes[ currentThemeIndex ].frontDetails;
		frontLayer.detailSpawnIntensity = themes[ currentThemeIndex ].frontDetailsIntensity; 
		
		themeTransitionInProgress = false;
		
		LugusCoroutines.use.StartRoutine( NextThemeRoutine() );
	}

	protected IEnumerator NextThemeRoutine()
	{
		if( !autoThemeTransitions )
			yield break;

		yield return new WaitForSeconds( timeBetweenThemes.Random () );

		NextTheme(); 
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

		if( /*LugusDebug.debug &&*/ !themeTransitionInProgress )
		{
			//NextTheme();
		}
	}
}
