using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner;
using BehaviorDesigner.Runtime;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject _projectilePrefab;
    [SerializeField]
    private AttributeData _attributeData;

    private NavMeshAgent _navMeshAgent;

    private void OnValidate()
    {
        _attributeData = GetComponent<AttributeData>();
    }

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if(Physics.Raycast(ray, out raycastHit, 1000))
            {
                _navMeshAgent.SetDestination(raycastHit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<float> angles = new List<float> { -20, -10, 0, 10, 20 };
            for(int i = 0; i < angles.Count; i++)
            {
                var projectile = Instantiate(_projectilePrefab);

                var projectileComponent = projectile.GetComponent<Projectile>();
                projectileComponent.Caster = gameObject;

                var newDirection = Quaternion.AngleAxis(angles[i], Vector3.up) * transform.forward;
                projectile.transform.forward = newDirection;
                projectile.transform.position = transform.position + newDirection * 2f + new Vector3(0, 0.5f, 0);
            }
        }

    }

    private void LateUpdate()
    {
        if (_attributeData.HP <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
