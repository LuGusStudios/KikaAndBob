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

		scoreText.GetComponent<TextMesh>().text = "" + score._amount;
		scoreText.GetComponent<TextMesh>().color = score._color;


		if( !string.IsNullOrEmpty(score._audioKey) )
		{
			LugusAudio.use.SFX().Play( LugusResources.use.GetAudio(score._audioKey) );
		}

		if( score._audioClip != null )
		{
			LugusAudio.use.SFX().Play( score._audioClip );
		}

		Vector2 screenPos = LugusCamera.game.WorldToScreenPoint( score._worldPosition );
		scoreText.transform.localPosition = (screenPos.v3 () / 100.0f);
		scoreText.transform.position = scoreText.transform.position.z( score.GetHUDElement().transform.position.z );

		
		scoreText.MoveTo( score.GetHUDElement().transform.position ).Time ( score._duration ).EaseType(iTween.EaseType.easeInBack).Execute(); 
		GameObject.Destroy(scoreText, score._duration);

		yield return new WaitForSeconds( score._duration );

		ApplyScore( score );
		yield break;
	} 

	protected void ApplyScore(Score score)
	{
		IHUDElement hud = score.GetHUDElement();

		hud.AddValue( score._amount, score._animate );


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
