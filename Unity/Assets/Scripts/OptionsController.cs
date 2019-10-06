using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class OptionsController : MonoBehaviour {

    [SerializeField] private OptionEntryController _entryPrefab;
    [SerializeField] private Transform _entryRoot;

    [Space]

    [SerializeField] private Animator _animator;

    private bool _isShowingPopup;

    private void Start() {
        _animator = gameObject.GetComponent<Animator>();

        HashSet<MapGenerator.CategoryType> allSavedCategoryTypes = MapGenerator.getAllSavedCategoryTypes();
        for (int i = 0; i < MapGenerator.DEFAULT_CATEGORY_TYPES.Count; i++) {
            MapGenerator.CategoryType categoryType = MapGenerator.DEFAULT_CATEGORY_TYPES[i];
            OptionEntryController entryController = GameObject.Instantiate(_entryPrefab, _entryRoot);
            entryController.Initialize(categoryType, allSavedCategoryTypes.Contains(categoryType));
        }
    }

    public void showOptions() {
        if (_isShowingPopup) {
            return;
        }

        _isShowingPopup = true;
        _animator.SetTrigger("Show");
    }

    public void hidePopup() {
        _animator.SetTrigger("Hide");
    }

    public void onPopupHidden() {
        _isShowingPopup = false;

        MapGenerator.TryReload();
    }

}
