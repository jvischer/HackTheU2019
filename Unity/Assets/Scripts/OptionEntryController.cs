using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionEntryController : MonoBehaviour {

    [SerializeField] private Toggle _toggleButton;
    [SerializeField] private TMPro.TextMeshProUGUI _displayNameLabel;

    private MapGenerator.CategoryType _categoryType;

    public void Initialize(MapGenerator.CategoryType categoryType, bool defaultValue) {
        _categoryType = categoryType;
        _displayNameLabel.text = categoryType.ToString();
        _toggleButton.isOn = defaultValue;
    }

    public bool isSet {
        get {
            return _toggleButton.isOn;
        }
        set {
            MapGenerator.ModifyFlag(_categoryType, value);
        }
    }

}
