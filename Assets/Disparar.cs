using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ESTE EL CODIGO DE DISPARO (CD1) DEL TEMA 2.4-C

public class Disparar : MonoBehaviour{
    public GameObject prefabBala;
    float tiempoUltimoDisparo = 0;
    float tiempoEntreDisparos = 0.2f;

    void FixedUpdate(){
        
        if (Time.time > (tiempoUltimoDisparo + tiempoEntreDisparos)) {
            GameObject x = Instantiate(prefabBala, transform.position, transform.rotation);
            Destroy(x, 3f);                                                   //Destruir la bala en 1 segundo            
            tiempoUltimoDisparo = Time.time;
}}}
