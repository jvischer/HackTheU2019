using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PopupController : MonoBehaviour {

    [SerializeField] private Animator _animator;
    [SerializeField] private TMPro.TextMeshProUGUI _nameLabel;
    [SerializeField] private TMPro.TextMeshProUGUI _addressLabel;
    [SerializeField] private Button _mapsButton;
    [SerializeField] private Button _exitButton;

    private static PopupController _instance;

    private bool _isShowingPopup;

    private void Awake() {
        _instance = this;

        _animator = gameObject.GetComponent<Animator>();
    }

    public static void showCandidate(CandidateData candidateData) {
        if (_instance == null || _instance._isShowingPopup) {
            return;
        }

        Vector3 playerLongLat = LocationController.getPlayerLongLat();

        _instance._nameLabel.text = candidateData.placeName;
        _instance._addressLabel.text = candidateData.placeAddress;
        _instance._mapsButton.onClick.RemoveAllListeners();
        _instance._mapsButton.onClick.AddListener(() => { Application.OpenURL(String.Format("https://www.google.com/maps/dir/'{0},{1}'/'{2},{3}'", playerLongLat.z, playerLongLat.x, candidateData.z, candidateData.x)); });
        _instance._exitButton.onClick.RemoveAllListeners();
        _instance._exitButton.onClick.AddListener(() => { _instance.hidePopup(); });

        _instance._isShowingPopup = true;
        _instance._animator.SetTrigger("Show");
    }

    public void hidePopup() {
        _animator.SetTrigger("Hide");
    }

    public void onPopupHidden() {
        _isShowingPopup = false;
    }

}
