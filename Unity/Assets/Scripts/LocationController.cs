using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour {

    private const float LOCATION_ENABLE_CHECK_REFRESH_COOLDOWN = 1.0F;
    private const float LOCATION_REFRESH_COOLDOWN = 0.5F;

    private static LocationController _instance;

    public LocationInfo initialLocationInfo;
    public bool isInitialized;

    private float _longitudeOffset;
    private float _latitudeOffset;

    private void Awake() {
        _instance = this;
        isInitialized = false;
    }

    private IEnumerator Start() {
        initialLocationInfo = new LocationInfo();

#if !UNITY_EDITOR
        while (!Input.location.isEnabledByUser) {
            Debug.Log("User's location is currently disabled.");
            yield return new WaitForSeconds(LOCATION_ENABLE_CHECK_REFRESH_COOLDOWN);
        }

        Input.location.Start();

        while (Input.location.status != LocationServiceStatus.Running) {
            yield return null;
        }

        initialLocationInfo = Input.location.lastData;
#else
        _longitudeOffset = AppConsts.DEFAULT_LONGITUDE;
        _latitudeOffset = AppConsts.DEFAULT_LATITUDE;
        yield return null;
#endif
        isInitialized = true;

        Debug.Log("Initialized w/ " + " " + initialLocationInfo.longitude + ", " + initialLocationInfo.latitude + " " + _longitudeOffset + " " + _latitudeOffset);

        //while (true) {
        //    LocationInfo locationInfo = Input.location.lastData;

        //    Debug.Log(locationInfo.longitude + " " + locationInfo.latitude + " " + locationInfo.altitude + " " + locationInfo.horizontalAccuracy + " " + locationInfo.verticalAccuracy);

        //    yield return new WaitForSeconds(LOCATION_REFRESH_COOLDOWN);
        //}
    }

    private const float OFFSET_PER_DEV_HOTKEY_SECOND = 1F;

    private void Update() {
        Vector3 devHotkeyMovementDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            devHotkeyMovementDir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            devHotkeyMovementDir += Vector3.back;
        }
        if (Input.GetKey(KeyCode.D)) {
            devHotkeyMovementDir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.A)) {
            devHotkeyMovementDir += Vector3.left;
        }

        Vector3 movementDir = MapGenerator.player.rotation * devHotkeyMovementDir;
        addOffset(OFFSET_PER_DEV_HOTKEY_SECOND * Time.deltaTime * movementDir.x, OFFSET_PER_DEV_HOTKEY_SECOND * Time.deltaTime * movementDir.z);
    }

    private float getInitialLongitude() {
#if UNITY_EDITOR
        return AppConsts.DEFAULT_LONGITUDE;
#endif
        return initialLocationInfo.longitude;
    }

    private float getInitialLatitude() {
#if UNITY_EDITOR
        return AppConsts.DEFAULT_LATITUDE;
#endif
        return initialLocationInfo.latitude;
    }

    public Vector3 getOriginLongLat() {
        return new Vector3(getInitialLongitude(), 0, getInitialLatitude());
    }

    public float getLongitude() {
        if (Input.location.status != LocationServiceStatus.Running) {
            return _longitudeOffset;
        }
        LocationInfo locationInfo = Input.location.lastData;
        return (float)(locationInfo.longitude + _longitudeOffset);
    }

    public float getLatitude() {
        if (Input.location.status != LocationServiceStatus.Running) {
            return _latitudeOffset;
        }
        LocationInfo locationInfo = Input.location.lastData;
        return (float) (locationInfo.latitude + _latitudeOffset);
    }

    public Vector3 getLongLat() {
        return new Vector3(getLongitude(), 0, getLatitude());
    }

    public static Vector3 getPlayerLongLat() {
        return _instance.getLongLat();
    }

    public void addOffset(float x, float z) {
        _longitudeOffset += x;
        _latitudeOffset += z;
    }

    private void OnApplicationQuit() {
        Input.location.Stop();
    }

}
