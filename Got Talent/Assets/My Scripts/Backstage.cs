using UnityEngine;

public class Backstage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Contestant"))
        {
            ObjectPool.Instance.ReturnGameObject(other.gameObject);
            EventManager.OnPlayerLeftStage.Invoke();
        }
    }
}
