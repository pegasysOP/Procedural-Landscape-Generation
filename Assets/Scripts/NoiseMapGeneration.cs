using UnityEngine;

public static class NoiseMapGeneration
{
    /// <summary>
    /// Generates a noise map using Perlin Noise from the waves specified
    /// </summary>
    /// <param name="depth">z size of the map</param>
    /// <param name="width">x size of the map</param>
    /// <param name="scale">how zoomed in the noise is</param>
    /// <param name="xOffset">offset on the x axis</param>
    /// <param name="zOffset">offset on the z axis</param>
    /// <param name="waves">the waves to apply the noise from</param>
    /// <returns>2D array containing the noise values</returns>
    public static float[,] GenerateNoiseMap(int depth, int width, float scale, float xOffset, float zOffset, Wave[] waves)
    {
        float[,] map = new float[depth, width];

        float xSample;
        float zSample;

        float noise;
        float normalization;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                xSample = (x + xOffset) / scale;
                zSample = (z + zOffset) / scale;

                noise = 0f;
                normalization = 0f;

                foreach (Wave wave in waves) // apply each wave
                {
                    noise += wave.amplitude * Mathf.PerlinNoise(xSample * wave.frequency + wave.seed, zSample * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }

                map[z, x] = noise / normalization;
            }
        }

        return map;
    }
}

[System.Serializable]
public struct Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}