using System;
using UnityEngine;
using UnityEngine.AI;
public class Contestant : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Vector3 _destination;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        SetInitialDestination();
    }

    private void OnEnable()
    {
        EventManager.OnPerformanceEnd.AddListener(EndPerformance);
    }

    private void OnDisable()
    {
        EventManager.OnPerformanceEnd.RemoveListener(EndPerformance);
    }

    private void Update()
    {
        ReachDestination();
    }

    private void SetInitialDestination()
    {
        _destination = new Vector3(0.95f, 2.75f, 35.3f);
        _agent.SetDestination(_destination);
    }

    private void ReachDestination()
    {
        if (Vector3.Distance(transform.position,_destination) < 0.5f && !_agent.isStopped )
        {
            _agent.isStopped = true;
            EventManager.OnReachDestination.Invoke();
        }
        if (_agent.isStopped)
        {
            transform.rotation = RotateTowardsJury();
        }
    }

    private void EndPerformance()
    {
        _agent.isStopped = false;
        _destination = new Vector3(0.95f, 2.75f, 24);
        _agent.SetDestination(_destination);
    }
    
    private Quaternion RotateTowardsJury()
    {
        Vector3 targetDirection = Vector3.back - transform.position;
        float singleStep = _agent.speed * Time.deltaTime;
        Vector3 direction = Vector3.RotateTowards(transform.forward, targetDirection,singleStep, 0.0f);
        
        return Quaternion.LookRotation(direction);
    }
}
