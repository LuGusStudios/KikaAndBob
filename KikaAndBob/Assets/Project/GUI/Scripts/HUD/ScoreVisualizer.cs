using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KikaAndBob
{
	public enum CommodityType
	{
		NONE = -1,

		Money = 1,
		Feather = 2,
		Time = 3,
		Life = 4,
		Score = 5,
		Distance = 6
	}
}

public class Score
{
	public float _amount = 0.0f;
	public KikaAndBob.CommodityType _commodity = KikaAndBob.CommodityType.NONE;
	public float _duration = 0.75f;
	public bool _animate = true; 

	public string _audioKey = "";
	public AudioClip _audioClip = null;

	public Vector3 _worldPosition = Vector3.zero;
	public Color _color = UnityEngine.Color.white;

	public string _text = ""; // normally we show the _amount. This can override that.
	public string _title = ""; // adds a second line of text above the score

	public float _minAmount = Mathf.NegativeInfinity;
	public float _maxAmount = Mathf.Infinity;

	protected IHUDElement _hud = null;
	public IHUDElement GetHUDElement()
	{
		if( _hud == null )
		{
			_hud = HUDManager.use.GetElementForCommodity(_commodity);

			if( _hud == null )
			{
				Debug.LogError("Score : Could not find IHUDELement for commodity " + _commodity);
				return null;
			}
		}

		return _hud;
	}

	public Score HUDElement(IHUDElement element)
	{
		_hud = element;
		return this;
	}

	// indicates whether the WorldCoordinates of Position are for the Game or the GUI camera
	public bool _useGUICamera = false;
	public Score UseGUICamera(bool useGUI)
	{
		_useGUICamera = useGUI;
		return this;
	}

	public Score Reset()
	{
		_amount = 0.0f;
		_commodity = KikaAndBob.CommodityType.NONE;
		_duration = 1.0f;

		_animate = true;
		_audioKey = "";
		_audioClip = null;

		_worldPosition = Vector3.zero;
		_color = UnityEngine.Color.white;

		_useGUICamera = false;

		_hud = null;

		return this;
	}

	public Score(KikaAndBob.CommodityType commodity, float amount)
	{
		_commodity = commodity;
		_amount = amount;
	}

	public Score Time(float duration)
	{
		_duration = duration;
		return this;
	}

	public Score Direct(bool direct)
	{
		_animate = !direct;
		return this;
	}
	
	public Score Animate(bool animate)
	{
		_animate = animate;
		return this;
	}
	
	public Score Audio(string key)
	{
		_audioKey = key;
		return this;
	}
	
	public Score Audio(AudioClip clip)
	{
		_audioClip = clip;
		return this;
	}

	public Score Position(Vector3 worldPosition)
	{
		_worldPosition = worldPosition;
		return this;
	}

	public Score Color(Color color)
	{
		_color = color;
		return this;
	}

	public Score Text(string text)
	{
		_text = text;
		return this;
	}

	public Score TextKey(string textKey)
	{
		_text = LugusResources.use.Localized.GetText(textKey);
		return this;
	}

	public Score Title(string title)
	{
		_title = title;
		return this;
	}
	
	public Score TitleKey(string titleKey)
	{
		_title = LugusResources.use.Localized.GetText(titleKey);
		return this;
	}

	public Score MinValue(float minAmount)
	{
		_minAmount = minAmount;
		return this;
	}

	public Score MaxValue(float maxAmount)
	{
		_maxAmount = maxAmount;
		return this;
	}

	public void Execute()
	{
		ScoreVisualizer.use.ShowScore( this );
	}
}

public class ScoreVisualizer : LugusSingletonRuntime<ScoreVisualizer> 
{
	public GameObject ScorePopupPrefab = null;

	public static Score Score(KikaAndBob.CommodityType commodity, float amount)
	{
		return ScoreVisualizer.use.NewScore(commodity, amount);
	}

	public Score NewScore(KikaAndBob.CommodityType commodity, float amount)
	{
		// TODO: use pool!
		return new Score(commodity, amount);
	}

