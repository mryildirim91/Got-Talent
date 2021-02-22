using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UITalentmeter : MonoBehaviour
{
    [SerializeField] private Ease easeType;
    [SerializeField] private GameObject _panel, _namePanel;
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
        StartCoroutine(ShowTalentmeterDelay());
    }

    private void HideTalentMeter()
    {
        _namePanel.SetActive(false);
        _panel.SetActive(false);
    }

    private IEnumerator ShowTalentmeterDelay()
    {
        yield return BetterWaitForSeconds.Wait(2);
        _namePanel.SetActive(true);
        yield return BetterWaitForSeconds.Wait(4);
        Contestant contestant = FindObjectOfType<Contestant>();
        _panel.SetActive(true);
        _arrow.rotation = Quaternion.Euler(0,0,0);
        yield return BetterWaitForSeconds.Wait(0.1f);
        _arrow.DORotate(Vector3.forward * -180, 0.5f, RotateMode.LocalAxisAdd).SetEase(easeType);
        yield return BetterWaitForSeconds.Wait(0.6f);
        _arrow.DORotate(Vector3.forward * 180, 0.5f,RotateMode.LocalAxisAdd).SetEase(easeType);
        yield return BetterWaitForSeconds.Wait(0.6f);
        _arrow.DORotate(Vector3.back * contestant.TalentAngle, 0.5f,RotateMode.LocalAxisAdd).SetEase(easeType);
        StopCoroutine(ShowTalentmeterDelay());
    }
}
