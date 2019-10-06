using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour {

    private const float LOCATION_ENABLE_CHECK_REFRESH_COOLDOWN = 1.0F;
    private const float LOCATION_REFRESH_COOLDOWN = 0.5F;

    private LocationInfo _initialLocationInfo;
    private double _latitudeOffset;
    private double _longitudeOffset;

    private IEnumerator Start() {
        _initialLocationInfo = new LocationInfo();

        while (!Input.location.isEnabledByUser) {
            Debug.Log("User's location is currently disabled.");
            yield return new WaitForSeconds(LOCATION_ENABLE_CHECK_REFRESH_COOLDOWN);
        }

        Input.location.Start();

        while (Input.location.status != LocationServiceStatus.Running) {
            yield return null;
        }

        _initialLocationInfo = Input.location.lastData;

        //while (true) {
        //    LocationInfo locationInfo = Input.location.lastData;

        //    Debug.Log(locationInfo.longitude + " " + locationInfo.latitude + " " + locationInfo.altitude + " " + locationInfo.horizontalAccuracy + " " + locationInfo.verticalAccuracy);

        //    yield return new WaitForSeconds(LOCATION_REFRESH_COOLDOWN);
        //}
    }

    public Vector2 getOriginLatLong() {
        return new Vector2(_initialLocationInfo.latitude, _initialLocationInfo.longitude);
    }

    public float getLatitude() {
        if (Input.location.status != LocationServiceStatus.Running) {
            return 0;
        }
        LocationInfo locationInfo = Input.location.lastData;
        return (float)(locationInfo.latitude + _latitudeOffset);
    }

    public float getLongitude() {
        if (Input.location.status != LocationServiceStatus.Running) {
            return 0;
        }
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
