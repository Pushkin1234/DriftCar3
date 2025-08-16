using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DriftScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countCoinsNumberText;
    [SerializeField] private TextMeshProUGUI _countDriftScoreNumberText;
    [SerializeField] private TextMeshProUGUI _recordCountdriftScoreNumberText;

    [SerializeField] private TextMeshProUGUI _driftScoreMultiplier;
    [SerializeField] private TextMeshProUGUI _driftScoreText;
    [SerializeField] private TextMeshProUGUI _driftScoreTitle;

    [SerializeField] private float _driftTreshold = 10f;
    [SerializeField] private float _requiredSpeed = 10f;

    [SerializeField] private float _timeToEnroDriftScore;

    private RCC_CarControllerV3 _rccCarController;
    private bool _isDrifting = false;

    public float driftThreshold = 10f;
    public float[] scoreMulestones = { 500f, 1000f, 1500f, 2000f, 2500f };
    public float[] multipliers = { 2f, 3f, 4f, 5f, 6f };
    private int currentMultiplierIndex = 0;

    private float driftScore = 0f;
    private float driftStartTime = 0f;

    private bool _isActivateMultiplier = false;
    private float _timeWithoutDrift = 0;

    private int _countDriftScoreEarnedperLevel = 0;
    private int _countCoinsEarnedPerLevel = 0;

}
