using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WaypointPotrol : MonoBehaviour
{
    [SerializeField] bool _patrolWaitng;
    [SerializeField] float _totalWaitTime = 3f;
    [SerializeField] float _switchProbability = 0.2f;
    [SerializeField] List<Waypoint> _patrolPoints;

    NavMeshAgent _navMeshAgent;
    int _currentPatrolIndex;
    bool _travelling;
    bool _wating;
    bool _patrolForward;
    float _waitTimer;
        
    public void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(_navMeshAgent == null)
        {
            Debug.LogError("Game Object Error is " + gameObject.name);
        }    
        else
        {
            if(_patrolPoints != null && _patrolPoints.Count >= 2)
            {
                _currentPatrolIndex = 0;
                SetDestination();
            }
            else
            {
                Debug.Log("points for basic");
            }
        }
    }


    // Update is called once per frame
    public void Update()
    {
        if(_travelling && _navMeshAgent.remainingDistance <= 1.0f)
        {
            _travelling = false;

            if(_patrolWaitng)
            {
                _wating = true;
                _waitTimer = 0f;
            }
            else
            {
                 ChangePatrolPoint();
                SetDestination();
            }
        }

        if (_wating)
        {
            _waitTimer += Time.deltaTime;
            if(_waitTimer >= _totalWaitTime)
            {
                _wating = false;

                ChangePatrolPoint();
                SetDestination();
            }
        }
    }

   

    private void SetDestination()
    {
        if (_patrolPoints != null)
        {
            Vector3 tragetVector = _patrolPoints[_currentPatrolIndex].transform.position;
            _navMeshAgent.SetDestination(tragetVector);
            _travelling = true;
        }
    }
    private void ChangePatrolPoint()
    {
        //if (UnityEngine.Random.Range(0f,1f) <= _switchProbability)
        //{
        //    _patrolForward = !_patrolForward;
        //}

        if(_patrolForward)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Count;
        }
        else
        {
            if (--_currentPatrolIndex < 0)
            {
                _currentPatrolIndex = _patrolPoints.Count - 1;
            }
        }
    }

}
