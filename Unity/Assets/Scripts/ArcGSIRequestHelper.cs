using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class CandidateData {

    private const string NAME_KEY = "PlaceName";
    private const string ADDR_KEY = "Place_Addr";

    private const string X_KEY = "x";
    private const string Z_KEY = "y";
    private const string X_MIN_KEY = "xmin";
    private const string X_MAX_KEY = "xmax";
    private const string Z_MIN_KEY = "ymin";
    private const string Z_MAX_KEY = "ymax";

    public string address;
    public Dictionary<string, object> location;
    public int score;
    public Dictionary<string, object> attributes;
    public Dictionary<string, object> extent;

    private bool tryGetValue(Dictionary<string, object> dict, string key, out object value) {
        if (dict.ContainsKey(key)) {
            value = dict[key];
            return true;
        }

        value = null;
        return false;
    }

    public string placeName {
        get {
            object nameVal;
            if (tryGetValue(attributes, NAME_KEY, out nameVal)) {
                return nameVal.ToString();
            }

            return "[NO NAME]";
        }
    }

    public string placeAddress {
        get {
            object addressVal;
            if (tryGetValue(attributes, ADDR_KEY, out addressVal)) {
                return addressVal.ToString();
            }

            return "[NO ADDRESS]";
        }
    }

    public float x {
        get {
            object xVal;
            float xFloat;
            if (tryGetValue(location, X_KEY, out xVal) && float.TryParse(xVal.ToString(), out xFloat)) {
                return xFloat;
            }

            return 0;
        }
    }

    public float z {
        get {
            object zVal;
            float zFloat;
            if (tryGetValue(location, Z_KEY, out zVal) && float.TryParse(zVal.ToString(), out zFloat)) {
                return zFloat;
            }

            return 0;
        }
    }

    public float xMin {
        get {
            object xVal;
            float xFloat;
            if (tryGetValue(location, X_MIN_KEY, out xVal) && float.TryParse(xVal.ToString(), out xFloat)) {
                return xFloat;
            }

            return 0;
        }
    }

    public float xMax {
        get {
            object xVal;
            float xFloat;
            if (tryGetValue(location, X_MAX_KEY, out xVal) && float.TryParse(xVal.ToString(), out xFloat)) {
                return xFloat;
            }

            return 0;
        }
    }

    public float zMin {
        get {
            object zVal;
            float zFloat;
            if (tryGetValue(location, Z_MIN_KEY, out zVal) && float.TryParse(zVal.ToString(), out zFloat)) {
                return zFloat;
            }

            return 0;
        }
    }

    public float zMax {
        get {
            object zVal;
            float zFloat;
            if (tryGetValue(location, Z_MAX_KEY, out zVal) && float.TryParse(zVal.ToString(), out zFloat)) {
                return zFloat;
            }

            return 0;
        }
    }

    public Rect rect {
        get {
            float xMinOffset = Mathf.Abs(x - xMin);
            float xMaxOffset = Mathf.Abs(xMax - x);
            float zMinOffset = Mathf.Abs(z - zMin);
            float zMaxOffset = Mathf.Abs(zMax - z);

            float xDelta = xMaxOffset + xMinOffset;
            float zDelta = zMaxOffset + zMinOffset;

            return new Rect(new Vector2((xMin + xMax) / 2, (zMin + zMax) / 2), new Vector2(xDelta, zDelta));
        }
    }

}

public class ArcGSIRequestHelper : MonoBehaviour {

    public void query() {
        Debug.Log("Querying");
        string[] categories = {
            "Gas%20Station",
        };
        string[] outFields = {
            "PlaceName",
            "Place_Addr",
            "City",
            "Region",
            "Location",
        };
        double latitude = -117.919120;
        double longitude = 33.812075;
        int maxResults = 10;

        if (true) {
            latitude = AppConsts.DEFAULT_LATITUDE;
            longitude = AppConsts.DEFAULT_LONGITUDE;
            maxResults = AppConsts.DEFAULT_MAX_RESULTS;
        }

        findAddressCandidates(categories, outFields, latitude, longitude, maxResults);
    }

