using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{   

    public Texture texture;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.SetTexture("_texture", texture);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.SetTexture("_texture", texture);
    }
}
