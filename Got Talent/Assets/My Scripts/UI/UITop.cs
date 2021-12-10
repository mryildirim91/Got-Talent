using UnityEngine;
using UnityEngine.UI;

public class UITop : MonoBehaviour
{
    public static UITop Instance { get; private set; }
    private int _currentEpisode;
    public int NumOfStars { get; private set; }

    [SerializeField]private Slider _slider;
    [SerializeField]private GameObject[] _stars;
    [SerializeField]private Text _episode, _totalStar;

    private void Awake()
    {
        Instance = this;
        _currentEpisode = PlayerPrefs.GetInt("CurrentEpisode") + 1;
        _episode.text = "Episode " + _currentEpisode;
        _totalStar.text = PlayerPrefs.GetInt("TotalStars").ToString();
    }

    public void GiveStars()
    {
        _stars[NumOfStars].SetActive(true);
        NumOfStars++;
        PlayerPrefs.SetInt("TotalStars", PlayerPrefs.GetInt("TotalStars") + 1);
        _totalStar.text = PlayerPrefs.GetInt("TotalStars").ToString();
        _slider.value = NumOfStars;
    }
    
}
