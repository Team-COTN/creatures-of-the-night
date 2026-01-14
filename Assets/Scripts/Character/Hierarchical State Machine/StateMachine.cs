using System.Collections.Generic;
using UnityEngine;

namespace HSM
{
    public class StateMachine
    {
        public readonly State Root;
        public readonly TransitionSequencer Sequencer;
        private bool started;

        public StateMachine(State root)
        {
            Root = root;
            Sequencer = new TransitionSequencer(this);
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
        
        // Perform the actual switch from 'from' to 'to' by exiting up to the shared ancestor, then entering down to the target
        public void ChangeState(State from, State to)
        {
            if (from == to || from == null || to == null) return;

            State lca = TransitionSequencer.Lca(from, to);
            
            // Exit current branch up to (but not including) LCA
            for (State s = from; s != lca; s = s.Parent) s.Exit();
            
            // Enter target branch from LCA down to target
            var stack = new Stack<State>();
            for (State s = to; s != lca; s = s.Parent) stack.Push(s);
            while (stack.Count > 0) stack.Pop().Enter();
        }
    }
}