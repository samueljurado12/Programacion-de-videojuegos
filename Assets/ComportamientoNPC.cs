using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ESTE EL CODIGO BASICO  (CB1) DEL TEMA 2.4-A

public class ComportamientoNPC : MonoBehaviour{
    public float velocFrontal = 0;
    public float velocGiro = 0;
    Rigidbody rb;                                              //Puede congelar (Freeze) rotaciones en X Z; también la posición Y.

    private void Start()
    {
        velocGiro = 20f;
        velocFrontal = 3;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * velocGiro * Time.deltaTime);
        rb.velocity = transform.forward * velocFrontal;    
    }
    private void OnCollisionEnter(Collision collision)
    {
        print("Colisión con " + collision.gameObject);
    }
}