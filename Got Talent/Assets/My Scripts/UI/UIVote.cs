using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVote : MonoBehaviour
{
    public static UIVote Instance { get; private set; }
    [SerializeField] private Image[] _image;
    [SerializeField] private Transform[] _transforms;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowVoteImage(int index, bool status)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_transforms[index].position);
        _image[index].gameObject.SetActive(status);
        _image[index].transform.position = pos;
    }
}