    public static List<CandidateData> findAddressCandidates(string[] categories, string[] outFields, double latitude, double longitude, int maxResults) {
        StringBuilder categoriesSB = new StringBuilder();
        for (int i = 0; i < categories.Length; i++) {
            if (i > 0) {
                categoriesSB.Append("%20");
            }
            categoriesSB.Append(categories[i]);
        }
        StringBuilder outFieldsSB = new StringBuilder();
        for (int i = 0; i < outFields.Length; i++) {
            if (i > 0) {
                outFieldsSB.Append("%2C");
            }
            outFieldsSB.Append(outFields[i]);
        }

        StringBuilder addressSB = new StringBuilder();
        string findAddressCandidatesUrl = "http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates?";

        string[] parameters = {
            "singleLine=",
            String.Format("category={0}", categoriesSB),
            String.Format("outFields={0}", outFieldsSB),
            String.Format("maxLocations={0}", maxResults),
            String.Format("location={0}%2C{1}", latitude, longitude),
            "forStorage=false",
            "f=pjson"
        };
        addressSB.Append(findAddressCandidatesUrl);
        for (int i = 0; i < parameters.Length; i++) {
            if (i > 0) {
                addressSB.Append("&");
            }
            addressSB.Append(parameters[i]);
        }
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(addressSB.ToString());
        request.Method = "GET";

        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        writeToFile(jsonResponse);
        Debug.Log(jsonResponse);

        Dictionary<string, object> jsonResponseMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
        foreach (string key in jsonResponseMapping.Keys) {
            Debug.Log("Key " + key);
        }
        if (jsonResponseMapping.ContainsKey("candidates")) {
            string candidatesJson = jsonResponseMapping["candidates"].ToString();
            //Debug.Log(featuresJson);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<CandidateData>>(candidatesJson);
        } else {
            Debug.Log(":(");
            return new List<CandidateData>();
        }
    }

