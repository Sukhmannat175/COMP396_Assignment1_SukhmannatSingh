using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StateMachine 
{
    public class State
    {
        public string Name;

        public System.Action onEnter;
        public System.Action onExit;
        public System.Action onStay;

        public override string ToString()
        {
            return Name;
        }
    }

    public Dictionary<string, State> states = new Dictionary<string, State>();

    public State currentState { get; private set; }

    public State initialState;
    public State previousState { get; private set; }

    //Factory Pattern - CreateState method
    public State CreateState(string name)
    {
        State newState= new State();
        newState.Name = name;

        if(states.Count == 0)
        {
            initialState = newState;
        }

        states[name] = newState;

        return newState;
    }

    public void Update()
    {
        if(states.Count == 0 || initialState == null)
        {
            Debug.LogErrorFormat("*** StateMachine has no states or initalState is null");
            return;
        }

        if(currentState == null)
        {
            ChangeState(initialState);
        }

        if(currentState.onStay != null)
        {
            currentState.onStay();
        }
    }

    public void ChangeState(State newState)
    {

        if (newState == null)
        {
            Debug.LogErrorFormat("*** Can not change to a null state!");
            return;
        }

        //S1.onExit
        if(currentState != null && currentState.onExit != null) { 
            currentState.onExit();
        }


        //S1 => S2
        Debug.LogFormat("Changing from {0} state to {1} state", currentState,newState);
        previousState = currentState;
        currentState = newState;

        //S2.onEnter
        if(newState.onEnter != null)
        {
            newState.onEnter();
        }


    }

    public void ChangeState(string newStateName)
    {
        if(!states.ContainsKey(newStateName))
        {
            Debug.LogErrorFormat($"The state machine does not contain the state {newStateName}");
            return;

        }
        ChangeState(states[newStateName]);
    }
}
