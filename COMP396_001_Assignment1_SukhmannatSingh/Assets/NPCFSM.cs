using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class NPCFSM : MonoBehaviour
{
    public enum NPCState {
        Idle,
        Walking,
        Interacting
    }
    public NPCState currentState = NPCState.Idle;
    public NPCState previousState;

    public GameObject player;
    public GameObject QuestMenu;
    public float maxSpeed;

    public Transform[] waypoints;
    public int nextWaypointIndex = 1;

    private bool questAccepted = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FSM();
        StartCoroutine(IdleTime());
    }

    IEnumerator IdleTime()
    {
        if (currentState == NPCState.Idle)
        {
            yield return new WaitForSeconds(3);
            ChangeState(NPCState.Walking);
        }
    }
    
    IEnumerator ResetQuest()
    {
        if (questAccepted)
        {
            yield return new WaitForSecondsRealtime(3);
            questAccepted = false;
        } 
    }

    private void FSM()
    {
        switch (currentState)
        {
            case NPCState.Idle:
                HandleIdle();
                break;
            case NPCState.Walking:
                HandleWalking();
                break;
            case NPCState.Interacting:
                HandleInteracting();
                break;
            default:
                print("current State is invalid" + currentState);
                break;

        }
    }

    private void ChangeState(NPCState newState)
    {
        previousState = currentState;
        currentState = newState;
    }

    private void HandleIdle()
    {
        //Rotates in place
        if (Vector3.Distance(this.transform.position, waypoints[0].position) < float.Epsilon)
        {
            transform.Rotate(0, 30 * Time.deltaTime, 0);
        }

        if (Vector3.Distance(this.transform.position, player.transform.position) < 1 && !questAccepted)
        {
            ChangeState(NPCState.Interacting);
        }
    }

    private void HandleWalking()
    {
        DoWalking();

        if (WaypointReached())
        {
            if (nextWaypointIndex == 1)
            {
                nextWaypointIndex = 0;
            }
            else if (nextWaypointIndex == 0)
            {
                nextWaypointIndex = 1;
                ChangeState(NPCState.Idle);
            }
        }

        if (Vector3.Distance(this.transform.position, player.transform.position) < 1 && !questAccepted)
        {
            ChangeState(NPCState.Interacting);
        }
    }

    private void HandleInteracting()
    {
        DoInteracting();
    }

    private void DoWalking()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, waypoints[nextWaypointIndex].position, maxSpeed * Time.deltaTime);
    }


    private bool WaypointReached()
    {        
        if(Vector3.Distance(this.transform.position, waypoints[nextWaypointIndex].position) < float.Epsilon)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DoInteracting()
    {
        QuestMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void AcceptQuest()
    {
        Time.timeScale = 1;
        questAccepted = true;
        StartCoroutine(ResetQuest());
        ChangeState(previousState);
        QuestMenu.SetActive(false);
    }
}
