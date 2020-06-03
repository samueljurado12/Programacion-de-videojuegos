using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public enum PlayerState { CHASING, RETURNING, SHOOT, WAITING }

    public PlayerState state;

    private PlayerMovement pMovement;
    private PruebaLanzamiento shootController;
    private bool waitingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        state = PlayerState.WAITING;
        pMovement = GetComponent<PlayerMovement>();
        shootController = GetComponent<PruebaLanzamiento>();
        waitingCoroutine = false;
    }

    private void FixedUpdate()
    {
        StateBehaviour();
    }

    private void StateBehaviour()
    {
        switch (state)
        {
            case StateController.PlayerState.CHASING:
                pMovement.Move();
                break;
            case StateController.PlayerState.RETURNING:
                pMovement.Move();
                if (Vector3.Distance(transform.position, pMovement.returnPosition) < 0.5)
                {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    switchShoot();
                }
                break;
            case StateController.PlayerState.SHOOT:
                if (!waitingCoroutine)
                {
                    StartCoroutine(Shoot());
                }
                break;
            case PlayerState.WAITING:
                break;
        }
    }

    internal void switchChasing(GameObject ball, Vector3 spawnPos)
    {
        pMovement.ChaseBall(Instantiate(ball, spawnPos, Quaternion.identity));
        state = PlayerState.CHASING;
    }

    internal void switchReturning()
    {
        float px = 0, pz = 0;
        shootController.GetRandomPosition(ref px, ref pz);
        pMovement.returnPosition = new Vector3(px, 0, pz);
        pMovement.SelectReturnPosition();
        state = PlayerState.RETURNING;
    }

    public void switchShoot()
    {
        state = PlayerState.SHOOT;
    }

    private IEnumerator Shoot()
    {
        waitingCoroutine = true;
        yield return shootController.CalculateForces();
        yield return shootController.Shoot();
        if (state != PlayerState.CHASING) state = PlayerState.WAITING;
        waitingCoroutine = false;
    }
}
