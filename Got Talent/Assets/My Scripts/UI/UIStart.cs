using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIStart : MonoBehaviour
{
    [SerializeField] private Text _startText;

    private void Start()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        while (!GameManager.Instance.IsGameStarted)
        {
            yield return BetterWaitForSeconds.Wait(1.1f);
            _startText.DOFade(0f, 1f);
            yield return BetterWaitForSeconds.Wait(1.1f);
            _startText.DOFade(1f, 1f);
        }
    }
}
