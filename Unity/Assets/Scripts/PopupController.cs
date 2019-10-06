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

        _instance._nameLabel.text = candidateData.placeName;
        _instance._addressLabel.text = candidateData.placeAddress;
        _instance._mapsButton.onClick.RemoveAllListeners();
        _instance._mapsButton.onClick.AddListener(() => { Application.OpenURL(String.Format("https://www.google.com/maps/@{0},{1}", candidateData.x, candidateData.z)); });

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
