using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// public class UnityEventGameObject : UnityEvent<GameObject>
// {
// }

public class Projectile : MonoBehaviour
{
    public float Speed = 10f;
    private float _limitRange = 50;
    private float _currentRange = 0;
    [HideInInspector]
    public GameObject Caster;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _currentRange += Speed * Time.deltaTime;
        transform.position = transform.position + transform.forward * Speed * Time.deltaTime;
        if(_currentRange >= _limitRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Caster != other.gameObject)
        {
            GameObject victim = other.gameObject;
            Debug.Log($"{Caster.name}'s projectile hit {victim.name}");
            
            UpdateDamage(Caster, victim);
            if (victim.name.StartsWith("Enemy"))
            {
                victim.GetComponent<EnemyAI>().GetHit();
            }
            Destroy(gameObject);
        }
    }

    private void UpdateDamage(GameObject attacker, GameObject victim)
    {
        int attackerDamage = attacker.GetComponent<AttributeData>().Attack;
        AttributeData victimAttributeData = victim.GetComponent<AttributeData>();
        victimAttributeData.ReceiveDamage(attackerDamage);
    }
}
