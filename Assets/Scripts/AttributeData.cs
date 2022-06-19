using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeData : MonoBehaviour
{
    public int MaxHP;
    public int HP;
    public int Attack = 50;

    public void OnValidate()
    {
        HP = MaxHP;
    }

    public void ReceiveDamage(int damage)
    {
        HP -= damage;
    }
}
