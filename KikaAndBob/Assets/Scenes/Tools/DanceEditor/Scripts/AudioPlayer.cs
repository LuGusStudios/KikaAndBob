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
		GUI.Box(new Rect(screenOffset.x, ypos, width, height), "Audio Player");

		// Buttons
		ypos += 25;
		if (_source.isPlaying)
		{
			if (GUI.Button(new Rect(xpos + 10, ypos, 80, 20), "Pause"))
				_source.Pause();
		}
		else
		{
			if (GUI.Button(new Rect(xpos + 10, ypos, 80, 20), "Play"))
				_source.Play();
		}

		if (GUI.Button(new Rect(xpos + 110, ypos, 80, 20), "Stop"))
		{
			_source.Stop();
			_seekTime = 0.0f;
			_source.time = 0.0f;
		}

		// Song that is playing
		xpos += 200;
		string clipname = string.Empty;
		if (_source.clip != null)
			clipname = _source.clip.name;

		GUI.Label(new Rect(xpos + 10, ypos, width - 200, 20), "Current Audio Clip: " + clipname);

		// Display play time
		ypos += 25;
		xpos = screenOffset.x;
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

		GUI.Label(new Rect(screenOffset.x + 10, ypos, 180, 20), "Play time: " + minutes + "." + seconds + " | " + totalMinutes + "." + totalSeconds);

		// Display search slider
		xpos += 200;
		if (_source.clip != null)
		{
			if (_source.isPlaying)
			{
				GUI.HorizontalSlider(new Rect(xpos + 10, ypos, width - 220, 20), _source.time, 0.0f, _source.clip.length);
				_seekTime = _source.time;
			}
			else
			{
				_seekTime = GUI.HorizontalSlider(new Rect(xpos + 10, ypos, width - 220, 20), _seekTime, 0.0f, _source.clip.length);
				_source.time = _seekTime;
			}
		}
		else
		{
			GUI.HorizontalSlider(new Rect(xpos + 10, ypos, width - 220, 20), 0.0f, 0.0f, 0.0f);
		}

		// Display the playtime above the hit line
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
