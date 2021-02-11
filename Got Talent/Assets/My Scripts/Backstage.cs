using UnityEngine;

public class Backstage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Contestant")
        {
            EventManager.OnPlayerLeftStage.Invoke();
        }
    }
}
