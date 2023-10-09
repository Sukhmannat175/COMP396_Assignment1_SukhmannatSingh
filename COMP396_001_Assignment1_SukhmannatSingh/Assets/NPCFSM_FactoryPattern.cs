using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPCFSM;

public class NPCFSM_FactoryPattern : MonoBehaviour
{
    public StateMachine stateMachine;

    public NPCState currentState = NPCState.Idle;
    public NPCState previousState;

    public GameObject player;
    public GameObject QuestMenu;
    public float maxSpeed;

    public Transform[] waypoints;
    public int nextWaypointIndex = 1;

    private bool questAccepted = false;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine();

        var idle = stateMachine.CreateState("Idle");
        idle.onEnter = delegate { Debug.Log("idle.onEnter"); };
        idle.onStay = delegate
        {
            Debug.Log("idle.onStay");

            HandleIdle();

            if (Vector3.Distance(this.transform.position, player.transform.position) < 1 && !questAccepted)
            {
                stateMachine.ChangeState("Interacting");
            }
        };
        idle.onExit = delegate { Debug.Log("idle.onEXit"); };

        var walking = stateMachine.CreateState("Walking");
        walking.onEnter = delegate { Debug.Log("walking.onEnter"); };
        walking.onStay = delegate
        {
            Debug.Log("walking.onStay");

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
                    stateMachine.ChangeState("Idle");
                }
            }

            if (Vector3.Distance(this.transform.position, player.transform.position) < 1 && !questAccepted)
            {
                stateMachine.ChangeState("Interacting");
            }
        };
        walking.onExit = delegate { Debug.Log("walking.onEXit"); };

        var interacting = stateMachine.CreateState("Interacting");
        interacting.onEnter = delegate { Debug.Log("interacting.onEnter"); };
        interacting.onStay = delegate
        {
            Debug.Log("interacting.onStay");

            DoInteracting();
        };
        interacting.onExit = delegate { Debug.Log("interacting.onEXit"); };
    }

    IEnumerator IdleTime()
    {
        yield return new WaitForSeconds(3);
        stateMachine.ChangeState("Walking");        
    }

    IEnumerator ResetQuest()
    {
        if (questAccepted)
        {
            yield return new WaitForSecondsRealtime(3);
            questAccepted = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private void HandleIdle()
    {
        //Rotates in place
        if (Vector3.Distance(this.transform.position, waypoints[0].position) < float.Epsilon)
        {
            transform.Rotate(0, 30 * Time.deltaTime, 0);
        }

        StartCoroutine(IdleTime());
    }

    private void DoWalking()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, waypoints[nextWaypointIndex].position, maxSpeed * Time.deltaTime);
    }


    private bool WaypointReached()
    {
        if (Vector3.Distance(this.transform.position, waypoints[nextWaypointIndex].position) < float.Epsilon)
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
        stateMachine.ChangeState(stateMachine.previousState);
        QuestMenu.SetActive(false);
    }
}