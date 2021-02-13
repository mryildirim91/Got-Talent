using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UILevelComplete : MonoBehaviour
{
    [SerializeField]private GameObject _levelCompletePanel;
    [SerializeField] private GameObject[] _stars;

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
        Invoke(nameof(DelayPanel),1);
    }

    private void DelayPanel()
    {
        _levelCompletePanel.SetActive(true);
        RectTransform rectTransform = _levelCompletePanel.transform.GetChild(0).GetComponent<RectTransform>();
        rectTransform.DOAnchorPos(new Vector2(0, -1100), 0.5f);
        StartCoroutine(LevelEndStarAnimation());
    }
    
    private IEnumerator LevelEndStarAnimation()
    {
        yield return BetterWaitForSeconds.Wait(0.5f);
        
        _stars[0].SetActive(true);
        _stars[0].transform.DOScale(Vector3.one, 0.1f);

        if (UITop.Instance.NumOfStars > 1)
        {
            yield return BetterWaitForSeconds.Wait(0.4f);
            _stars[1].SetActive(true);
            _stars[1].transform.DOScale(Vector3.one, 0.1f);
        }
        if(UITop.Instance.NumOfStars > 2)
        {
            yield return BetterWaitForSeconds.Wait(0.4f);
            _stars[2].SetActive(true);
            _stars[2].transform.DOScale(Vector3.one, 0.1f);
        }
        
        StopCoroutine(LevelEndStarAnimation());
    }
}
