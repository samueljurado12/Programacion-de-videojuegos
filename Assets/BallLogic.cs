using UnityEngine;

public class BallLogic : MonoBehaviour
{
    private FixedJoint fixedJoint;
    private int notLikeThatSongOfDaftPunk; // one more time

    // Start is called before the first frame update
    void Start()
    {
        notLikeThatSongOfDaftPunk = 0;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player") && notLikeThatSongOfDaftPunk == 0)
        {
            notLikeThatSongOfDaftPunk++;
            other.gameObject.GetComponent<PlayerMovement>().ReturnToOriginalPosition();
        }
    }
}
