using UnityEngine;

public class BallLogic : MonoBehaviour
{

    private int notLikeThatSongOfDaftPunk; // one more time
    private GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        notLikeThatSongOfDaftPunk = 0;
    }

    void Update()
    {
        if(playerObject != null)
        {
            transform.position = playerObject.transform.position + new Vector3(0, 1.5f, 1);
        }
    }

    public void PickedUpByPlayer(GameObject gameObject)
    {
        playerObject = gameObject;
    }

    public void BallHasBeenThrown()
    {
        playerObject = null;
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
