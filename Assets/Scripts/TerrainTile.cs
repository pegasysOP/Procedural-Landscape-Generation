using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshCollider))]
public class TerrainTile : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;

    float maxHeight;
    AnimationCurve heightCurve;
    TerrainType[] terrainTypes;

    /// <summary>
    /// Initialise the mesh to the provided heightmap
    /// </summary>
    /// <param name="heightMap">Height map to get vertex positions and colours from</param>
    /// <param name="maxHeight">Highest possible location of a vertex</param>
    /// <param name="heightCurve">The height curve</param>
    /// <param name="terrainTypes">List of the types of terrian at each height</param>
    public void Init(float[,] heightMap, float maxHeight, AnimationCurve heightCurve, TerrainType[] terrainTypes)
    {
        this.maxHeight = maxHeight;
        this.heightCurve = heightCurve;
        this.terrainTypes = terrainTypes;

        meshRenderer.material.mainTexture = BuildTexture(heightMap);
        UpdateMeshVertices(heightMap);
    }

    /// <summary>
    /// Creates a 2D texture by using the terrain types specified by the heights in the heightmap 
    /// </summary>
    /// <param name="heightMap">The height map to create the texture from</param>
    /// <returns>2D texture with colours from the terrain types</returns>
    private Texture2D BuildTexture(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        // generate the color map
        Color[] colorMap = new Color[depth * width];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int colorIndex = z * width + x;
                float height = heightMap[z, x];
                colorMap[colorIndex] = GetTerrainTypeAtHeight(height).color;
            }
        }

        // create texture from color map
        Texture2D texture = new Texture2D(width, depth);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Gets the terrain type for the specified height
    /// </summary>
    /// <param name="height">The height to get the terrain type from</param>
    /// <returns>The terrain type for the specified height</returns>
    private TerrainType GetTerrainTypeAtHeight(float height)
    {
        foreach (TerrainType terrainType in terrainTypes)
        {
            if (height < terrainType.height)
            {
                return terrainType;
            }
        }

        return terrainTypes[terrainTypes.Length - 1];
    }

    /// <summary>
    /// Sets the vertical coordinates for the mesh vertices to those speicfied by the height map
    /// </summary>
    /// <param name="heightMap">The hieght map to match the mesh to</param>
    private void UpdateMeshVertices(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        // generate the new vertices
        Vector3[] vertices = meshFilter.mesh.vertices;
        int vertexIndex = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 vertex = vertices[vertexIndex];
                vertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(heightMap[z, x]) * maxHeight, vertex.z);

                vertexIndex++;
            }
        }

        // update the mesh to the new vertices
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateBounds();
        //meshFilter.mesh.RecalculateNormals(); // the meshfilter has no knowledge of adjacent tiles and so seams are visible
        meshFilter.mesh.normals = CalculateNormals(heightMap);

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public int GetMeshSize()
    {
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        return (int)Mathf.Sqrt(meshVertices.Length); // since it's square, the depth and width will be equal
    }

    public static Vector3[] CalculateNormals(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        Vector3[] normals = new Vector3[depth * width];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                int vertexIndex = x * depth + z;

                // edge verteces
                if (x == 0 || x == width - 1 || z == 0 || z == depth - 1)
                {
                    normals[vertexIndex] = Vector3.up;
                    continue;
                }

                float gradientZ = heightMap[z + 1, x] - heightMap[z - 1, x];
                float gradientX = heightMap[z, x + 1] - heightMap[z, x - 1];

                Vector3 normal = new Vector3(-gradientZ, 0.5f, -gradientX);
                normal.Normalize();

                normals[vertexIndex] = normal;
            }
        }

        return normals;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}