using System.Collections.Generic;
using System.Linq;

namespace HSM
{
    public class StateMachine
    {
        public readonly State Root;
        private HashSet<State> states;
        private bool started;

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
            for (State s = from; s != lca; s = s.Parent) s.Exit();
            
            // Enter target branch from LCA down to target
            var stack = new Stack<State>();
            for (State s = to; s != lca; s = s.Parent) stack.Push(s);
            while (stack.Count > 0) stack.Pop().Enter();
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
}