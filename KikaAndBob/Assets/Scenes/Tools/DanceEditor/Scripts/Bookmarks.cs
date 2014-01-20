using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bookmark
{

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
				_name = value.Trim();
			else
				_name = "Bookmark";
		}
	}
	public float Time
	{
		get
		{
			return _time;
		}
		set
		{
			if (value < 0f)
				_time = 0f;
			else if (AudioPlayer.use.Source.clip != null && value > AudioPlayer.use.Source.clip.length)
				_time = AudioPlayer.use.Source.clip.length;
			else
				_time = value;
		}
	}

	protected string _name = "";
	protected float _time = 0f;

	public Bookmark(string name, float time)
	{
		Name = name;
		Time = time;
	}
}
public class Bookmarks : LugusSingletonRuntime<Bookmarks>
{
	public Vector2 screenOffset = new Vector2(10, 10);

	public List<Bookmark> bookmarks = new List<Bookmark>();

	protected string _newBookmarkName = "";

	void OnGUI()
	{
		int xpos = Screen.width - 200 - (int)screenOffset.x;
		int ypos = (int)screenOffset.y;

		int width = 200;
		int height = 75 + bookmarks.Count * 25;

		GUILayout.BeginArea(new Rect(xpos, ypos, width, height), GUI.skin.box);
		GUILayout.BeginVertical();

		// Display title of the box
		GUIStyle centered = new GUIStyle(GUI.skin.label);
		centered.alignment = TextAnchor.UpperCenter;
		GUILayout.Label("Bookmarks", centered);

		// Draw the buttons for the bookmarks
		GUILayoutOption[] buttonOptions = new GUILayoutOption[1];
		buttonOptions[0] = GUILayout.Width(95);

		for (int i = 0; i < bookmarks.Count; ++i )
		{
			Bookmark bookmark = bookmarks[i];
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(bookmark.Name, buttonOptions))
				LoadBookmark(bookmark);

			if (GUILayout.Button("Remove", buttonOptions))
			{
				bookmarks.RemoveAt(i);
				--i;
			}

			GUILayout.EndHorizontal();
		}

		// Draw buttons to create a bookmark
		GUILayout.Label("Create a bookmark:");
		GUILayout.BeginHorizontal();
		_newBookmarkName = GUILayout.TextField(_newBookmarkName, 16, buttonOptions);
		if (GUILayout.Button("Create", buttonOptions))
			CreateBookmark(_newBookmarkName, AudioPlayer.use.SeekTime);

		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public string ToXML(int depth)
	{
		string data = string.Empty;

		string bookmarkTabs = string.Empty;
		for (int i = 0; i < depth; ++i)
			bookmarkTabs += "\t";

		foreach(Bookmark bookmark in bookmarks)
		{
			data += bookmarkTabs + "<Bookmark>\r\n";
			data += bookmarkTabs + "\t<Name>" + bookmark.Name + "</Name>\r\n";
			data += bookmarkTabs + "\t<Time>" + bookmark.Time.ToString() + "</Time>\r\n";
			data += bookmarkTabs + "</Bookmark>\r\n";
		}

		return data;
	}

	public void FromXML(TinyXmlReader xml)
	{
		if (xml.tagName != "Bookmarks")
			return;

		while (xml.Read("Bookmarks"))
		{
			if((xml.tagType == TinyXmlReader.TagType.OPENING) && (xml.tagName == "Bookmark"))
			{
				string name = string.Empty;
				float time = 0.0f;

				while(xml.Read("Bookmark"))
				{
					if (xml.tagType != TinyXmlReader.TagType.OPENING)
						continue;

					if (xml.tagName == "Name")
						name = xml.content.Trim();
					else if (xml.tagName == "Time")
						time = float.Parse(xml.content.Trim());
				}

				bookmarks.Add(new Bookmark(name, time));
			}
		}
	}

	protected void LoadBookmark(Bookmark bookmark)
	{
		if (bookmark == null)
			return;

		AudioPlayer.use.SeekTime = bookmark.Time;
	}

	protected void CreateBookmark(string name, float time)
	{
		Bookmark bookmark = new Bookmark(name, time);

		if (bookmarks.Count == 0)
			bookmarks.Add(bookmark);
		else
		{
			for (int i = 0; i < bookmarks.Count; ++i)
			{
				if (bookmarks[i].Time > bookmark.Time)
				{
					bookmarks.Insert(i, bookmark);
					return;
				}
			}
			bookmarks.Add(bookmark);
		}
	}
}
