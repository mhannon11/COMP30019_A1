using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSplatmap : MonoBehaviour
{
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
    }

    public SplatHeights[] splatHeights;

    // Start is called before the first frame update
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        int terrainWidth = terrain.terrainData.alphamapWidth;
        int terrainHeight = terrain.terrainData.alphamapHeight;

        float[, ,] splatmap = new float[terrainWidth, terrainHeight, terrain.terrainData.alphamapLayers];

        // Read heights into array
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                float[] splat = new float[splatHeights.Length];
                float height = terrain.terrainData.GetHeight(y,x);
                
                for (int i=0; i <splatHeights.Length; i++)
                {   
                    if(i == splatHeights.Length-1 && height >= splatHeights[i].startingHeight)
                    {
                        splat[i] = 1;
                    }
                    else if(height >= splatHeights[i].startingHeight && height <= splatHeights[i+1].startingHeight) {
                        splat[i] = 1;
                    }
                }

                for (int j=0; j<splatHeights.Length; j++)
                {
                    splatmap[x, y, j] = splat[j];
                }
            }
        }
        terrain.terrainData.SetAlphamaps(0, 0, splatmap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
