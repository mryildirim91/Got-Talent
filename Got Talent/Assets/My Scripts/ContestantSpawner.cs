using UnityEngine;
public class ContestantSpawner : MonoBehaviour
{
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private GameObject[] _contestants;
    private void Start()
    {
        SpawnContestant();
    }
    private void SpawnContestant()
    {
        int contestantIndex = PlayerPrefs.GetInt("ContestantIndex");
        GameObject obj = ObjectPool.Instance.GetObject(_contestants[contestantIndex]);
        obj.transform.position = _spawnPos.position;
    }
}
