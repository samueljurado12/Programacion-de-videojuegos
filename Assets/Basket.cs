using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{

    [SerializeField]
    private BallManager ballManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("TRES PUNTOS COLEGA");
            Destroy(other.gameObject);
            ballManager.SpawnNewBall();
        }
    }
}
