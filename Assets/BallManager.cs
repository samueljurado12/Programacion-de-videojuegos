using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField]
    private Vector3[] spawnPositions;
    [SerializeField]
    private PlayerMovement character;
    [SerializeField]
    private GameObject ball;

    public void SpawnNewBall()
    {
        Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
        character.ChaseBall(Instantiate(ball, spawnPos, Quaternion.identity));
    }

    private void OnDrawGizmosSelected()
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
