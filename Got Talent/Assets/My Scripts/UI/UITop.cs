using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITop : MonoBehaviour
{
    public static UITop Instance { get; private set; }
    private int _numOfStars;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject[] _stars;

    private void Awake()
    {
        Instance = this;
        GiveStars();
    }

    public void GiveStars()
    {
        _stars[_numOfStars].SetActive(true);
        _numOfStars++;
        _slider.value = _numOfStars;
    }
    
}
