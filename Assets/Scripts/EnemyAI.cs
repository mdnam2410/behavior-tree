using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner;
using BehaviorDesigner.Runtime;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Targets;
    [SerializeField]
    ExternalBehavior ExternalBehavior;
    [SerializeField]
    private GameObject _projectilePrefab;
    [SerializeField]
    [HideInInspector]
    private AttributeData _attributeData;

    BehaviorTree _behaviorTree;
    SharedFloat _speed;
    SharedFloat _viewDistance;
    SharedFloat _fieldOfView;
    SharedGameObject _target;
    SharedGameObjectList _targets;

    private void OnValidate()
    {
        _attributeData = GetComponent<AttributeData>();    
    }

    void Start()
    {
        _behaviorTree = gameObject.AddComponent<BehaviorTree>();
        _behaviorTree.ExternalBehavior = ExternalBehavior;

        _speed = _behaviorTree.GetVariable("Speed") as SharedFloat;
        _viewDistance = _behaviorTree.GetVariable("ViewDistance") as SharedFloat;
        _fieldOfView = _behaviorTree.GetVariable("FieldOfView") as SharedFloat;
        _target = _behaviorTree.GetVariable("Target") as SharedGameObject;
        _targets = _behaviorTree.GetVariable("Targets") as SharedGameObjectList;

        _speed.Value = 6;
        _viewDistance.Value = 10;
        _fieldOfView.Value = 120;
        _targets.Value.AddRange(Targets);

        _behaviorTree.RegisterEvent("Spawn Projectile", OnSpawnProjectile);

        _behaviorTree.EnableBehavior();
    }

    private void Update()
    {
        if (_attributeData.HP <= 0)
        {
            Destroy(gameObject);
        }

        _behaviorTree.SetVariable("AllPlayersAreDead", (SharedBool) PlayerManager.Instance.AllPlayerAreDead);
    }

    private void OnSpawnProjectile()
    {
        // Look at the player
        transform.LookAt(_target.Value.transform);

        List<float> angles = new List<float> { -20, -10, 0, 10, 20 };
        for (int i = 0; i < angles.Count; ++i)
        {
            // Spawn in the direction of the gun
            Vector3 projectileDirection = Quaternion.AngleAxis(angles[i], Vector3.up) * transform.forward;
            var projectileLaunchPosition = transform.position + projectileDirection * 2f + new Vector3(0, 0.5f, 0);

            // Launch the projectile
            LaunchProjectile(projectileDirection, projectileLaunchPosition);
        }
    }

    private void LaunchProjectile(Vector3 direction, Vector3 position)
    {
        GameObject projectileGO = Instantiate(_projectilePrefab);
        projectileGO.transform.forward = direction;
        projectileGO.transform.position = position;
        
        var projectileComponent = projectileGO.GetComponent<Projectile>();
        projectileComponent.Caster = gameObject;
    }

    public void GetHit()
    {
        Debug.Log($"{gameObject.name} gets hit");
        _behaviorTree.SendEvent("Get Hit");
    }
}
