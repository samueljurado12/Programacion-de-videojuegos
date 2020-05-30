using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ESTE EL CODIGO DE PROYECTIL (CP1) DEL TEMA 2.4-C
// Note produce efectos de explosión para objetos del layer = 8 solamente

public class compBala : MonoBehaviour{

    void Update(){
        transform.Translate(Vector3.forward * 50f * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other) {   
        if (other.gameObject.layer == 8) { 
            float radio = 10;
            Collider[] coliders = Physics.OverlapSphere(transform.position, radio, 1 << 8);
            for (int i=0;  i<coliders.Length; i++){
                Rigidbody rb = coliders[i].GetComponent<Rigidbody>();               
                if (rb == null) continue;
                float fuerza = 10000; 
                rb.AddExplosionForce(fuerza, transform.position, radio);
                Destroy(gameObject);
 }}}}

