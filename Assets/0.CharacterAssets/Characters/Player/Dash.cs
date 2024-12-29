using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
public class Dash : MonoBehaviour {

    [SerializeField]
    private float initialDashStrength = 10f;
    [SerializeField]
    private float maxChargeDashStrength = 15f;
    [Tooltip("second")]
    [SerializeField]
    private float dashChargeTime = 1.0f;
    // charge curve
    [SerializeField]
    private AnimationCurve chargeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    private float dashTime = 0.25f;
    [SerializeField]
    private int maxDashes = 1; // max dashs while in air
    private int dashCount = 0;
    // private float currentDashCharge = 0;
    public float currentDashChargeTime = 0.0f;
    public float currentDashCharge = 0.0f;
    public bool isChargingDash = false;
    public bool queueDash = false;
    private float initCamFov;


    public bool queueDashCharger = false;
    private float dashTimer = 0;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
