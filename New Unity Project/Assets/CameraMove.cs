using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float speed;
    public float sensitivity;

    Vector2 angle = new Vector2(0,0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        //Control translation
        if (Input.GetKey("w")) {
            this.transform.localPosition += Camera.main.transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey("s")) {
            this.transform.localPosition -= Camera.main.transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey("a")) {
            this.transform.localPosition -= Camera.main.transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey("d")) {
            this.transform.localPosition += Camera.main.transform.right * speed * Time.deltaTime;
        }

        //Control camera rotation
        angle.y += Input.GetAxis("Mouse X");
        angle.x += -Input.GetAxis("Mouse Y");
        transform.eulerAngles = (Vector2)angle * sensitivity;
    }
}
