using UnityEngine;
using UnityEditor;

public class TerrainCopyTool : EditorWindow
{
    Terrain sourceTerrain;
    Terrain targetTerrain;

    [MenuItem("Tools/Terrain Copier")]
    public static void ShowWindow()
    {
        GetWindow<TerrainCopyTool>("Terrain Copier");
    }

    void OnGUI()
    {
        GUILayout.Label("Terrain Copier", EditorStyles.boldLabel);
        sourceTerrain = (Terrain)EditorGUILayout.ObjectField("Source Terrain", sourceTerrain, typeof(Terrain), true);
        targetTerrain = (Terrain)EditorGUILayout.ObjectField("Target Terrain", targetTerrain, typeof(Terrain), true);

        if (GUILayout.Button("Copy Terrain Data"))
        {
            if (sourceTerrain != null && targetTerrain != null)
            {
                CopyTerrainData(sourceTerrain, targetTerrain);
            }
            else
            {
                Debug.LogWarning("Please assign both source and target terrain.");
            }
        }
    }

    void CopyTerrainData(Terrain source, Terrain target)
    {
        TerrainData sourceData = source.terrainData;
        TerrainData targetData = target.terrainData;

        // 1. 높이 정보 복사
        float[,] heights = sourceData.GetHeights(0, 0, sourceData.heightmapResolution, sourceData.heightmapResolution);
        targetData.SetHeights(0, 0, heights);

        // 2. 스플랫맵 (베이스맵) 복사
        for (int i = 0; i < sourceData.alphamapLayers; i++)
        {
            float[,,] alphaMaps = sourceData.GetAlphamaps(0, 0, sourceData.alphamapWidth, sourceData.alphamapHeight);
            targetData.SetAlphamaps(0, 0, alphaMaps);
        }

        // 3. 디테일 복사
        for (int i = 0; i < sourceData.detailPrototypes.Length; i++)
        {
            int[,] detailLayer = sourceData.GetDetailLayer(0, 0, sourceData.detailWidth, sourceData.detailHeight, i);
            targetData.SetDetailLayer(0, 0, i, detailLayer);
        }

        // 4. 트리 복사
        targetData.treeInstances = sourceData.treeInstances;
        targetData.treePrototypes = sourceData.treePrototypes;

        Debug.Log("Terrain data copied successfully.");
    }
}