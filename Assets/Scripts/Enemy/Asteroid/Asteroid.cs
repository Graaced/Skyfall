using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 25f;
    [SerializeField] private float damage = 50f;

    private Vector3 target;
    public bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * fallSpeed * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(direction);
    }


    public void SetTarget(Vector3 planetPosition) 
    {
        target = planetPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Planet"))
        {
            // Notify SpawnManager to handle the asteroid hit
            FindObjectOfType<SpawnManager>().OnAsteroidHit(this,collision);
        }
    }

    
}
