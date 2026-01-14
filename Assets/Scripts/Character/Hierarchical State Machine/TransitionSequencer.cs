using System.Collections.Generic;
using UnityEngine;

namespace HSM
{
    public class TransitionSequencer
    {
        public readonly StateMachine Machine;

        public TransitionSequencer(StateMachine machine)
        {
            Machine = machine;
        }
        
        // Request a transition from one state to another
        public void RequestTransition(State from, State to)
        {
            Machine.ChangeState(from, to);
        }

        // Compute the Lowest Common Ancestor of two states
        public static State Lca(State a, State b)
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
