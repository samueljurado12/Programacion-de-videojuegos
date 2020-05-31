using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{

    public GameObject BallToChase;

    private NavMeshAgent navmesh;
    private NavMeshPath path;
    private int pathIndex; 
    private Rigidbody Rigidbody;
    public float Thrust;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        navmesh = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        pathIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(path.corners.Length);
    }

    private void FixedUpdate()
    {
        var currentPosition = transform.position;
        var positionToMove = path.corners[pathIndex];
        var directionVector = new Vector3(currentPosition.x - positionToMove.x, currentPosition.y - positionToMove.y);

        Rigidbody.AddForce(directionVector * Thrust);

        if (currentPosition == positionToMove)
        {
            pathIndex++;
        }
    }

    public void ChaseBall(GameObject ball)
    {
        BallToChase = ball;
        navmesh.CalculatePath(ball.transform.position, path);
    }
}
