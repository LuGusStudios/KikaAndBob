using UnityEditor;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;

public class WebplayerBuilder : MonoBehaviour 
{
	[MenuItem("KikaAndBob/Build for web")]
	public static void BuildGame ()
	{
		// Get filename.
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "KikaAndBob");
		
		// Build player.

		List<string> levels = new List<string>();
		levels.Add( "Assets/Scenes/Builders/Dance/DanceBuilder.unity" );
		levels.Add( "Assets/Scenes/Minigames/e02_argentina/e02_argentina.unity" ); 
		//levels.Add( "Assets/Scenes/Builders/Frogger/FroggerBuilder.unity" );
		//levels.Add( "Assets/Scenes/Builders/Pacman/PacmanBuilder.unity" );
		//levels.Add( "Assets/Scenes/Builders/Runner/RunnerBuilder.unity" ); 

		foreach( string level in levels )
		{
			string[] lvl = new string[1];
			lvl[0] = level;
			BuildPipeline.BuildPlayer(lvl, path + "/" + level, BuildTarget.WebPlayer, BuildOptions.None);
		}

		/*
		// Copy a file from the project folder to the build folder, alongside the built game.
		FileUtil.CopyFileOrDirectory("Assets/WebPlayerTemplates/Readme.txt", path + "Readme.txt");
		
		// Run the game (Process class from System.Diagnostics).
		Process proc = new Process();
		proc.StartInfo.FileName = path + "BuiltGame.exe";
		proc.Start();
		*/
	}
}