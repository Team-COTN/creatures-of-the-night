using System.Collections.Generic;
using System.Linq;

namespace HSM
{
    public abstract class State
    {
        public readonly StateMachine Machine;
        public readonly State Parent;
        public State ActiveChild;

        public State(StateMachine machine, State parent = null)
        {
            Machine = machine;
            Parent = parent;
        }
        
        public virtual State GetDefaultChildState() => null;
        protected virtual (State state, string reason) GetNextState() => (state: null, reason: null);
        
        // Lifecycle hooks
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnFixedUpdate(float fixedDeltaTime) { }

        public bool IsActive => Machine.Root.Leaf().PathToRoot().Contains(this);

        internal void Enter()
        {
            if (Parent != null) Parent.ActiveChild = this;
            OnEnter();
            State child = GetDefaultChildState();
            if (child != null) child.Enter();
        }
        
        internal void Exit()
        {
            if (ActiveChild != null) ActiveChild.Exit();
            ActiveChild = null;
            OnExit();
        }
        
        internal void Update(float deltaTime)
        {
            if (TryTransition()) return;
            if (ActiveChild != null) ActiveChild.Update(deltaTime);
            OnUpdate(deltaTime);
        }

        internal void FixedUpdate(float fixedDeltaTime)
        {
            if (TryTransition()) return;
            if (ActiveChild != null) ActiveChild.FixedUpdate(fixedDeltaTime);
            OnFixedUpdate(fixedDeltaTime);
        }

        private bool TryTransition()
        {
            var result = GetNextState();
            if (result.state == null) return false;
            Machine.ChangeState(this, result.state, result.reason);
            return true;
        }

        public State Leaf()
        {
            State s = this;
            while (s.ActiveChild != null) s = s.ActiveChild;
            return s;
        }

        public IEnumerable<State> PathToRoot()
        {
            for (State s = this; s != null; s = s.Parent) yield return s;
        }
        
        public virtual void DrawCustomGizmos()
        {
            
        }
    }
}