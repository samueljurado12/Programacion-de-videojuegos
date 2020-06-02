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
        if(path.corners.Length > 0)
        {
            var currentPosition = transform.localPosition;
            var positionToMove = path.corners[pathIndex];
            var directionVector = new Vector3(positionToMove.x - currentPosition.x, positionToMove.y - currentPosition.y, positionToMove.z - currentPosition.z);

            if((Vector3.Distance(transform.localPosition, positionToMove) >= 0.5 || Rigidbody.velocity.sqrMagnitude < 3) && Time.time > 0.1 + impulseLastTime)
            {
                impulseLastTime = Time.time;
                Rigidbody.AddForce(directionVector * Thrust, ForceMode.Impulse);
            } 

            if (Vector3.Distance(transform.localPosition, positionToMove) < 1 && Time.time > 0.5 + lastTime)
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
        pathIndex = 0;
        navmesh.CalculatePath(originalPosition, path);
    }

    public void OnDrawGizmos()
    {
        if(path.corners.Length > 0)
        {
            Gizmos.DrawLine(transform.position, path.corners[pathIndex]);
        }
    }
}
