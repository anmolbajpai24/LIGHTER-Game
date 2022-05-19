using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject target;

    private Vector3 offset;

    private void Awake()
    {
        target = GameObject.FindWithTag("Player");
    }
    // Start is called before the first frame update
    void Start()
    {
        if(target)
        {
            offset = transform.position - target.transform.position;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(target)
        {
            transform.position = target.transform.position + offset;
        }
    }
}
