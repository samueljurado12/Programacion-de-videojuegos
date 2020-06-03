using UnityEngine;

public class BallLogic : MonoBehaviour
{

    public PruebaLanzamiento prueba;
    private int notLikeThatSongOfDaftPunk; // one more time

    // Start is called before the first frame update
    void Start()
    {
        notLikeThatSongOfDaftPunk = 0;
    }

    void Update()
    {
    }

    private void Thrown()
    {
        transform.parent = null;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player") && notLikeThatSongOfDaftPunk == 0)
        {
            notLikeThatSongOfDaftPunk++;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.parent = other.gameObject.GetComponent<PlayerMovement>().ballHolder;
            transform.localPosition = Vector3.zero;
            other.gameObject.GetComponent<PlayerMovement>().ReturnToOriginalPosition();
        }
    }


    public void NotifyController()
    {
        prueba.hasScored = true;
    }
}
