using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] CandidateController _candidatePrefab;

    [SerializeField] LocationController _locationController;

    private enum CategoryType {
        ArtsAndEntertainment,
        Education,
        Food,
        ShopsAndService,
        LandFeatures,
        Nightlife,
        ParksAndOutdoors,
        ProfessionalAndOtherPlaces,
    }

    private class CategoryData {
        public string[] Filters;
        public Color Color;
    }

    private static readonly Dictionary<CategoryType, CategoryData> CATEGORY_TYPES_TO_SEARCH_CATEGORIES = new Dictionary<CategoryType, CategoryData>() {
        { CategoryType.ArtsAndEntertainment, new CategoryData {
            Filters = new string[] { "Arts and Entertainment" },
            Color = new Color32(0x6C, 0x72, 0xDB, 0xFF),
        } },
        { CategoryType.Education, new CategoryData {
            Filters = new string[] { "Education" },
            Color = new Color32(0x12, 0x38, 0x52, 0xFF),
        } },
        { CategoryType.Food, new CategoryData {
            Filters = new string[] { "Food" },
            Color = new Color32(0x93, 0x2C, 0x2F, 0xFF),
        } },
        { CategoryType.ShopsAndService, new CategoryData {
            Filters = new string[] { "Shops and Service" },
            Color = new Color32(0x19, 0x1C, 0x59, 0xFF),
        } },
        { CategoryType.LandFeatures, new CategoryData {
            Filters = new string[] { "Land Features" },
            Color = new Color32(0x4A, 0x94, 0x70, 0xFF),
        } },
        { CategoryType.Nightlife, new CategoryData {
            Filters = new string[] { "Nightlife Spot" },
            Color = new Color32(0x03, 0x25, 0x3A, 0xFF),
        } },
        { CategoryType.ParksAndOutdoors, new CategoryData {
            Filters = new string[] { "Parks and Outdoors" },
            Color = new Color32(0x0F, 0x59, 0x35, 0xFF),
        } },
        { CategoryType.ProfessionalAndOtherPlaces, new CategoryData {
            Filters = new string[] { "Professional and Other Places" },
            Color = new Color32(0x61, 0x9A, 0xBF, 0xFF),
        } },
    };

    public static Transform player;

    private List<CandidateController> _activeCandidates;

    private void Awake() {
        player = GameObject.Instantiate(_playerPrefab).transform;
        _activeCandidates = new List<CandidateController>();

        Screen.sleepTimeout = -1;
    }

    private IEnumerator Start() {
        while (!_locationController.isInitialized) {
            yield return null;
        }

        CategoryType[] categoryTypes = {
            CategoryType.Food,
            CategoryType.ShopsAndService,
            CategoryType.ArtsAndEntertainment,
            CategoryType.Education,
            CategoryType.LandFeatures,
            CategoryType.ParksAndOutdoors,
            CategoryType.ProfessionalAndOtherPlaces,
            CategoryType.Nightlife,
        };
        for (int i = 0; i < categoryTypes.Length; i++) {
            List<string> categories = new List<string>();
            CategoryData categoryData = CATEGORY_TYPES_TO_SEARCH_CATEGORIES[categoryTypes[i]];
            for (int j = 0; j < categoryData.Filters.Length; j++) {
                categories.Add(categoryData.Filters[j]);
            }

            //string[] categories = {
            //    "Gas%20Station",
            //};
            string[] outFields = {
                "PlaceName",
                "Place_Addr",
                "City",
                "Region",
                "Location", 
            };
            float longitude = _locationController.getLongitude();
            float latitude = _locationController.getLatitude();
            int maxResults = AppConsts.DEFAULT_MAX_RESULTS;
            List<CandidateData> candidateData = ArcGSIRequestHelper.findAddressCandidates(categories.ToArray(), outFields, longitude, latitude, maxResults);

            Material candidateMaterial = GameObject.Instantiate(_candidatePrefab.meshRenderer.sharedMaterial);
            candidateMaterial.color = categoryData.Color;

            for (int j = 0; j < candidateData.Count; j++) {
                // Skip earth since yes, that's a data point
                if (candidateData[j].placeName.Equals("Earth")) {
                    continue;
                }
                //Debug.Log(j + ": " + candidateData[j].placeName + " " + candidateData[j].placeAddress + " " + candidateData[j].rect);

                CandidateController candidateInstance = GameObject.Instantiate(_candidatePrefab);
                candidateInstance.Initialize(candidateData[j]);
                candidateInstance.transform.position = new Vector3((candidateData[j].x - longitude) * AppConsts.MAP_SCALE_FACTOR, 0, (candidateData[j].z - latitude) * AppConsts.MAP_SCALE_FACTOR);

                candidateInstance.meshRenderer.sharedMaterial = candidateMaterial;

                _activeCandidates.Add(candidateInstance);
            }
        }
    }

    private void Update() {
        player.transform.position = _locationController.getLongLat() - _locationController.getOriginLongLat();

        Vector3 playerPos = player.transform.position;
        for (int i = 0; i < _activeCandidates.Count; i++) {
            _activeCandidates[i].UpdateNamePlate(ref playerPos);
        }
    }

}
