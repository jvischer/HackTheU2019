using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenController : MonoBehaviour {

    private Vector3 _prevMousePos;
    private RaycastHit[] _raycastHits;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _prevMousePos = Input.mousePosition;

            if (Camera.main != null) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                _raycastHits = Physics.RaycastAll(ray);
            }
        }
        if (Input.GetMouseButton(0)) {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseDelta = mousePos - _prevMousePos;
            _prevMousePos = mousePos;

            float mouseHorizontalVelocity = mouseDelta.x * Time.deltaTime * AppConsts.SCREEN_HORIZONTAL_SCROLL_SENSITIVITY;
            Vector3 playerEulerAngles = MapGenerator.player.rotation.eulerAngles;
            playerEulerAngles.y += mouseHorizontalVelocity;
            MapGenerator.player.rotation = Quaternion.Euler(playerEulerAngles);
        }
        if (Input.GetMouseButtonUp(0)) {
            _prevMousePos = Input.mousePosition;

            if (Camera.main != null) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] newHits = Physics.RaycastAll(ray);

                // If something was hit on the way down and up
                if (_raycastHits.Length > 0 && newHits.Length > 0) {
                    HashSet<Transform> previousHits = new HashSet<Transform>();
                    for (int i = 0; i < _raycastHits.Length; i++) {
                        previousHits.Add(_raycastHits[i].transform);
                    }
                    for (int i = 0; i < newHits.Length; i++) {
                        if (previousHits.Contains(newHits[i].transform)) {
                            // We have a match! View the node!
                            CandidateController candidateController = newHits[i].transform.parent.GetComponent<CandidateController>();
                            if (candidateController != null) {
                                PopupController.showCandidate(candidateController.getCandidateData());
                                Debug.Log(candidateController.getCandidateData().placeName);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

}