    //private void syncRequest() {
    //    string serviceUrl = "https://services3.arcgis.com/GVgbJbqm8hXASVYi/arcgis/rest/services/Trailheads/FeatureServer";
    //    //string serviceUrl = "https://services5.arcgis.com/FgGW6hTiQnpYHl7G/arcgis/rest/services/htu/FeatureServer";
    //    string queryContent = "f=json&where=1=1&outSr=2281&outFields=TRL_NAME,ELEV_FT,CITY_JUR,PARK_NAME,FID";
    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}/0/query?{1}", serviceUrl, queryContent));
    //    request.Method = "POST";
    //    request.ContentLength = queryContent.Length;

    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    StreamReader reader = new StreamReader(response.GetResponseStream());
    //    string jsonResponse = reader.ReadToEnd();
    //    writeToFile(jsonResponse);
    //    Debug.Log(jsonResponse);

    //    Dictionary<string, object> jsonResponseMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
    //    foreach (string key in jsonResponseMapping.Keys) {
    //        Debug.Log("Key " + key);
    //    }
    //    if (jsonResponseMapping.ContainsKey("features")) {
    //        string featuresJson = jsonResponseMapping["features"].ToString();
    //        //Debug.Log(featuresJson);

    //        List<FeatureData> featureData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FeatureData>>(featuresJson);
    //        Debug.Log(featureData.Count + " feature data points");
    //        GameObject featurePrefab = Resources.Load<GameObject>("FeaturePrefab");

    //        for (int i = 0; i < featureData.Count; i++) {
    //            Debug.Log(i + ": " + featureData[i].name + " " + featureData[i].x + ", " + featureData[i].y + " @ " + featureData[i].elevation);

    //            GameObject featureInstance = GameObject.Instantiate(featurePrefab);
    //            featureInstance.name = i + ": " + featureData[i].name;
    //            featureInstance.transform.position = new Vector3(featureData[i].x * 200, featureData[i].elevation / 500, featureData[i].y * 200);
    //        }
    //    } else {
    //        Debug.Log(":(");
    //    }
    //}

    private static void writeToFile(string content) {
        string filePath = Application.streamingAssetsPath + "/rest_api_test.txt";
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }

        StreamWriter streamWriter = File.CreateText(filePath);
        streamWriter.WriteLine(content);
        streamWriter.Close();

        Debug.Log("Wrote file");
    }

    //public class FeatureData {

    //    private const string NAME_KEY = "PARK_NAME";
    //    private const string NAME_2_KEY = "TRL_NAME";
    //    private const string ELEVATION_KEY = "ELEV_FT";
    //    private const string X_KEY = "x";
    //    private const string Y_KEY = "y";

    //    public Dictionary<string, object> attributes;
    //    public Dictionary<string, object> geometry;

    //    public string name {
    //        get {
    //            StringBuilder sb = new StringBuilder();
    //            if (attributes.ContainsKey(NAME_KEY) && attributes[NAME_KEY] != null) {
    //                sb.Append(attributes[NAME_KEY].ToString().Trim());
    //            }
    //            if (attributes.ContainsKey(NAME_2_KEY) && attributes[NAME_2_KEY] != null) {
    //                if (sb.Length > 0) {
    //                    sb.Append(" - ");
    //                }
    //                sb.Append(attributes[NAME_2_KEY].ToString().Trim());
    //            }

    //            return sb.Length > 0 ? sb.ToString() : "[NO_NAME]";
    //        }
    //    }

    //    public int elevation {
    //        get {
    //            int elevationFt;
    //            if (attributes.ContainsKey(ELEVATION_KEY) && Int32.TryParse(attributes[ELEVATION_KEY].ToString(), out elevationFt)) {
    //                return elevationFt;
    //            }

    //            return 0;
    //        }
    //    }

    //    public float x {
    //        get {
    //            float xVal;
    //            if (geometry.ContainsKey(X_KEY) && float.TryParse(geometry[X_KEY].ToString(), out xVal)) {
    //                return xVal;
    //            }

    //            return 0;
    //        }
    //    }

    //    public float y {
    //        get {
    //            float yVal;
    //            if (geometry.ContainsKey(Y_KEY) && float.TryParse(geometry[Y_KEY].ToString(), out yVal)) {
    //                return yVal;
    //            }

    //            return 0;
    //        }
    //    }

    //}

    //private void asyncRequest() {
    //    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(String.Format("https://services3.arcgis.com/GVgbJbqm8hXASVYi/arcgis/rest/services/Trailheads/FeatureServer/0/query"));
    //    request.Method = "POST";

    //    request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
    //}

    //void GetRequestStreamCallback(IAsyncResult callbackResult) {
    //    HttpWebRequest webRequest = (HttpWebRequest) callbackResult.AsyncState;
    //    Stream postStream = webRequest.EndGetRequestStream(callbackResult);

    //    string requestBody = "{\"f\":\"json\",\"where\":\"1=1\",\"outSr\":\"4326\",\"outFields\":\"TRL_NAME,ELEV_FT,CITY_JUR,PARK_NAME,FID\"}";
    //    byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);

    //    postStream.Write(byteArray, 0, byteArray.Length);
    //    postStream.Close();

    //    webRequest.ContentLength = byteArray.Length;
    //    webRequest.BeginGetResponse(new AsyncCallback(GetResponseStreamCallback), webRequest);
    //}

    //void GetResponseStreamCallback(IAsyncResult callbackResult) {
    //    HttpWebRequest request = (HttpWebRequest) callbackResult.AsyncState;
    //    HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(callbackResult);
    //    using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream())) {
    //        string result = httpWebStreamReader.ReadToEnd();
    //        writeToFile(result);
    //        Debug.Log(result);
    //    }
    //}

}
