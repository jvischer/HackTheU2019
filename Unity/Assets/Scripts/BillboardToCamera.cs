using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardToCamera : MonoBehaviour {

    private void Update() {
        Vector3 objectToCameraDir = (Camera.main.transform.position - transform.position).normalized;
        transform.localRotation = Quaternion.FromToRotation(Vector3.back, objectToCameraDir);
    }

}
