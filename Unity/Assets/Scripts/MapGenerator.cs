using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] CandidateController _candidatePrefab;

    [SerializeField] LocationController _locationController;

    public enum CategoryType {
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

    public static readonly List<CategoryType> DEFAULT_CATEGORY_TYPES = new List<CategoryType>() {
        CategoryType.Food,
        CategoryType.ShopsAndService,
        CategoryType.ArtsAndEntertainment,
        CategoryType.Education,
        CategoryType.LandFeatures,
        CategoryType.ParksAndOutdoors,
        CategoryType.ProfessionalAndOtherPlaces,
        CategoryType.Nightlife,
    };

    private const string CATEGORY_TYPES_KEY = "ctk";

    public static Transform player;
    private static MapGenerator _instance;

    private List<CandidateController> _activeCandidates;
    private bool _isDirty;

    public static HashSet<CategoryType> getAllSavedCategoryTypes() {
        HashSet<CategoryType> categoryTypes = new HashSet<CategoryType>();
        List<CategoryType> types = _instance.getSavedCategoryTypes();
        for (int i = 0; i < types.Count; i++) {
            categoryTypes.Add(types[i]);
        }
        return categoryTypes;
    }

    private List<CategoryType> getSavedCategoryTypes() {
        if (PlayerPrefs.HasKey(CATEGORY_TYPES_KEY)) {
            string serializedCategoryTypes = PlayerPrefs.GetString(CATEGORY_TYPES_KEY, String.Empty);
            if (!String.IsNullOrEmpty(serializedCategoryTypes)) {
                try {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<CategoryType>>(serializedCategoryTypes);
                } catch (Exception e) {
                    Debug.LogErrorFormat("Failed to deserialize {0} into a list of category types.", serializedCategoryTypes);
                }
            }
        }

        return DEFAULT_CATEGORY_TYPES;
    }

    private void saveCategoryTypes(List<CategoryType> categoryTypes) {
        if (categoryTypes == null) {
            categoryTypes = new List<CategoryType>();
        }
        PlayerPrefs.SetString(CATEGORY_TYPES_KEY, Newtonsoft.Json.JsonConvert.SerializeObject(categoryTypes));
    }

    private void Awake() {
        _instance = this;

        player = GameObject.Instantiate(_playerPrefab).transform;
        _activeCandidates = new List<CandidateController>();
        _isDirty = false;

        Screen.sleepTimeout = -1;
    }

    public static void ModifyFlag(CategoryType categoryType, bool isSet) {
        _instance._isDirty = true;

        List<CategoryType> categoryTypes = _instance.getSavedCategoryTypes();
        HashSet<CategoryType> categorySet = new HashSet<CategoryType>();
        for (int i = 0; i < categoryTypes.Count; i++) {
            categorySet.Add(categoryTypes[i]);
        }

        if (isSet) {
            categorySet.Add(categoryType);
        } else {
            categorySet.Remove(categoryType);
        }

        categoryTypes.Clear();
        foreach (CategoryType catType in categorySet) {
            categoryTypes.Add(catType);
        }

        _instance.saveCategoryTypes(categoryTypes);
    }

    public static void TryReload() {
        if (!_instance._isDirty) {
            return;
        }

        _instance._isDirty = false;

        _instance.StartCoroutine(_instance.CleanUpAndRestart());
    }

    private IEnumerator CleanUpAndRestart() {
        for (int i = 0; i < _instance._activeCandidates.Count; i++) {
            GameObject.Destroy(_instance._activeCandidates[i].gameObject);
        }
        _activeCandidates = new List<CandidateController>();
        yield return Start();
    }

    private IEnumerator Start() {
        while (!_locationController.isInitialized) {
            yield return null;
        }

        CategoryType[] categoryTypes = getSavedCategoryTypes().ToArray();
        Debug.Log(categoryTypes.Length);
        for (int i = 0; i < categoryTypes.Length; i++) {
            List<string> categories = new List<string>();
            CategoryData categoryData = CATEGORY_TYPES_TO_SEARCH_CATEGORIES[categoryTypes[i]];
            for (int j = 0; j < categoryData.Filters.Length; j++) {
                categories.Add(categoryData.Filters[j]);
            }

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
