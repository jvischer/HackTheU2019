using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardToCamera : MonoBehaviour {

    private void Update() {
        Vector3 objEulerAngles = transform.rotation.eulerAngles;
        Vector3 camEulerAngles = Camera.main.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(objEulerAngles.x, camEulerAngles.y, objEulerAngles.z);
    }

}
