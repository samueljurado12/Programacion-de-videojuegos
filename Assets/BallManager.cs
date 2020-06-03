using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField]
    private Vector3[] spawnPositions;
    [SerializeField]
    private StateController character;
    [SerializeField]
    private GameObject ball;


    public void SpawnNewBall()
    {
        Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
        character.switchChasing(ball, spawnPos);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPositions.Length > 0)
        {
            foreach (Vector3 vector in spawnPositions)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(vector, 0.5f);
            }
        }
    }
}
