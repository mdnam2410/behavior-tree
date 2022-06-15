using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField]
    GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        _offset = Camera.main.transform.position - Target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target.activeInHierarchy == true)
        {
            var targetPosition = Target.transform.position + _offset;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, 10 * Time.deltaTime);
        }
    }
}
