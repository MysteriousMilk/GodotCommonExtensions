using System;
using System.Collections.Generic;

namespace Godot.Extensions
{
    public partial class StateMachine : Node
    {
        private Node parent = null;
        private Dictionary<string, StateData> states;
        private StateData currentState;
        private StateMachineProcessMode stateProcessMode;
        private bool debugState = false;

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

        public StateMachine()
        {
            states = new Dictionary<string, StateData>();
        }

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

        public void ChangeState(string state)
        {
            CallDeferred(nameof(SetState), state);
        }

        public string GetCurrentState()
        {
            return currentState != null ? currentState.Name : null;
        }

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

        public enum StateMachineProcessMode
        {
            Process,
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
}
