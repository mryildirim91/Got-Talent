using System;
using UnityEngine;
using UnityEngine.AI;
public class Contestant : MonoBehaviour
{
    private bool _initialDestinationSet;
    private NavMeshAgent _agent;
    private Vector3 _destination;
    private AudioSource _source;
    [SerializeField] private float _talentAngle;

    public float TalentAngle => _talentAngle;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        EventManager.OnPerformanceStart.AddListener(PlayMusic);
        EventManager.OnPerformanceEnd.AddListener(EndPerformance);
        EventManager.OnVotingEnd.AddListener(LeaveStage);
    }

    private void OnDisable()
    {
        EventManager.OnPerformanceStart.RemoveListener(PlayMusic);
        EventManager.OnPerformanceEnd.RemoveListener(EndPerformance);
        EventManager.OnVotingEnd.RemoveListener(LeaveStage);
    }

    private void Update()
    {
        SetInitialDestination();
        ReachDestination();
    }

    private void SetInitialDestination()
    {
        if (GameManager.Instance.IsGameStarted && !_initialDestinationSet)
        {
            _initialDestinationSet = true;
            _destination = new Vector3(0.95f, 2.75f, 35.3f);
            _agent.SetDestination(_destination);
        }
    }

    private void ReachDestination()
    {
        if (Vector3.Distance(transform.position,_destination) < 1.5f && !_agent.isStopped )
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
        StopMusic();
        _agent.isStopped = false;
        _destination = new Vector3(0, 2.75f, 25);
        _agent.SetDestination(_destination);
    }
    
    private Quaternion RotateTowardsJury()
    {
        Vector3 targetDirection = Vector3.back - transform.position;
        float singleStep = _agent.speed * Time.deltaTime;
        Vector3 direction = Vector3.RotateTowards(transform.forward, targetDirection,singleStep, 0.0f);
        
        return Quaternion.LookRotation(direction);
    }

    private void LeaveStage()
    {
        _agent.isStopped = false;
        _destination = new Vector3(-20, 2.75f, 48);
        _agent.SetDestination(_destination);
    }

    private void PlayMusic()
    {
        if (gameObject.name == "4 Cyclist(Clone)" || gameObject.name == "12 Farmer(Clone)")
        {
            _source.PlayDelayed(3);
        }
        else
        {
            _source.PlayDelayed(0.5f);
        }
    }

    private void StopMusic()
    {
        _source.Stop();
    }
}