	public void ShowScore(Score score)
	{
		if( score.GetHUDElement() == null )
			return;

		if( !score._animate || score._duration == 0.0f )
		{
			ApplyScore( score );
		}
		else
		{
			LugusCoroutines.use.StartRoutine( ShowScoreRoutine(score) );
		}
	}

	protected IEnumerator ShowScoreRoutine(Score score)
	{
		GameObject scoreText = (GameObject) GameObject.Instantiate( ScorePopupPrefab );
		scoreText.transform.parent = HUDManager.use.transform;
		scoreText.layer = this.gameObject.layer;

		if( string.IsNullOrEmpty(score._text) )
			scoreText.GetComponent<TextMesh>().text = "" + score._amount;
		else
			scoreText.GetComponent<TextMesh>().text = "" + score._text;

		scoreText.GetComponent<TextMesh>().color = score._color;


		if( !string.IsNullOrEmpty(score._audioKey) )
		{
			LugusAudio.use.SFX().Play( LugusResources.use.Shared.GetAudio(score._audioKey) );
		}

		if( score._audioClip != null )
		{
			LugusAudio.use.SFX().Play( score._audioClip );
		}

		Vector2 viewportCoords = LugusCamera.game.WorldToViewportPoint( score._worldPosition );
		if( score._useGUICamera )
		{
			viewportCoords = LugusCamera.ui.WorldToViewportPoint( score._worldPosition ); 
		}
		viewportCoords = Vector2.Scale( viewportCoords, new Vector2(LugusUtil.UIWidth, LugusUtil.UIHeight) );

		scoreText.transform.localPosition = (viewportCoords.v3 () );// / 100.0f);
		scoreText.transform.position = scoreText.transform.position.z( score.GetHUDElement().transform.position.z );

		float moveDuration = score._duration;
		float moveOverDelay = 0.0f;
		if( score._duration > 1.0f )
		{
			moveDuration = 1.0f;
			moveOverDelay = score._duration - 1.0f;
			scoreText.MoveTo( scoreText.transform.position.yAdd(1.0f) ).Time ( score._duration - 1.0f - 0.1f ).Execute();
		}

		scoreText.MoveTo( score.GetHUDElement().transform.position ).Delay(moveOverDelay).Time ( moveDuration ).EaseType(iTween.EaseType.easeInBack).Execute(); 

		GameObject.Destroy(scoreText, score._duration);

		if( !string.IsNullOrEmpty(score._title) )
		{
			GameObject titleText = (GameObject) GameObject.Instantiate( ScorePopupPrefab );
			titleText.transform.parent = HUDManager.use.transform;
			titleText.layer = this.gameObject.layer;

			titleText.GetComponent<TextMesh>().text = "" + score._title;
			titleText.GetComponent<TextMesh>().color = score._color;


			titleText.transform.localPosition = scoreText.transform.localPosition.yAdd( titleText.renderer.bounds.size.y + (titleText.renderer.bounds.size.y / 2.0f) );
			//titleText.transform.position = scoreText.transform.position.z( score.GetHUDElement().transform.position.z );
			
			
			if( score._duration > 1.0f )
			{
				titleText.MoveTo( titleText.transform.position.yAdd(1.0f) ).Time ( score._duration - 1.0f - 0.1f ).Execute();
			}

			titleText.MoveTo( score.GetHUDElement().transform.position ).Delay(moveOverDelay).Time ( moveDuration ).EaseType(iTween.EaseType.easeInBack).Execute(); 
			GameObject.Destroy(titleText, score._duration);
		}

		yield return new WaitForSeconds( score._duration );

		ApplyScore( score );
		yield break;
	} 

	protected void ApplyScore(Score score)
	{
		IHUDElement hud = score.GetHUDElement();

		hud.AddValue( score._amount, score._animate, score._minAmount, score._maxAmount );


		// TODO: recycle score object 
		score.Reset();
		//GameObject.Destroy( score );
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( ScorePopupPrefab == null )
		{
			ScorePopupPrefab = transform.FindChild("ScorePopupPrefab").gameObject;
		}

		if( ScorePopupPrefab == null )
		{
			Debug.LogError(transform.Path () + " : no ScorePopupPrefab found!" ); 
		}
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
