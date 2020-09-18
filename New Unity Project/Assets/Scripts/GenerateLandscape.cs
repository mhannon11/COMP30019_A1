using System.Collections;
using UnityEngine;

public class GenerateLandscape : MonoBehaviour {
    [System.Serializable]
    public class SplatHeights {
        public int textureIndex;
        public int startingHeight;
    }

    public SplatHeights[] splatHeights;

    public float roughness; // Roughness constant, 0 = flatest, 1 = steepest

    int resolution = 1025; //Dimension of Landscape
    int height = 513; // Height of Landscape
    float rnd;
    float scale;

    // Start is called before the first frame update
    void Start () {
        Terrain terrain = GetComponent<Terrain> ();
        Generate (terrain.terrainData, DiamondSquare ());
        paintTerrain (terrain.terrainData);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            Terrain terrain = GetComponent<Terrain> ();
            Generate (terrain.terrainData, DiamondSquare ());
            paintTerrain (terrain.terrainData);
        }
    }

    // Generates the height map of the landscape using the diamond square algorithm
    // Some of the following code is based off the diamond square algorithm publically shared by github user eogas
    // https://github.com/eogas/DiamondSquare/blob/master/DiamondSquare/DiamondSquare/DiamondSquare.cs
    float[, ] DiamondSquare () {
        float[, ] heights = new float[resolution, resolution];
        float s0, s1, s2, s3, d0, d1, d2, d3, c, value;

        //Start by setting all 4 corners with random values
        float[] corners = new float[4];
        int low = 0;
        int high = 0;

        heights[0, 0] = (float) Random.Range (0f, height);
        heights[0, resolution - 1] = (float) Random.Range (0f, height);
        heights[resolution - 1, 0] = (float) Random.Range (0f, height);
        heights[resolution - 1, resolution - 1] = (float) Random.Range (0f, height);

        //Recurse over grid, halving step each time
        int step;
        for (step = resolution - 1; step > 1; step /= 2) {
            float scale = roughness * step;

            //Diamond
            for (int y = 0; y < resolution - 1; y += step) {
                for (int x = 0; x < resolution - 1; x += step) {
                    d0 = heights[x, y];
                    d1 = heights[x, y + step];
                    d2 = heights[x + step, y];
                    d3 = heights[x + step, y + step];

                    value = (d0 + d1 + d2 + d3) / 4.0f;
                    rnd = generateValid (value, scale);
                    heights[x + (step / 2), y + (step / 2)] = (float) (value + rnd);
                }
            }

            //Square
            for (int x = 0; x < resolution - 1; x += step) {
                for (int y = 0; y < resolution - 1; y += step) {
                    s0 = heights[x, y];
                    s1 = heights[x, y + step];
                    s2 = heights[x + step, y];
                    s3 = heights[x + step, y + step];
                    c = (heights[x + (step / 2), y + (step / 2)]);

                    //Check if on the edge of grid
                    if (y <= 0) {
                        d0 = (s0 + s1 + c) / 3.0f;
                    } else {
                        d0 = (s0 + s1 + c + (heights[x + (step / 2), y - (step / 2)])) / 4.0f;
                    }
                    if (x <= 0) {
                        d1 = (s0 + s2 + c) / 3.0f;
                    } else {
                        d1 = (s0 + s1 + c + (heights[x - (step / 2), y + (step / 2)])) / 4.0f;
                    }
                    if (y >= (resolution - 1) - step) {
                        d3 = (s2 + s3 + c) / 3.0f;
                    } else {
                        d3 = (s2 + s3 + c + (heights[x + (step / 2), y + step + (step / 2)])) / 4.0f;
                    }
                    if (x >= (resolution - 1) - step) {
                        d2 = (s1 + s3 + c) / 3.0f;
                    } else {
                        d2 = (s1 + s3 + c + (heights[x + step + (step / 2), y + (step / 2)])) / 4.0f;
                    }

                    rnd = generateValid (d0, scale);
                    heights[x + (step / 2), y] = (float) (d0 + rnd);
                    rnd = generateValid (d1, scale);
                    heights[x, y + (step / 2)] = (float) (d1 + rnd);
                    rnd = generateValid (d2, scale);
                    heights[x + step, y + (step / 2)] = (float) (d2 + rnd);
                    rnd = generateValid (d3, scale);
                    heights[x + (step / 2), y + step] = (float) (d3 + rnd);
                }
            }
            scale /= 2.0f;
        }

        //Normalise mesh
        float normal;
        for (int i = 0; i < resolution; i++) {
            for (int j = 0; j < resolution; j++) {
                normal = (float) heights[i, j] / resolution;
                if (normal > 1) {
                    heights[i, j] = 1;
                } else if (normal < 0) {
                    heights[i, j] = 0;
                } else {
                    heights[i, j] = normal;
                }
            }
        }
        return heights;
    }

    // Generates a random number that does not result in a value higher than the max height
    float generateValid (float value, float scale) {
        int valid = 0;
        float rnd = 0.0f;

        while (valid == 0) {
            rnd = (float) (Random.value * scale * 2.0f) - scale;
            if (rnd + value <= height) {
                valid = 1;
            }
        }

        return rnd;
    }

    // Paints terrain with textures
    // Some of the following code is based off a similar implementation by youtube Holistic3d
    // https://www.youtube.com/watch?v=aUcWm1k0xDc&ab_channel=Holistic3d

    void paintTerrain (TerrainData tData) {
        int terrainWidth = tData.alphamapWidth;
        int terrainHeight = tData.alphamapHeight;

        float[, , ] splatmap = new float[terrainWidth, terrainHeight, tData.alphamapLayers];

        // Read heights into array
        for (int x = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainHeight; y++) {
                float[] splat = new float[splatHeights.Length];
                float height = tData.GetHeight (y, x);

                for (int i = 0; i < splatHeights.Length; i++) {
                    if (i == splatHeights.Length - 1 && height >= splatHeights[i].startingHeight) {
                        splat[i] = 1;
                    } else if (height >= splatHeights[i].startingHeight && height <= splatHeights[i + 1].startingHeight) {
                        splat[i] = 1;
                    }
                }

                for (int j = 0; j < splatHeights.Length; j++) {
                    splatmap[x, y, j] = splat[j];
                }
            }
        }
        tData.SetAlphamaps (0, 0, splatmap);
    }

    // Generates terrain using a heightmap
    void Generate (TerrainData tData, float[, ] heights) {
        tData.heightmapResolution = resolution;
        tData.size = new Vector3 (resolution, height, resolution);
        tData.SetHeights (0, 0, heights);
    }
}