using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    public float speed;
    public float sensitivity;
    public Vector3 defaultPosition;
    public Vector3 defaultAngle;

    float rotationX = 0.0f;
    float rotationY = 0.0f;

    Rigidbody rigidBody;
    Vector2 angle = new Vector2 (0, 0);

    void Start () {
        resetCamera ();
        rigidBody = GetComponent<Rigidbody> ();
    }

    // Update is called once per frame
    void Update () {
        // Reset camera if new terrain is generated
        if (Input.GetKeyDown (KeyCode.Space)) {
            resetCamera ();
        } else {
            // Control camera rotation
            rotationX += Input.GetAxis ("Mouse X") * sensitivity;
            rotationY += Input.GetAxis ("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp (rotationY, -90f, 90f);

            transform.localRotation = Quaternion.AngleAxis (rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis (rotationY, Vector3.left);

            rigidBody.velocity = Vector3.zero;
            rigidBody.velocity += transform.forward * Input.GetAxis ("Vertical") * Time.deltaTime * speed;
            rigidBody.velocity += transform.right * Input.GetAxis ("Horizontal") * Time.deltaTime * speed;
        }
    }

    // Moves camera to starting posision and resets camera angle
    void resetCamera () {
        this.transform.position = defaultPosition;
        rotationX = defaultAngle.y;
        rotationY = -defaultAngle.x;
    }
}