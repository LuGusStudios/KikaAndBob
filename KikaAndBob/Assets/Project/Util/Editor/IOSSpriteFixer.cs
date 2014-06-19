using UnityEngine;
using System.Collections;
using UnityEditor;

public class IOSSpriteFixer : EditorWindow 
{
	
	void Start () {
	
	}

	void Update () {
	
	}

	[MenuItem("KikaAndBob/Sprite fixer")]
	static void Initialize()
	{
		IOSSpriteFixer fixer = GetWindow<IOSSpriteFixer>();
		fixer.Show();
	}


	void OnGUI()
	{
		if (GUILayout.Button("Check sprites"))
		{

#if !UNITY_IOS
			Debug.Log("This is only relevant on iOS build mode!");
				return;  
#endif

			Object[] textures = GetSelectedTextures();

			foreach (Texture2D texture in textures)  
			{
				string path = AssetDatabase.GetAssetPath(texture);
				//Debug.Log("path: " + path);
				TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;



				if (textureImporter.textureType == TextureImporterType.Sprite && (texture.format == TextureFormat.PVRTC_RGBA4 || texture.format == TextureFormat.PVRTC_RGB4))
				{
					// only find POT textures
					if (textureImporter.npotScale == TextureImporterNPOTScale.None && textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor)
					{
						TextureImporterFormat savedFormat = textureImporter.textureFormat;

						textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;

						AssetDatabase.ImportAsset(path);

						// only keep truecolor if not square
						if (texture.width == texture.height)
						{
							textureImporter.textureFormat = savedFormat;
						}

						AssetDatabase.ImportAsset(path);
					
					}
				}
			}
		}
	}

	protected void SelectedChangeTextureFormatSettings(TextureImporterFormat newFormat) {
		

		Selection.objects = new Object[0];

	}


	
	protected Object[] GetSelectedTextures()
	{
		return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
	}
}
