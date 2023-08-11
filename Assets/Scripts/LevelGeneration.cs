using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;

    [Header("Level Size")]
    [SerializeField] int mapWidth;
    [SerializeField] int mapDepth;
    [SerializeField] float maxHeight;

    [Header("Terrain Settings")]
    [Range(1f, 10f)]
    [SerializeField] float resolution;
    [SerializeField] AnimationCurve heightCurve;
    [SerializeField] TerrainType[] terrainTypes;
    [SerializeField] Wave[] waves;

    private List<TerrainTile> tiles;

    private void Start()
    {
        GenerateMeshes();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            GenerateTerrain();
        }
    }

    private void GenerateMeshes()
    {
        tiles = new List<TerrainTile>();

        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapDepth; z++)
            {
                Vector3 tilePosition = new Vector3(transform.position.x + (x * tileSize.x), 
                    transform.position.y, 
                    transform.position.z + (z * tileSize.z));

                tiles.Add(Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform).GetComponent<TerrainTile>());
            }
        }
    }

    private void GenerateTerrain()
    {
        // randomise seeds for the waves
        for (int i  = 0; i < waves.Length; i++)
            waves[i].seed = Random.Range(1f, 10000f);

        int meshSize = tiles[0].GetMeshSize();

        foreach (TerrainTile tile in tiles)
        {
            float[,] heightMap = NoiseMapGeneration.GenerateNoiseMap(meshSize, meshSize, resolution, 
                -tile.transform.position.x, -tile.transform.position.z, waves);
            tile.Init(heightMap, maxHeight, heightCurve, terrainTypes);
        }
    }
}