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

	[MenuItem("KikaAndBob/Test Asset Moves")]
	public static void AssetMoveTest ()
	{
		MoveSceneSpecificAssets( "/Resources", "/ResourcesTemp", "e03_china" );


		/*
		DirectoryInfo toplevel = new DirectoryInfo( Application.dataPath + "/Resources/" );

		if( !Directory.Exists( Application.dataPath + tempFolder) )
		{
			Directory.CreateDirectory(Application.dataPath + tempFolder);
		}


		AssetMoveRecursive( toplevel, tempFolder, "e03_china" );
		*/

		/*
		DirectoryInfo toplevel = new DirectoryInfo( Application.dataPath + "/" + tempFolder );
		
		if( !Directory.Exists( Application.dataPath + "/Resources") )
		{
			Directory.CreateDirectory(Application.dataPath + "/Resources");
		}
		
		
		AssetMoveRecursive( toplevel, "/Resources", "e03_china" );
		*/
		
		EditorUtility.DisplayDialog("Moving assets", "Done", "ok"); 
	}
	
	
	[MenuItem("KikaAndBob/Test Asset Moves Back")]
	public static void AssetMoveTestBack () 
	{
		MoveSceneSpecificAssets( "/ResourcesTemp", "/Resources", "e03_china" );
		
		EditorUtility.DisplayDialog("Moving assets back", "Done", "ok"); 
	}

	protected static void MoveSceneSpecificAssets( string sourceFolder, string targetFolder, string excludePrefix )
	{
		List<string> l = new List<string>();
		l.Add( excludePrefix );

		MoveSceneSpecificAssets( sourceFolder, targetFolder, l );
	}

	protected static void MoveSceneSpecificAssets( string sourceFolder, string targetFolder, List<string> excludePrefixes )
	{
		string excluded = "";
		foreach( string prefix in excludePrefixes )
			excluded += "" + prefix + ", ";

		UnityEngine.Debug.Log("MoveSceneSpecificAssets: " + sourceFolder + " to " + targetFolder + " without " + excluded);

		depth = 0;
		
		DirectoryInfo source = new DirectoryInfo( Application.dataPath + sourceFolder );
		
		if( source == null )
		{
			UnityEngine.Debug.LogError("Source folder does not exist! " + (Application.dataPath + sourceFolder) );
			return;
		}
		
		if( !Directory.Exists( Application.dataPath + targetFolder ) )
		{
			Directory.CreateDirectory(Application.dataPath + targetFolder);
		}
		
		
		AssetMoveRecursive( source, targetFolder, excludePrefixes );
	}

	public static int depth = 0;
	protected static void AssetMoveRecursive(DirectoryInfo parent, string basePath, List<string> excludePrefixes)
	{
		if( depth > 20 ) 
			return;

		// recursive in side directories
		DirectoryInfo[] dirs = parent.GetDirectories();
		foreach( DirectoryInfo dir in dirs )
		{
			if( !Directory.Exists( Application.dataPath + basePath + "/" + dir.Name ) )
			{
				Directory.CreateDirectory( Application.dataPath + basePath + "/" + dir.Name);
			}

			++depth;

			UnityEngine.Debug.Log("MoveAssets: Recursiving " + (basePath + "/" + dir.Name) ); 
			AssetMoveRecursive( dir, basePath + "/" + dir.Name, excludePrefixes );

			/*
			DirectoryInfo[] sceneFolders = category.GetDirectories();
			foreach( DirectoryInfo sceneFolder in sceneFolders )
			{
				FileInfo[] scenes = sceneFolder.GetFiles("*.unity");
				foreach(FileInfo scene in scenes )
				{
					string name = scene.FullName.Replace( Application.dataPath, "Assets");
					
					//output += "levels.Add(\"" + name + "\");\n"; 
				}
			}
			*/
		}

		FileInfo[] files = parent.GetFiles();
		foreach( FileInfo file in files )
		{
			// for MOveAssets, we need the path relative to the PROJECT folder of unity...
			// we don't have that readily available, so force it out of file.FullName
			// App.DataPath is everything up and including Assets, so we need to prepend assets ourselves
			string oldPath = "Assets" + (file.FullName.Replace(Application.dataPath, ""));

			string newPath = "Assets" + basePath + "/" + file.Name;

			if( file.Name.Contains(".meta") )
			{
				continue;
			}

			bool prefixFound = false;
			foreach( string prefix in excludePrefixes )
			{
				//UnityEngine.Debug.LogWarning("Checking file " + file.Name +" to prefix " + prefix + " -> " + file.Name.Contains(prefix) );
				if( file.Name.Contains(prefix) )
				{
					prefixFound = true;
					break;
				}
			}

			if( prefixFound )
				continue;

			// if we get here, the file doesn't contain prefix
			// still, we can't necessarily just move it. This would mean missing all normal SFX and buttons etc.
			// we ONLY want to move stuff that is scene specific, i.e. exx_
			// so check if we have that kind of file
			if( !file.Name.StartsWith("e") )
				continue;

			if( file.Name.IndexOf("_") != 3 )
				continue;

			string numbers = file.Name.Substring(1, 2);
			int epnumber = 0;

			try
			{
				epnumber = int.Parse(numbers);
			}
			catch( System.Exception e ) 
			{
				// if this was no number, it's no episode-file, so we skip it;
				UnityEngine.Debug.LogWarning("File skipped : " + file.Name);
				continue;
			}


			// DEBUG : remove me
			//if( !file.FullName.Contains("Audio") )
			//{
			//	continue;
			//}
			                       
			
			// DEBUG : remove me
			//if( file.Name.Contains("e03_china_1.") )
			//{
				UnityEngine.Debug.LogError("Moving file " + oldPath + " TO " + newPath );
				string result = AssetDatabase.MoveAsset( oldPath, newPath );

				if( !string.IsNullOrEmpty(result) )
				{
					UnityEngine.Debug.LogError("Error moving " + oldPath + " TO " + newPath + " -> " + result);
				}
			//}

			//MoveAsset( "/Assets" + basePath );
		}
	}

	[MenuItem("KikaAndBob/Build for web")]
	public static void BuildGame ()
	{

		// Build players

		List<string> levels = new List<string>();  

		/* 0.3
		levels.Add( "Assets/Scenes/Builders/Dance/DanceBuilder.unity" );
		levels.Add( "Assets/Scenes/Builders/Pacman/PacmanBuilder.unity" );
		levels.Add( "Assets/Scenes/Builders/Runner/RunnerBuilder.unity" ); 
		levels.Add( "Assets/Scenes/Minigames/e05_Mexico/e05_Mexico.unity" );  
		*/


		/*
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
		*/

		// 0.5
		//levels.Add("Assets/Scenes/Minigames/e01_kenia/e01_kenia.unity");
		//levels.Add("Assets/Scenes/Minigames/e02_argentina/e02_argentina.unity");
		//levels.Add("Assets/Scenes/Minigames/e03_china/e03_china.unity");
		//levels.Add("Assets/Scenes/Minigames/e04_tasmania/e04_tasmania.unity");
		//levels.Add("Assets/Scenes/Minigames/e05_Mexico/e05_Mexico.unity");
		//levels.Add("Assets/Scenes/Minigames/e06_egypt/e06_egypt.unity");

		// 0.5.1
		//levels.Add("Assets/Scenes/Minigames/e09_Brazil/e09_Brazil.unity");
		
		// 0.5.2
		//levels.Add("Assets/Scenes/Minigames/e03_china/e03_china.unity"); 

		// 1.0
		//levels.Add("Assets/Scenes/Minigames/e01_kenia/e01_kenia.unity"); 
		//levels.Add("Assets/Scenes/Minigames/e02_argentina/e02_argentina.unity");
		//levels.Add("Assets/Scenes/Minigames/e03_china/e03_china.unity");
		
		// 0.5.3
		//levels.Add("Assets/Scenes/Minigames/e09_Brazil/e09_Brazil.unity"); 

		// 0.6
		//levels.Add("Assets/Scenes/Minigames/e13_pacific/e13_pacific.unity");
		//levels.Add("Assets/Scenes/Minigames/e09_Brazil/e09_Brazil.unity");
		//levels.Add("Assets/Scenes/Minigames/e08_texas/e08_texas.unity");

		// 1.1 
		//levels.Add("Assets/Scenes/Minigames/e04_tasmania/e04_tasmania.unity");
		//levels.Add("Assets/Scenes/Minigames/e05_Mexico/e05_Mexico.unity");
		//levels.Add("Assets/Scenes/Minigames/e06_egypt/e06_egypt.unity");

		// 0.7
		//levels.Add("Assets/Scenes/Minigames/e12_newyork/e12_newyork.unity");
		//levels.Add("Assets/Scenes/Minigames/e11_vatican/e11_vatican.unity");
		//levels.Add("Assets/Scenes/Minigames/e10_Swiss/e10_Swiss.unity"); 

		// 1.2
		//levels.Add("Assets/Scenes/Minigames/e13_pacific/e13_pacific.unity");
		//levels.Add("Assets/Scenes/Minigames/e09_Brazil/e09_Brazil.unity");
		//levels.Add("Assets/Scenes/Minigames/e08_texas/e08_texas.unity");

		// 0.8
		//levels.Add("Assets/Scenes/Minigames/e18_amsterdam/e18_amsterdam.unity");
		//levels.Add("Assets/Scenes/Minigames/e16_israel/e16_israel.unity");
		//levels.Add("Assets/Scenes/Minigames/e20_morocco/e20_morocco.unity");

		// 1.3
		//levels.Add("Assets/Scenes/Minigames/e12_newyork/e12_newyork.unity");
		//levels.Add("Assets/Scenes/Minigames/e11_vatican/e11_vatican.unity");
		//levels.Add("Assets/Scenes/Minigames/e10_Swiss/e10_Swiss.unity"); 

		// 0.9
		//levels.Add("Assets/Scenes/Minigames/e15_india/e15_india.unity");
		//levels.Add("Assets/Scenes/Minigames/e14_buthan/e14_buthan.unity");
		//levels.Add("Assets/Scenes/Minigames/e17_greenland/e17_greenland.unity");

		// 1.3
		levels.Add("Assets/Scenes/Minigames/e18_amsterdam/e18_amsterdam.unity");
		levels.Add("Assets/Scenes/Minigames/e16_israel/e16_israel.unity");
		levels.Add("Assets/Scenes/Minigames/e20_morocco/e20_morocco.unity");

		/*
		levels.Add("Assets/Scenes/Minigames/e14_buthan/e14_buthan.unity");
		levels.Add("Assets/Scenes/Minigames/e15_india/e15_india.unity");
		levels.Add("Assets/Scenes/Minigames/e16_israel/e16_israel.unity");
		levels.Add("Assets/Scenes/Minigames/e17_greenland/e17_greenland.unity");
		levels.Add("Assets/Scenes/Minigames/e18_amsterdam/e18_amsterdam.unity");
		levels.Add("Assets/Scenes/Minigames/e19_illinois/e19_illinois.unity");
		levels.Add("Assets/Scenes/Minigames/e20_morocco/e20_morocco.unity");
		levels.Add("Assets/Scenes/Minigames/e21_cuba/e21_cuba.unity");
		levels.Add("Assets/Scenes/Minigames/e22_russia/e22_russia.unity");
		levels.Add("Assets/Scenes/Minigames/e23_england/e23_england.unity");
		levels.Add("Assets/Scenes/Minigames/e24_japan/e24_japan.unity");
		levels.Add("Assets/Scenes/Minigames/e25_sicily/e25_sicily.unity");
		levels.Add("Assets/Scenes/Minigames/e26_belgium/e26_belgium.unity");
		*/

		string levelListOutput = "";
		foreach( string levelName in levels ) 
		{
			levelListOutput += "" + levelName + "\n";
		}
		
		EditorUtility.DisplayDialog("Scene list", levelListOutput, "ok");
		
		// Get filename.
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "KikaAndBob");
		
		// Application.datapath is /Assets in the editor
		File.Copy(Application.dataPath + "/Project/Editor/index.php", path + "/index.php");

		Directory.CreateDirectory( path + "/images" );
		File.Copy(Application.dataPath + "/Project/Editor/images/LoadBar01.png", path + "/images/LoadBar01.png");
		File.Copy(Application.dataPath + "/Project/Editor/images/LogoKB02.png",  path + "/images/LogoKB02.png");


		foreach( string currentLevel in levels ) 
		{
			string level = currentLevel;
			level = level.Substring(level.LastIndexOf("/") + 1);
			level = level.Replace(".unity", "");


			MoveSceneSpecificAssets( "/Resources", "/ResourcesTemp", level );

			string[] lvl = new string[1]; 
			lvl[0] = currentLevel;
			BuildPipeline.BuildPlayer(lvl, path + "/" + level, BuildTarget.WebPlayer, BuildOptions.None);
			
			MoveSceneSpecificAssets( "/ResourcesTemp", "/Resources", level );
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