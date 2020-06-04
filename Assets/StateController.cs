using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateController : MonoBehaviour
{
    public enum PlayerState { CHASING, RETURNING, SHOOT, WAITING }

    public PlayerState state;

    private PlayerMovement pMovement;
    private StephenCurry shootController;
    private bool waitingCoroutine;

    [SerializeField]
    private Text currentSpeedMultiplier, shotPercentage;

    // Start is called before the first frame update
    void Start()
    {
        state = PlayerState.WAITING;
        pMovement = GetComponent<PlayerMovement>();
        shootController = GetComponent<StephenCurry>();
        waitingCoroutine = false;
    }

    private void FixedUpdate()
    {
        if(!shootController.training && !shootController.testing)
        StateBehaviour();
    }

    public void speedUpdate(float value)
    {
        currentSpeedMultiplier.text = $"Current speed: {value}x";
        Time.timeScale = value;
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
                if (Vector3.Distance(transform.position, pMovement.positionToGo) < 2)
                {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
        pMovement.positionToGo = new Vector3(px, 0, pz);
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
        UpdatePct();
        if (state != PlayerState.CHASING) state = PlayerState.WAITING;
        waitingCoroutine = false;
    }

    private void UpdatePct()
    {
        shotPercentage.text = $"{shootController.shotsMade}/{shootController.shotsTried} - " +
            $"{Math.Round(shootController.shotsMade / shootController.shotsTried, 2)*100}%";
    }
}
