using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HSM
{
    public class StateMachine
    {
        public readonly State Root;
        private HashSet<State> states;
        private bool started;

        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event EventHandler<StateEventArgs> StateEntered;
        public event EventHandler<StateEventArgs> StateExited;
        
        public StateMachine(State root, HashSet<State> states)
        {
            Root = root;
            this.states = states;
        }

        public void Start()
        {
            if (started) return;
            started = true;
            Root.Enter();
        }

        public void Tick(float deltaTime)
        {
            if (!started) Start();
            InternalTick(deltaTime);
        }

        public void FixedTick(float fixedDeltaTime)
        {
            if (!started) Start();
            InternalFixedTick(fixedDeltaTime);
        }
        
        internal void InternalTick(float deltaTime) => Root.Update(deltaTime);
        internal void InternalFixedTick(float fixedDeltaTime) => Root.FixedUpdate(fixedDeltaTime);

        public T GetState<T>() where T : State
        {
            return states.OfType<T>().FirstOrDefault();
        }
        
        // Perform the actual switch from 'from' to 'to' by exiting up to the shared ancestor, then entering down to the target
        public void ChangeState(State from, State to)
        {
            if (from == to || from == null || to == null) return;

            State lca = Lca(from, to);
            
            // Exit current branch up to (but not including) LCA
            for (State s = from; s != lca; s = s.Parent)
            {
                s.Exit();
                StateExited?.Invoke(this, new StateEventArgs(s));
            }
            
            // Enter target branch from LCA down to target
            var stack = new Stack<State>();
            for (State s = to; s != lca; s = s.Parent) stack.Push(s);
            while (stack.Count > 0)
            {
                var state = stack.Pop();
                state.Enter();
                StateEntered?.Invoke(this, new StateEventArgs(state));
            }
            
            StateChanged?.Invoke(this, new StateChangedEventArgs(from, to));
        }
        
        // Compute the Lowest Common Ancestor of two states
        private static State Lca(State a, State b)
        {
            // Create a set of all parents of 'a'
            var ap = new HashSet<State>();
            for (var s = a; s != null; s = s.Parent) ap.Add(s);
            
            // Find the first parent of 'b' that is also a parent of 'a'
            for (var s = b; s != null; s = s.Parent)
                if (ap.Contains(s)) return s;
            
            // If no common ancestor is found, return null
            return null;
        }
    }
    
    public class StateEventArgs : EventArgs
    {
        public State State { get; }
        
        public StateEventArgs(State state)
        {
            State = state;
        }
    }
    
    public class StateChangedEventArgs : EventArgs
    {
        public State From { get; }
        public State To { get; }
        
        public StateChangedEventArgs(State from, State to)
        {
            From = from;
            To = to;
        }
    }
    
    public static class StateMachineExtensions
    {
        public static void OnStateEntered<T>(this StateMachine machine, Action<T> handler) 
            where T : State
        {
            machine.StateEntered += (sender, e) =>
            {
                if (e.State is T state)
                {
                    handler(state);
                }
            };
        }
        
        public static void OnStateExited<T>(this StateMachine machine, Action<T> handler) 
            where T : State
        {
            machine.StateExited += (sender, e) =>
            {
                if (e.State is T state)
                {
                    handler(state);
                }
            };
        }
        
        public static void OnStateChangedTo<T>(this StateMachine machine, Action<T> handler) 
            where T : State
        {
            machine.StateChanged += (sender, e) =>
            {
                if (e.To is T state)
                {
                    handler(state);
                }
            };
        }
        
        public static void OnStateChangedFrom<T>(this StateMachine machine, Action<T> handler) 
            where T : State
        {
            machine.StateChanged += (sender, e) =>
            {
                if (e.From is T state)
                {
                    handler(state);
                }
            };
        }
        
        public static void OnStateChangedFromTo<TFrom, TTo>(this StateMachine machine, Action<TFrom, TTo> handler) 
            where TFrom : State
            where TTo : State
        {
            machine.StateChanged += (sender, e) =>
            {
                if (e.From is TFrom fromState && e.To is TTo toState)
                {
                    handler(fromState, toState);
                }
            };
        }
    }
}