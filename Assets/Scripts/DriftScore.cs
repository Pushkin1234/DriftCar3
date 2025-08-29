using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
public class DriftScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countCoinsNumberText;
    [SerializeField] private TextMeshProUGUI _countDriftScoreNumberText;
    [SerializeField] private TextMeshProUGUI _recordCountDriftScoreNumberText;

    [SerializeField] private TextMeshProUGUI _driftScoreMultiplier;
    [SerializeField] private TextMeshProUGUI _driftScoreText;
    [SerializeField] private TextMeshProUGUI _driftScoreTitle;

    [SerializeField] private float _driftTreshold = 10f;
    [SerializeField] private float _requiredSpeed = 10f;

    [SerializeField] private float _timeToEnroDriftScore;

    private RCC_CarControllerV3 _rccCarController;
    private bool _isDrifting = false;

    public float driftThreshold = 10f;
    public float[] scoreMilestones = { 500f, 1000f, 1500f, 2000f, 2500f };
    public float[] multipliers = { 2f, 3f, 4f, 5f, 6f };
    private int currentMultiplierIndex = 0;

    private float driftScore = 0f;
    private float driftStartTime = 0f;

    private bool _isActivateMultiplier = false;
    private float _timeWithoutDrift = 0;

    private int _countDriftScoreEarnedPerLevel = 0;
    private int _countCoinsEarnedPerLevel = 0;

    private void Start()
    {
        _rccCarController = FindObjectOfType<RCC_CarControllerV3>();
        _driftScoreMultiplier.gameObject.SetActive(false);
        _driftScoreText.gameObject.SetActive(false);
        _driftScoreTitle.gameObject.SetActive(false);

        _countCoinsNumberText.text = _countCoinsEarnedPerLevel.ToString();
        _countDriftScoreNumberText.text = _countDriftScoreEarnedPerLevel.ToString();

        _recordCountDriftScoreNumberText.text = SaveData.Instance.Data.RecordDriftScore.ToString();
    }

    private void Update()
    {
        float driftAngle = Vector3.Angle(_rccCarController.transform.forward, _rccCarController.Rigid.velocity);

        if (driftAngle > _driftTreshold && _rccCarController.speed > _requiredSpeed)
        {
            if (!_isDrifting)
            {
                StartDrift();
            }
            UpdateDrift(driftAngle);
            _timeWithoutDrift = 0;
        }
        else
        {
            _timeWithoutDrift += Time.deltaTime;

            if (_isDrifting && _timeWithoutDrift > _timeToEnroDriftScore)
            {
                EndDrift();
            }
        }

        _driftScoreText.text = "+" + Mathf.RoundToInt(driftScore).ToString();
    }

    private void StartDrift()
    {
        _isDrifting = true;
        _driftScoreText.gameObject.SetActive(true);
        _driftScoreTitle.gameObject.SetActive(true);
    }
    private void UpdateDrift(float driftAngle)
    {
        float driftDuration = Time.time - driftStartTime;

        if(_isActivateMultiplier)
        {
            driftScore += driftAngle * Time.deltaTime * multipliers[currentMultiplierIndex];
        }
        else
        {
            driftScore += driftAngle * Time.deltaTime;
        }

        CheckForMilestones();
    }

    private void CheckForMilestones()
    {
        if (currentMultiplierIndex < (scoreMilestones.Length-1) && driftScore >= scoreMilestones[currentMultiplierIndex])
        {
            _isActivateMultiplier = true;
            currentMultiplierIndex++;
            _driftScoreMultiplier.text = multipliers[currentMultiplierIndex].ToString();
            _driftScoreMultiplier.gameObject.SetActive(true);
            Debug.Log("multiplier increased! current multiplier: " + multipliers[currentMultiplierIndex]);
        }
    }
    private void EndDrift()
    {
        Debug.Log("Зашли в EndDrift");
        _isDrifting = false;
        _timeWithoutDrift = 0;
        Debug.Log("Drift Score: " + driftScore);
        SaveData.Instance.Data.Coins += Convert.ToInt32(driftScore * 0.5F);
        _countCoinsEarnedPerLevel += Convert.ToInt32(driftScore * 0.5f);
        _countDriftScoreEarnedPerLevel += Convert.ToInt32(driftScore);

        _driftScoreMultiplier.gameObject.SetActive(false);
        _driftScoreText.gameObject.SetActive(false);
        _driftScoreTitle.gameObject.SetActive(false);

        _countCoinsNumberText.text = _countCoinsEarnedPerLevel.ToString();
        _countDriftScoreNumberText.text = _countDriftScoreEarnedPerLevel.ToString();

        if(driftScore > SaveData.Instance.Data.RecordDriftScore)
        {
            SaveData.Instance.Data.RecordDriftScore = Convert.ToInt32(driftScore);
            _recordCountDriftScoreNumberText.text = SaveData.Instance.Data.RecordDriftScore.ToString();
        }

        currentMultiplierIndex = 0;

        driftScore = 0;

        SaveData.Instance.SaveYandex();
    }

}
