using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private AttributeData _attributeData;
    [SerializeField]
    private Slider _slider;

    private void OnValidate()
    {
        _slider = GetComponent<Slider>();
    }

    void Update()
    {
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                         _mainCamera.transform.rotation * Vector3.up);
        _slider.value = (float) (_attributeData.HP) / _attributeData.MaxHP;
    }
}
