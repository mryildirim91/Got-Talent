using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITalentmeter : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private RectTransform _arrow;
    
    private void OnEnable()
    {
        EventManager.OnPerformanceEnd.AddListener(ShowTalentmeter);
        EventManager.OnVotingEnd.AddListener(HideTalentMeter);
    }

    private void OnDisable()
    {
        EventManager.OnPerformanceEnd.RemoveListener(ShowTalentmeter);
        EventManager.OnVotingEnd.RemoveListener(HideTalentMeter);
    }

    private void ShowTalentmeter()
    {
        Invoke(nameof(ShowTalentmeterDelay), 9);
    }

    private void HideTalentMeter()
    {
        _panel.SetActive(false);
    }

    private void ShowTalentmeterDelay()
    {
        Contestant contestant = FindObjectOfType<Contestant>();
        _panel.SetActive(true);
        _arrow.rotation = Quaternion.Euler(0,0,contestant.TalentAngle);
    }
}
