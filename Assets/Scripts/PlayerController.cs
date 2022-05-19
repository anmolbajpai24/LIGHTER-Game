using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 25f, rotationSpeed = 180f;

    [SerializeField]
    private bool playerCar = false;

    [SerializeField]
    private GameObject explosion;

    private Rigidbody body;

    private float translation, rotation;

   // [SerializeField]
    private GameObject target;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player");
    }
    // Update is called once per frame
    void Update()
    {
        if(!playerCar && !target)
        {
            return;
        }

        if(playerCar)
        {
            if(Mathf.Abs(Input.GetAxis("Vertical"))>0)
            {
                rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
                translation = Input.GetAxis("Vertical") * speed;
            }

            else
            {
                rotation = 0f;
                translation = 0f;
            }
            
        }
        
        else
        {
            Vector3 targetDirection = transform.position - target.transform.position;
            targetDirection.Normalize();

            rotation = Vector3.Cross(targetDirection, transform.forward).y;
        }
    }

    private void FixedUpdate()
    {
        if(playerCar)
        {
            body.velocity = transform.forward * translation;
            transform.Rotate(Vector3.up * rotation);
        }

        else
        {
            body.angularVelocity = rotationSpeed * rotation * new Vector3(0, 1, 0);
            body.velocity = transform.forward * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            //Instantiate(explosion, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            Destroy(collision.gameObject);
            Destroy(gameObject);
            
        }
    }
}
