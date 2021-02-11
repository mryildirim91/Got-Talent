using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UILevelComplete : MonoBehaviour
{
    [SerializeField]private GameObject _levelCompletePanel;

    private void OnEnable()
    {
        EventManager.OnLevelComplete.AddListener(OpenPanel);
    }

    private void OnDisable()
    {
        EventManager.OnLevelComplete.RemoveListener(OpenPanel);
    }

    private void OpenPanel()
    {
        Invoke(nameof(DelayPanel),2);
    }

    private void DelayPanel()
    {
        _levelCompletePanel.SetActive(true);
        RectTransform rectTransform = _levelCompletePanel.transform.GetChild(0).GetComponent<RectTransform>();
        rectTransform.DOAnchorPos(new Vector2(0, 0), 0.5f); 
    }
}
