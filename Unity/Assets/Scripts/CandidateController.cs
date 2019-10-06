using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandidateController : MonoBehaviour {

    public MeshRenderer meshRenderer;
    public TMPro.TextMeshProUGUI nameLabel;
    public Animator candidateAnimator;

    private CandidateData _candidateData;
    private Vector3 _initialNameLabelScale;

    public void Initialize(CandidateData candidateData) {
        _candidateData = candidateData;
        _initialNameLabelScale = nameLabel.transform.localScale;

        gameObject.name = _candidateData.placeName;
        nameLabel.text = _candidateData.placeName;

        //candidateAnimator.SetFloat("Offset", Random.Range(0.0F, 1.0F));
    }

    public void UpdateNamePlate(ref Vector3 playerPos) {
        float candidateDistanceFromPlayer = Vector3.SqrMagnitude(playerPos - transform.position);
        if (candidateDistanceFromPlayer <= AppConsts.MAX_VISIBLE_NAME_PLATE_DISTANCE) {
            nameLabel.transform.localScale = (1 - Mathf.Clamp01(candidateDistanceFromPlayer / AppConsts.MAX_VISIBLE_NAME_PLATE_DISTANCE)) * _initialNameLabelScale;
        } else {
            nameLabel.transform.localScale = Vector3.zero;
        }
    }

}
