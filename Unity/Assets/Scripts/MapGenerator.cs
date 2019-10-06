using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] CandidateController _candidatePrefab;

    [SerializeField] LocationController _locationController;

    private Transform _player;

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
            Color = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
        } },
        { CategoryType.Education, new CategoryData {
            Filters = new string[] { "Education" },
            Color = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
        } },
        { CategoryType.Food, new CategoryData {
            Filters = new string[] { "Food" },
            Color = new Color32(0x00, 0xFF, 0xFF, 0xFF),
        } },
        { CategoryType.ShopsAndService, new CategoryData {
            Filters = new string[] { "Shops and Service" },
            Color = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
        } },
        { CategoryType.LandFeatures, new CategoryData {
            Filters = new string[] { "Land Features" },
            Color = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
        } },
        { CategoryType.Nightlife, new CategoryData {
            Filters = new string[] { "Nightlife Spot" },
            Color = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
        } },
        { CategoryType.ParksAndOutdoors, new CategoryData {
            Filters = new string[] { "Parks and Outdoors" },
            Color = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
        } },
        { CategoryType.ProfessionalAndOtherPlaces, new CategoryData {
            Filters = new string[] { "Professional and Other Places" },
            Color = new Color32(0xFF, 0xFF, 0xFF, 0xFF),
        } },
    };

    private void Awake() {
        _player = GameObject.Instantiate(_playerPrefab).transform;

        CategoryType[] categoryTypes = {
            CategoryType.Food,
            CategoryType.ShopsAndService,
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
            float latitude = AppConsts.DEFAULT_LATITUDE;
            float longitude = AppConsts.DEFAULT_LONGITUDE;
            int maxResults = AppConsts.DEFAULT_MAX_RESULTS;
            List<CandidateData> candidateData = ArcGSIRequestHelper.findAddressCandidates(categories.ToArray(), outFields, latitude, longitude, maxResults);
            Debug.Log(candidateData.Count + " candidate data points");

            Material candidateMaterial = GameObject.Instantiate(_candidatePrefab.meshRenderer.sharedMaterial);
            candidateMaterial.color = categoryData.Color;

            for (int j = 0; j < candidateData.Count; j++) {
                Debug.Log(j + ": " + candidateData[j].placeName + " " + candidateData[j].placeAddress + " " + candidateData[j].rect);

                CandidateController candidateInstance = GameObject.Instantiate(_candidatePrefab);
                candidateInstance.name = j + ": " + candidateData[j].placeName;
                candidateInstance.transform.position = new Vector3((candidateData[j].x - latitude) * AppConsts.MAP_SCALE_FACTOR, 0, (candidateData[j].z - longitude) * AppConsts.MAP_SCALE_FACTOR);

                candidateInstance.meshRenderer.sharedMaterial = candidateMaterial;
            }
        }
    }

    private void Update() {
        _player.transform.position = _locationController.getLatLong() - _locationController.getOriginLatLong();
    }

}
