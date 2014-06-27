using UnityEditor;
using UnityEngine;
using System.Collections;

public class CatchingMiceLevelWindow : EditorWindow
{
	protected CatchingMiceLevelDefinition levelDefinition = null;

    bool useBothTypes = false;
    Transform levelParent = null;

    Vector2 TrapTile = Vector2.zero;

    [MenuItem("KikaAndBob/CathingMice/LevelWindow")]
	// Use this for initialization
    static void Init()
    {
        CatchingMiceLevelWindow window = (CatchingMiceLevelWindow)EditorWindow.GetWindow(typeof(CatchingMiceLevelWindow));
        window.Show();
    }
    protected void Update()
    {
        if (levelParent == null)
        {
			GameObject obj = GameObject.Find("LevelParent");
			if (obj != null)
			{
				levelParent = obj.transform;
			}
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("Create new Catching Mice level (root folder)"))
        {
            CatchingMiceLevelDefinition levelDef = ScriptableObject.CreateInstance<CatchingMiceLevelDefinition>();
            AssetDatabase.CreateAsset(levelDef, "Assets/NewCatchingMiceLevel.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelDef;
        }

		levelDefinition = (CatchingMiceLevelDefinition) EditorGUILayout.ObjectField(levelDefinition, typeof(CatchingMiceLevelDefinition), false);

        if (GUILayout.Button("Build Level"))
        {
            if (levelDefinition != null)
			{
				CatchingMiceLevelManager.use.CurrentLevel = levelDefinition;
				CatchingMiceLevelManager.use.BuildLevelEditor();
			}
        }

        if (GUILayout.Button("Snap selection to grid"))
        {
            SnapToGrid(); 
        }

        useBothTypes = EditorGUILayout.Toggle("Use both WaypointTypes", useBothTypes);

        if(GUILayout.Button("Test pathfinding"))
        {
            FindClosestCheese();
        }

        if (GUILayout.Button("Reset Game"))
        {
            CatchingMiceGameManager.use.StartGame();
            
            //SpawnMouseDebug();
        }

        if (GUILayout.Button("Spawn Player"))
        {
            CharacterDebug();
        }

        TrapTile = EditorGUILayout.Vector2Field("Trap Spawn tile", TrapTile);
    }

    //Source: http://wiki.unity3d.com/index.php?title=SnapToGrid
    protected void SnapToGrid()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);

        float gridx = 1.0f;
        float gridy = 1.0f;
        float gridz = 1.0f;

        foreach (Transform transform in transforms)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Round(newPosition.x / gridx) * gridx;
            newPosition.y = Mathf.Round(newPosition.y / gridy) * gridy;
            newPosition.z = Mathf.Round(newPosition.z / gridz) * gridz;
            transform.position = newPosition;
        }
    }

    protected void FindClosestCheese()
    {
        GameObject pathfindingGO = GameObject.Find("PathFindingObject");
        if(pathfindingGO == null)
        {
            pathfindingGO = new GameObject();
            pathfindingGO.name = "PathFindingObject";
            GameObject activeObject = Selection.activeGameObject;
            if(activeObject != null)
                pathfindingGO.transform.position = activeObject.transform.position;
            pathfindingGO.AddComponent<CatchingMicePathFinding>();
        }
        
        CatchingMicePathFinding pathfindScript = pathfindingGO.GetComponent<CatchingMicePathFinding>();
        if (pathfindScript != null)
        {
            pathfindScript.SetupLocal();
            if (useBothTypes)
                pathfindScript.wayType = CatchingMiceTile.TileType.Both;
            else
                pathfindScript.wayType = CatchingMiceTile.TileType.Ground;
               
			CatchingMiceWaypoint target = null;

            float smallestDistance = float.MaxValue;
            //Check which cheese tile is the closest
            foreach (CatchingMiceTile tile in CatchingMiceLevelManager.use.CheeseTiles)
            {
                float distance = Vector2.Distance(pathfindScript.transform.position.v2(), tile.location.v2());
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    target = tile.waypoint;
                }
            }

            if(target!=null)
            {
               pathfindScript.DetectPath(target);
            }
            else
            {
                Debug.LogError("No target found");
            }
        }
    }
    public void SpawnMouseDebug()
    {
        
        CatchingMiceCharacterMouse mousePrefab = null;
        CatchingMiceCharacterMouse mouseController = null;

		foreach (CatchingMiceCharacterMouse prefab in CatchingMiceLevelManager.use.enemyPrefabs)
        {
            mouseController = prefab.GetComponent<CatchingMiceCharacterMouse>();
            if (mouseController != null)
            {
                mousePrefab = prefab;

            }
        }
        GameObject pathfindingGO = GameObject.Find("PathFindingObject");
        if (pathfindingGO == null)
        {
            pathfindingGO = new GameObject();
            pathfindingGO.name = "PathFindingObject";
            GameObject activeObject = Selection.activeGameObject;
            if (activeObject != null)
                pathfindingGO.transform.position = activeObject.transform.position;
            pathfindingGO.AddComponent<CatchingMicePathFinding>();
        }
        if (mousePrefab != null)
        {
            GameObject movePrefab = Instantiate(mousePrefab, pathfindingGO.transform.position, Quaternion.identity) as GameObject;
            if (useBothTypes)
                movePrefab.GetComponent<CatchingMiceCharacterMouse>().walkable = CatchingMiceTile.TileType.Both;
            else
                movePrefab.GetComponent<CatchingMiceCharacterMouse>().walkable = CatchingMiceTile.TileType.Ground;
            movePrefab.GetComponent<CatchingMiceCharacterMouse>().GetTarget();
        }
    }
    public void CharacterDebug()
    {
        GameObject characterPrefab = null;

        foreach (ICatchingMiceCharacter prefab in CatchingMiceLevelManager.use.characterPrefabs)
        {
            if (prefab != null)
            {
                characterPrefab = prefab.gameObject;
                break;
            }
        }
        GameObject pathfindingGO = GameObject.Find("PathFindingObject");
        if (pathfindingGO == null)
        {
            pathfindingGO = new GameObject();
            pathfindingGO.name = "PathFindingObject";
            GameObject activeObject = Selection.activeGameObject;
            if (activeObject != null)
                pathfindingGO.transform.position = activeObject.transform.position;
            pathfindingGO.AddComponent<CatchingMicePathFinding>();
        }
        if (characterPrefab != null)
        {
            Instantiate(characterPrefab, pathfindingGO.transform.position, Quaternion.identity);
        }
    }
}
