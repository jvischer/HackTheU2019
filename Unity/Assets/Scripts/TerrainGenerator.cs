using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    private const float LOCATION_ENABLE_CHECK_REFRESH_COOLDOWN = 1.0F;
    private const float LOCATION_REFRESH_COOLDOWN = 0.5F;

    private IEnumerator Start() {
        while (!Input.location.isEnabledByUser) {
            Debug.Log("User's location is currently disabled.");
            yield return new WaitForSeconds(LOCATION_ENABLE_CHECK_REFRESH_COOLDOWN);
        }

        Input.location.Start();

        while (true) {
            LocationInfo locationInfo = Input.location.lastData;

            Debug.Log(locationInfo.longitude + " " + locationInfo.latitude + " " + locationInfo.altitude + " " + locationInfo.horizontalAccuracy + " " + locationInfo.verticalAccuracy);

            yield return new WaitForSeconds(LOCATION_REFRESH_COOLDOWN);
        }
    }

    private void OnApplicationQuit() {
        Input.location.Stop();
    }

}
