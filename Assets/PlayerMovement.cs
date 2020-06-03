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
    private float time;
    private float lastTime;
    private float impulseLastTime;
    public Vector3 originalPosition;
    public Transform ballHolder;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        navmesh = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        pathIndex = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(path.corners.Length);
    }

    private void FixedUpdate()
    {
        if(path.corners.Length > 0)
        {
            var currentPosition = transform.position;
            var positionToMove = path.corners[pathIndex];
            var directionVector = new Vector3(positionToMove.x - currentPosition.x, positionToMove.y - currentPosition.y, positionToMove.z - currentPosition.z).normalized;
            transform.LookAt(positionToMove);

            if(Time.time > 0.1 + impulseLastTime)
            {
                Rigidbody.AddForce(directionVector * (Vector3.Distance(currentPosition, positionToMove) - 1) * Thrust, ForceMode.Force);
                impulseLastTime = Time.time;
            } 

            if (Vector3.Distance(transform.localPosition, positionToMove) < 2 && Time.time > 0.5 + lastTime)
            {
                lastTime = Time.time;
                pathIndex++;
            }
        }
    }

    public void ChaseBall(GameObject ball)
    {
        BallToChase = ball;
        navmesh.CalculatePath(ball.transform.position, path);
    }

    public void ReturnToOriginalPosition()
    {
        navmesh.CalculatePath(originalPosition, path);
        pathIndex = 0;
    }

    public void OnDrawGizmos()
    {
        if(path.corners.Length > 0)
        {
            Gizmos.DrawLine(transform.position, path.corners[pathIndex]);
        }
    }
}
