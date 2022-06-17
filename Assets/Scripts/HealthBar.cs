using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private AttributeData _attributeData;

    void Update()
    {
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                         _mainCamera.transform.rotation * Vector3.up);
        _text.SetText($"Health: {_attributeData.HP}/{_attributeData.MaxHP}");
    }
}
