using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeData : MonoBehaviour
{
    public int HP = 100;
    public int Attack = 50;

    public void ReceiveDamage(int damage)
    {
        HP -= damage;
    }
}
