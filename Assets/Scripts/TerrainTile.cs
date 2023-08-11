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
    /// <param name="heightMap">Height map to get vertex positions and colours from (1 pixel wide border used for edge calculations)</param>
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

    // Create the texture for the mesh
    private Texture2D BuildTexture(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        // generate the color map
        Color[] colorMap = new Color[(depth - 2) * (width - 2)];
        for (int z = 1; z < depth - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int colorIndex = (z - 1) * (width - 2) + x - 1;
                float height = heightMap[z, x];
                colorMap[colorIndex] = GetTerrainTypeAtHeight(height).color;
            }
        }

        // create texture from color map
        Texture2D texture = new Texture2D(width - 2, depth - 2);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

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

    // Update the verteces 
    private void UpdateMeshVertices(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        // generate the new vertices
        Vector3[] vertices = meshFilter.mesh.vertices;
        int vertexIndex = 0;
        for (int z = 1; z < depth - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Vector3 vertex = vertices[vertexIndex];
                vertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(heightMap[z, x]) * maxHeight, vertex.z);

                vertexIndex++;
            }
        }

        // update the mesh to the new vertices
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.normals = CalculateNormals(heightMap); // custom normal calculation
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public int GetMeshSize()
    {
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        return (int)Mathf.Sqrt(meshVertices.Length); // since it's square, the depth and width will be equal (for now)
    }

    // Calculate normals for the vertices using the height map
    private Vector3[] CalculateNormals(float[,] heightMap)
    {
        int depth = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        Vector3[] normals = new Vector3[(depth - 2) * (width - 2)];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // ignore borders
                if (x == 0 || x == width - 1 || z == 0 || z == depth - 1)
                    continue;

                int vertexIndex = (z - 1) * (width - 2) + x - 1;

                float gradientZ = heightMap[z + 1, x] - heightMap[z - 1, x];
                float gradientX = heightMap[z, x + 1] - heightMap[z, x - 1];

                Vector3 normal = new Vector3(-gradientZ, 0.2f, -gradientX);
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