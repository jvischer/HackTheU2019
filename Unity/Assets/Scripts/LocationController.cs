using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour {

    private const float LOCATION_ENABLE_CHECK_REFRESH_COOLDOWN = 1.0F;
    private const float LOCATION_REFRESH_COOLDOWN = 0.5F;

    private double _latitudeOffset;
    private double _longitudeOffset;

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

    public float getLatitude() {
        LocationInfo locationInfo = Input.location.lastData;
        return (float)(locationInfo.latitude + _latitudeOffset);
    }

    public float getLongitude() {
        LocationInfo locationInfo = Input.location.lastData;
        return (float)(locationInfo.longitude + _longitudeOffset);
    }

    public Vector2 getLatLong() {
        return new Vector2(getLatitude(), getLongitude());
    }

    public void addOffset(double x, double z) {
        _latitudeOffset += x;
        _longitudeOffset += z;
    }

    private void OnApplicationQuit() {
        Input.location.Stop();
    }

}
