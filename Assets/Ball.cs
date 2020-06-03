using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public PruebaLanzamiento prueba;

    public void NotifyController()
    {
        prueba.hasScored = true;
    }
    
}
