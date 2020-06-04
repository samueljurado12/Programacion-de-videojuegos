using UnityEngine;

public class BallLogic : MonoBehaviour
{

    public StephenCurry prueba;
    private int notLikeThatSongOfDaftPunk; // one more time
    private StateController character;

    // Start is called before the first frame update
    void Start()
    {
        notLikeThatSongOfDaftPunk = 0;
    }

    public void Thrown()
    {
        transform.parent = null;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player") && notLikeThatSongOfDaftPunk == 0)
        {
            gameObject.layer = LayerMask.NameToLayer("Pelota");
            notLikeThatSongOfDaftPunk++;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.parent = other.gameObject.GetComponent<PlayerMovement>().ballHolder;
            other.gameObject.GetComponent<StephenCurry>().holdedBall = this;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            character = other.gameObject.GetComponent<StateController>();
            character.switchReturning();
        }
    }

    private void OnDestroy()
    {
        if(character.state == StateController.PlayerState.WAITING)
        {
            FindObjectOfType<BallManager>().SpawnNewBall();
        }
    }


    public void NotifyController()
    {
        prueba.hasScored = true;
    }
}
