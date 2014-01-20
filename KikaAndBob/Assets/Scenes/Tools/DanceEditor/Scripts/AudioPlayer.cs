using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : LugusSingletonRuntime<AudioPlayer>
{
	public Vector2 screenOffset = new Vector2(10, 10);
	public AudioSource Source
	{
		get
		{
			return _source;
		}

		set
		{
			_source = value;
		}
	}
	public float SeekTime
	{
		get
		{
			return _seekTime;
		}
		set
		{
			if (_source.isPlaying)
				_source.time = value;
			else
				_seekTime = value;
		}
	}
	public bool IsPlaying
	{
		get
		{
			return _source.isPlaying;
		}
	}
	
	protected AudioSource _source = null;
	protected float _seekTime = 0.0f;

	void Awake()
	{
		_source = GetComponent<AudioSource>();
		_source.Stop();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (_source.isPlaying)
				_source.Pause();
			else
				_source.Play();
		}
	}

	void OnGUI()
	{
		float width = Screen.width - (screenOffset.x * 2.0f);
		float height = 75.0f;

		float xpos = screenOffset.x;
		float ypos = Screen.height - screenOffset.y - height;

		GUILayout.BeginArea(new Rect(xpos, ypos, width, height), GUI.skin.box);
		GUILayout.BeginVertical();

		// Display title of the box
		GUIStyle centered = new GUIStyle(GUI.skin.label);
		centered.alignment = TextAnchor.UpperCenter;
		GUILayout.Label("Audio Player", centered);

		// Display the buttons and title of the song
		ButtonGUI();

		// Display the current time of playing and the search slider
		SliderGUI();

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	void ButtonGUI()
	{
		// Display the Play-Stop buttons

		GUILayoutOption[] buttonOptions = new GUILayoutOption[1];
		buttonOptions[0] = GUILayout.Width(50);
		GUILayout.BeginHorizontal();

		// Button should change name when music is playing or not
		if (_source.isPlaying)
		{
			if (GUILayout.Button("Pause", buttonOptions))
				_source.Pause();
		}
		else
		{
			if (GUILayout.Button("Play", buttonOptions))
				_source.Play();
		}

		if (GUILayout.Button("Stop", buttonOptions))
		{
			_source.Stop();
			_seekTime = 0.0f;
			_source.time = 0.0f;
		}

		// Song that is playing
		string clipname = string.Empty;
		if (_source.clip != null)
			clipname = _source.clip.name;

		GUILayout.Label("Current Audio Clip: " + clipname);
		GUILayout.EndHorizontal();
	}

	void SliderGUI()
	{

		// Make the seconds and minutes strings to display the time played
		string minutes = "--", seconds = "--", totalMinutes = "--", totalSeconds = "--";
		if (_source.clip != null)
		{
			minutes = GetMinutes(_seekTime).ToString();
			minutes = minutes.Length == 1 ? "0" + minutes : minutes;
			seconds = GetSeconds(_seekTime).ToString();
			seconds = seconds.Length == 1 ? "0" + seconds : seconds;

			totalMinutes = GetMinutes(_source.clip.length).ToString();
			totalMinutes = totalMinutes.Length == 1 ? "0" + totalMinutes : totalMinutes;
			totalSeconds = GetSeconds(_source.clip.length).ToString();
			totalSeconds = totalSeconds.Length == 1 ? "0" + totalSeconds : totalSeconds;
		}

		GUILayout.BeginHorizontal();
		GUILayoutOption[] sliderOptions = new GUILayoutOption[1];
		sliderOptions[0] = GUILayout.ExpandWidth(false);
		GUILayout.Label("Play time: " + minutes + "." + seconds + " | " + totalMinutes + "." + totalSeconds, sliderOptions);

		// Display the search slider
		if (_source.clip != null)
		{
			if (_source.isPlaying)
			{
				GUILayout.HorizontalSlider(_source.time, 0.0f, _source.clip.length);
				_seekTime = _source.time;
			}
			else
			{
				_seekTime = GUILayout.HorizontalSlider(_seekTime, 0.0f, _source.clip.length);
				_source.time = _seekTime;
			}
		}
		else
		{
			GUILayout.HorizontalSlider(0.0f, 0.0f, 0.0f);
		}
		GUILayout.EndHorizontal();

		// Display the time above the hit line
		if (HitLine.use.renderer.enabled)
		{
			Vector3 hltoppos = HitLine.use.transform.position + new Vector3(0.0f, HitLine.use.transform.localScale.y / 2.0f, 0.0f);
			float hlypos = Screen.height - Camera.main.WorldToScreenPoint(hltoppos).y - 20.0f;
			float hlxpos = Camera.main.WorldToScreenPoint(hltoppos).x;

			GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
			centeredStyle.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(hlxpos - 50, hlypos, 100, 25), minutes.ToString() + "." + seconds.ToString(), centeredStyle);
		}
	}

	int GetMinutes(float time)
	{
		return (int)(time / 60.0f);
	}

	int GetSeconds(float time)
	{
		return (int)(time % 60);
	}
}
