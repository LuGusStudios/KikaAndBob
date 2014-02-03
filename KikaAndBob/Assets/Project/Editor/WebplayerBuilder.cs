using UnityEditor;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class WebplayerBuilder : MonoBehaviour 
{
	[MenuItem("KikaAndBob/Log scene list")]
	public static void SceneList ()
	{
		//EditorUtility.DisplayDialog("Scene list", "SCENE LIST", "ok");

		DirectoryInfo toplevel = new DirectoryInfo( Application.dataPath + "/Scenes/" );

		string output = "";
		//output += Application.dataPath;

		DirectoryInfo[] categories = toplevel.GetDirectories();
		foreach( DirectoryInfo category in categories )
		{
			DirectoryInfo[] sceneFolders = category.GetDirectories();
			foreach( DirectoryInfo sceneFolder in sceneFolders )
			{
				FileInfo[] scenes = sceneFolder.GetFiles("*.unity");
				foreach(FileInfo scene in scenes )
				{
					string name = scene.FullName.Replace( Application.dataPath, "Assets");

					output += "levels.Add(\"" + name + "\");\n"; 
				}
			}
		}
		
		EditorUtility.DisplayDialog("Scene list", output, "ok");
	}

	[MenuItem("KikaAndBob/Build for web")]
	public static void BuildGame ()
	{
		// Get filename.
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "KikaAndBob");

		// Application.datapath is /Assets in the editor
		File.Copy(Application.dataPath + "/Project/Editor/index.php", path + "/index.php");

		// Build players

		List<string> levels = new List<string>();  

		/* 0.3
		levels.Add( "Assets/Scenes/Builders/Dance/DanceBuilder.unity" );
		levels.Add( "Assets/Scenes/Builders/Pacman/PacmanBuilder.unity" );
		levels.Add( "Assets/Scenes/Builders/Runner/RunnerBuilder.unity" );  
		levels.Add( "Assets/Scenes/Minigames/e05_Mexico/e05_Mexico.unity" ); 
		*/

		// 0.4
		levels.Add("Assets/Scenes/Minigames/e13_pacific/e13_pacific.unity");
		levels.Add("Assets/Scenes/Minigames/e09_Brazil/e09_Brazil.unity");
		levels.Add("Assets/Scenes/Minigames/e08_texas/e08_texas.unity");
		levels.Add("Assets/Scenes/Minigames/e12_newyork/e12_newyork.unity");
		levels.Add("Assets/Scenes/Minigames/e11_vatican/e11_vatican.unity");
		//levels.Add("Assets/Scenes/Minigames/e10_Swiss/e10_Swiss.unity");
		levels.Add("Assets/Scenes/Minigames/e18_amsterdam/e18_amsterdam.unity");
		levels.Add("Assets/Scenes/Minigames/e16_israel/e16_israel.unity");
		levels.Add("Assets/Scenes/Minigames/e20_morocco/e20_morocco.unity");
		levels.Add("Assets/Scenes/Minigames/e15_india/e15_india.unity");
		levels.Add("Assets/Scenes/Builders/DartsBuilder/DartsBuilder.unity");





		foreach( string currentLevel in levels ) 
		{
			string level = currentLevel;
			level = level.Substring(level.LastIndexOf("/") + 1);
			level = level.Replace(".unity", "");

			string[] lvl = new string[1]; 
			lvl[0] = currentLevel;
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