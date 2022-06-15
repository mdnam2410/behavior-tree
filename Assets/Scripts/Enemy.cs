using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Targets;

    private float _fieldOfView = 120f;
    private float _radius = 10;
    private GameObject _target;
    private float _currentTimeToMoveRandom = 3;
    private float _limitTimeToMoveRandom = 3;
    private float _currentTimeToBack = 7;
    private float _limitTimeToBack = 7;
    private float _minDistanceToRandom = 3f;
    private float _limitDistanceToRandom = 8f;
    private ActorState _actorState;

    private Vector3 _backPosition;

    private NavMeshAgent _navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_navMeshAgent.velocity.magnitude <= 0)
        {
            _actorState = ActorState.Idle;
            if(_target != null)
            {
                _currentTimeToBack = _limitTimeToBack;
            }
        }
        if (_target == null)
        {
            if(_actorState == ActorState.Idle)
            {
                _currentTimeToMoveRandom -= Time.deltaTime;
                if (_currentTimeToMoveRandom <= 0)
                {
                    _currentTimeToMoveRandom = _limitTimeToMoveRandom;
                    var angle = Random.RandomRange(0, 360f);
                    var d = Random.RandomRange(_minDistanceToRandom, _limitDistanceToRandom);
                    var newDirection = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
                    var randomPosition = transform.position + newDirection * d;
                    NavMeshHit hit;
                    var result = NavMesh.SamplePosition(randomPosition, out hit, float.MaxValue, NavMesh.AllAreas);
                    if (result)
                    {
                        _actorState = ActorState.Move;
                        _navMeshAgent.SetDestination(randomPosition);
                    }
                }
            }

            for (int i = 0; i < Targets.Count; i++)
            {
                var target = Targets[i];
                var direction = (target.transform.position - transform.position);
                var angle = Vector3.Angle(direction, transform.forward);
                if (direction.magnitude < _radius && angle < _fieldOfView * 0.5f)
                {
                    _target = target;
                    _backPosition = transform.position;
                    break;
                }
            }
        }
        else
        {
            _actorState = ActorState.Move;
            _currentTimeToBack -= Time.deltaTime;
            if(_currentTimeToBack <= 0)
            {
                _currentTimeToBack = _limitTimeToBack;
                _target = null;
                _navMeshAgent.SetDestination(_backPosition);
            }
            else
            {
                _navMeshAgent.SetDestination(_target.transform.position);
            }
        }
    }
}
