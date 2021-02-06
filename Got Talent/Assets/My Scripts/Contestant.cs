using UnityEngine;
using UnityEngine.AI;
public class Contestant : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField]private Vector3 _destination;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _agent.SetDestination(_destination);
    }

    private void Update()
    {
        StartPerformance();
    }

    private void StartPerformance()
    {
        if (Vector3.Distance(transform.position,_destination) <= 1 && !_agent.isStopped)
        {
            transform.rotation = RotateTowardsJury();
            
            if (transform.position == _destination)
            {
                _agent.isStopped = true;
                Debug.Log(_agent.isStopped);
            }
        }
    }

    private Quaternion RotateTowardsJury()
    {
        Vector3 targetDirection = Vector3.back - transform.position;
        float singleStep = _agent.speed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection,singleStep, 0.0f);
        
        return Quaternion.LookRotation(newDirection);
    }
}
