using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CompassController : MonoBehaviour {

    [SerializeField] private Image _compassImage;

    private void Update() {
        if (MapGenerator.player != null) {
            _compassImage.transform.rotation = Quaternion.Euler(0, 0, MapGenerator.player.rotation.eulerAngles.y);
        }
    }

}
