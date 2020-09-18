using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunOrbit : MonoBehaviour {
    public float orbitSpeed;
    public float orbitDistance;
    public GameObject sun;
    public GameObject pointLight;

    // Start is called before the first frame update
    void Start () {
        sun.transform.localPosition = orbitDistance * new Vector3 (0.0f, 1.0f, 0.0f);
        pointLight.transform.localPosition = orbitDistance * new Vector3 (0.0f, 1.0f, 0.0f);

    }

    // Update is called once per frame
    void Update () {
        this.transform.localRotation *= Quaternion.AngleAxis (Time.deltaTime * orbitSpeed, new Vector3 (0.0f, 0.0f, 1.0f));
    }
}