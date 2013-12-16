using UnityEditor;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class IOSBuilder : MonoBehaviour 
{
	[MenuItem("KikaAndBob/Build for ios")]
	public static void BuildGame ()
	{
		// Get filename.
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "KikaAndBob"); 

		// Application.datapath is /Assets in the editor
		//File.Copy(Application.dataPath + "/Project/Editor/index.php", path + "/index.php");

		// Build players

		List<string> levels = new List<string>(); 
		levels.Add( "Assets/Scenes/Builders/Dance/DanceBuilder.unity" );
		levels.Add( "Assets/Scenes/Minigames/e02_argentina/e02_argentina.unity" ); 
		levels.Add( "Assets/Scenes/Builders/Frogger/FroggerBuilder.unity" );
		levels.Add( "Assets/Scenes/Builders/Pacman/PacmanBuilder.unity" );
		levels.Add( "Assets/Scenes/Builders/Runner/RunnerBuilder.unity" );  
		levels.Add( "Assets/Scenes/Builders/DartsBuilder/DartsBuilder.unity" ); 

		//foreach( string currentLevel in levels ) 
		//{
			/*
			string level = currentLevel;
			level = level.Substring(level.LastIndexOf("/") + 1);
			level = level.Replace(".unity", "");

			string[] lvl = new string[1];
			lvl[0] = currentLevel;
			*/

			BuildPipeline.BuildPlayer(levels.ToArray(), path + "/", BuildTarget.iPhone, BuildOptions.None);
		//}



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