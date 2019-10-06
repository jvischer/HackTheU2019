using UnityEngine;
using System.Collections;

public class ScreenController : MonoBehaviour {

    private Vector3 _prevMousePos;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _prevMousePos = Input.mousePosition;
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
    }

}
