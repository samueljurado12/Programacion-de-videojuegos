using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField]
    private Vector3[] spawnPositions;
    [SerializeField]
    private Transform character;
    [SerializeField]
    private GameObject ball;

    public void SpawnNewBall()
    {
        Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
        GameObject newBall = Instantiate(ball, spawnPos, Quaternion.identity);
        //character.BallToChase = newBall;
    }

    private void OnDrawGizmos()
    {
        if(spawnPositions.Length > 0)
        {
            foreach (Vector3 vector in spawnPositions)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(vector, 0.5f);
            }
        }
    }
}
