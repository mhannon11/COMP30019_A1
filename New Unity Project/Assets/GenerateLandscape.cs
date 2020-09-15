using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLandscape : MonoBehaviour
{

    public float roughness = 0.5f; // Roughness constant, 0 = flatest, 1 = steepest
    
    int resolution = 513; //Dimension of Landscape
    int height = 257; // Height of Landscape
    float rnd;

    // Start is called before the first frame update
    void Start()
    {   
        Terrain terrain = GetComponent<Terrain>();
        float[,] map = new float[resolution,resolution];
        Generate(terrain.terrainData, DiamondSquare());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Terrain terrain = GetComponent<Terrain>();
            Generate(terrain.terrainData, DiamondSquare());
        }
    }

    // Prints heights of terrain
    void printTerrain(float[,] map, int size)
    {
        for (int i=0; i<size; i++) {
            for (int j=0; j<size; j++) 
            {
                print(map[i,j]);
            }
        }
    }

    // Generates the height map of the landscape using the diamond square algorithm
    // Some of the following code is based off the diamond square algorithm publically shared by github user eogas
    // https://github.com/eogas/DiamondSquare/blob/master/DiamondSquare/DiamondSquare/DiamondSquare.cs
    float [,] DiamondSquare() {
        float[,] heights = new float[resolution, resolution];
        float s0, s1, s2, s3, d0, d1, d2, d3, c, value;

        //Start by setting all 4 corners with random values
        heights[0,0] = (float)Random.Range(0f, resolution);
        heights[0, resolution - 1] = (float)Random.Range(0f, resolution);
        heights[resolution - 1, 0] = (float)Random.Range(0f, resolution);
        heights[resolution -1, resolution - 1] = (float)Random.Range(0f, resolution);

        //Recurse over grid, halving step each time
        int step;
        for (step = resolution - 1; step > 1; step /= 2) {
            float scale = roughness * step;

            //Diamond
            for (int y = 0; y < resolution-1; y += step)
            {
                for (int x = 0; x < resolution-1; x += step) 
                {
                    d0 = heights[x,y];
                    d1 = heights[x, y + step];
                    d2 = heights[x + step, y];
                    d3 = heights[x + step, y + step];
                    
                    value = (d0 + d1 + d2 + d3) / 4.0f;
                    rnd = (float)(Random.value * scale * 2.0f) - scale;
                    heights[x + (step/2), y + (step/2)] = (float)(value + rnd);
                }
            }

            //Square
            for (int x = 0; x < resolution-1; x += step)
            {
                for (int y = 0; y < resolution-1; y += step) 
                {
                    s0 = heights[x,y];
                    s1 = heights[x, y + step];
                    s2 = heights[x + step, y];
                    s3 = heights[x + step, y + step];
                    c = (heights[x + (step/2), y + (step/2)]);

                    //Check if on the edge of grid
                    if(y <= 0) {
                        d0 = (s0 + s1 + c) / 3.0f;
                    }
                    else
                    {
                        d0 = (s0 + s1 + c + (heights[x + (step/2), y - (step/2)])) / 4.0f;
                    }
                    if(x <= 0) {
                        d1 = (s0 + s2 + c) / 3.0f;
                    }
                    else
                    {
                        d1 = (s0 + s1 + c + (heights[x - (step/2), y + (step/2)])) / 4.0f;
                    }
                    if(y >= (resolution-1) - step) {
                        d3 = (s2 + s3 + c) / 3.0f;
                    }
                    else
                    {
                        d3 = (s2 + s3 + c + (heights[x + (step/2), y + step + (step/2)])) / 4.0f;
                    }
                    if(x >= (resolution-1) - step) {
                        d2 = (s1 + s3 + c) / 3.0f;
                    }
                    else
                    {
                        d2 = (s1 + s3 + c + (heights[x + step + (step/2), y + (step/2)])) / 4.0f;
                    }
                    
                    rnd = (float)(Random.value * scale * 2.0f) - scale;
                    heights[x + (step/2), y] = (float)(d0 + rnd);
                    rnd = (float)(Random.value * scale * 2.0f) - scale;
                    heights[x, y + (step/2)] = (float)(d1 + rnd);
                    rnd = (float)(Random.value * scale * 2.0f) - scale;
                    heights[x + step, y + (step/2)] = (float)(d2 + rnd);
                    rnd = (float)(Random.value * scale * 2.0f) - scale;
                    heights[x + (step/2), y + step] = (float)(d3 + rnd);
                }
            }
            scale /= 2.0f;
        }

        //Normalise mesh
        float normal;
        for (int i=0; i<resolution; i++) {
            for (int j=0; j<resolution; j++) 
            {
                normal = (float)heights[i,j] / resolution;
                if(normal > 1)
                {
                    heights[i,j] = 1;
                }
                else{
                    heights[i,j] = normal;
                }
            }
        }
        // printTerrain(heights, resolution);
        return heights;
    }

    // Generates terrain using a heightmap
    void Generate(TerrainData tData, float [,] heights)
    {
        tData.heightmapResolution = resolution;
        tData.size = new Vector3(resolution, height, resolution);
        tData.SetHeights(0, 0, heights);
    }
}
