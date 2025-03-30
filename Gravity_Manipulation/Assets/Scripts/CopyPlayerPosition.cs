using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPlayerPosition : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    void Update()
    {
        transform.position = playerObject.transform.position;
    }
}
