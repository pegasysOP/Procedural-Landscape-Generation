using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] int mapWidth;
    [SerializeField] int mapDepth;

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapDepth; z++)
            {
                Vector3 tilePosition = new Vector3(transform.position.x + (x * tileSize.x), 
                    transform.position.y, 
                    transform.position.z + (z * tileSize.z));

                Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
            }
        }
    }
}