using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DanceHeroLevelOffset : EditorWindow 
{
	protected float offset = 0.0f;
	protected TextAsset textAsset = null;
	protected string returnText = "";

	[MenuItem ("KikaAndBob/DanceHero/DanceHeroLevelOffset")]
	protected static void Init () 
	{
		DanceHeroLevelOffset window = (DanceHeroLevelOffset)EditorWindow.GetWindow (typeof (DanceHeroLevelOffset));
		window.Show();
	}

	protected void OnGUI()
	{
		offset = EditorGUILayout.FloatField("offset", offset);
		textAsset = (TextAsset) EditorGUILayout.ObjectField(textAsset, typeof(TextAsset), false);

		if (GUILayout.Button("Update Text Asset"))
		{
			UpdateFile(textAsset);
		}

		EditorGUILayout.TextArea(returnText);
	}

	protected void UpdateFile(TextAsset asset)
	{
		if (asset == null)
		{
			Debug.LogError("DanceHeroLevelOffset: File is null.");
			return;
		}

		string text = asset.text;
		returnText = "";

		TinyXmlReader parser = new TinyXmlReader(text);


		
		int laneCount = 0;
		while (parser.Read())
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "Lane"))
			{
				returnText += "LANE ------------------------------------------\n\n";
				while (parser.Read("Lane"))
				{
					if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "Item"))
					{
						float time = 0.0f;
						
						// Parse the lane item
						while (parser.Read("Item"))
						{
							if (parser.tagType != TinyXmlReader.TagType.OPENING)
								continue;
							
							if (parser.tagName == "Time")
							{
								time = float.Parse(parser.content.Trim());
								time += offset;
								returnText += (time.ToString() + "\n\n");
							}
						}
					}
				}
			}
		}



	}
}
