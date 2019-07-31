using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour {

    public static MainCameraController instance;

    void Awake() {
        instance = this;
    }

    // Setup camera with default view
    public void Configure(Transform parent)
    {
        transform.parent = parent;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        transform.localPosition = new Vector3(0, 2.185f, -3.70f);
        transform.localRotation = Quaternion.Euler(20, 0, 0);
    }
    
}
