using System;
using System.Collections.Generic;

namespace Godot.Common.Nodes;

/// <summary>
/// A state machine that utilizes <see cref="Action"/> delegates for user logic/callbacks.
/// </summary>
public partial class DelegateStateMachine : Node
{
    private Node parent = null;
    private Dictionary<string, StateData> states;
    private StateData currentState;
    private StateMachineProcessMode stateProcessMode;
    private bool debugState = false;

    /// <summary>
    /// Indicates if the <see cref="DelegateStateMachine"/> is processed in _Process or 
    /// _PhysicsProcess.
    /// </summary>
    [Export]
    public StateMachineProcessMode StateProcessMode
    {
        get => stateProcessMode;
        set
        {
            if (value != stateProcessMode)
            {
                stateProcessMode = value;

                if (stateProcessMode == StateMachineProcessMode.Process)
                {
                    SetPhysicsProcess(false);
                    SetProcess(true);
                }
                else
                {
                    SetPhysicsProcess(true);
                    SetProcess(false);
                }
            }
        }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public DelegateStateMachine()
    {
        states = new Dictionary<string, StateData>();
    }

    /// <summary>
    /// Adds a new state to the <see cref="DelegateStateMachine"/>.
    /// </summary>
    /// <param name="name">The name/identifier of the state.</param>
    /// <param name="enterState">Delegate to invoke when this state is entered.</param>
    /// <param name="exitState">Delegate to invoke when exiting this state.</param>
    /// <param name="processLogic">Delegate to invoke during the process routine for the node.</param>
    public void AddState(string name, Action<string, string> enterState = null, Action<string, string> exitState = null, Action<double> processLogic = null)
    {
        states.Add(name, new StateData()
        {
            Name = name,
            EnterStateDelegate = enterState,
            ExitStateDelegate = exitState,
            ProcessLogicDelegate = processLogic
        });
    }

    /// <summary>
    /// Changes the <see cref="DelegateStateMachine"/> to a specific state. This is called deferred, on the next frame.
    /// </summary>
    /// <param name="state">The state to change to.</param>
    public void ChangeState(string state)
    {
        CallDeferred(nameof(SetState), state);
    }

    /// <summary>
    /// Returns the string name of the state that the <see cref="DelegateStateMachine"/> is currently in.
    /// </summary>
    /// <returns>Name of the current state.</returns>
    public string GetCurrentState()
    {
        return currentState != null ? currentState.Name : null;
    }

    /// <summary>
    /// Checks to see if the <see cref="DelegateStateMachine"/> is in a certain state.
    /// </summary>
    /// <param name="state">String name of the state to check.</param>
    /// <returns>True if the <see cref="DelegateStateMachine"/> is in the given state, False if not.</returns>
    public bool IsInState(string state)
    {
        if (state == null && currentState == null)
            return true;

        return currentState != null && currentState.Name == state;
    }

    internal void SetState(string newState)
    {
        string oldState = null;

        if (currentState != null)
        {
            oldState = currentState.Name;
            currentState.ExitStateDelegate?.Invoke(oldState, newState);
        }

        currentState = states[newState];

        if (debugState)
            GD.Print(currentState.Name);

        CallDeferred(nameof(InvokeEnterStateDelegate), newState, oldState);
    }

    private void InvokeEnterStateDelegate(string newState, string oldState)
    {
        if (currentState != null)
            currentState.EnterStateDelegate?.Invoke(newState, oldState);
    }

    public override void _Ready()
    {
        parent = GetParent<Node>();
    }

    public override void _Process(double delta)
    {
        if (StateProcessMode == StateMachineProcessMode.Physics)
            return;

        if (currentState != null)
            currentState.ProcessLogicDelegate?.Invoke(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (StateProcessMode == StateMachineProcessMode.Process)
            return;

        if (currentState != null)
            currentState.ProcessLogicDelegate?.Invoke(delta);
    }

    internal void SetDebugState(bool debug)
    {
        debugState = debug;
    }

    /// <summary>
    /// Types of process modes for the <see cref="DelegateStateMachine"/>.
    /// </summary>
    public enum StateMachineProcessMode
    {
        /// <summary>
        /// Process in the _PhysicProcess callback.
        /// </summary>
        Process,

        /// <summary>
        /// Process in the _Processs callback.
        /// </summary>
        Physics
    }

    internal class StateData
    {
        public string Name { get; internal set; }
        public Action<string, string> EnterStateDelegate;
        public Action<string, string> ExitStateDelegate;
        public Action<double> ProcessLogicDelegate;
    }
}
