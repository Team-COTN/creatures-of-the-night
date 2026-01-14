using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HSM
{
    public class StateMachineBuilder
    {
        readonly State root;

        public StateMachineBuilder(State root)
        {
            this.root = root;
        }

        public StateMachine Build()
        {
            var m = new StateMachine(root);
            Wire(root, m, new HashSet<State>());
            return m;
        }

        void Wire(State s, StateMachine m, HashSet<State> visited)
        {
            if (s == null) return;
            if (!visited.Add(s)) return;
            
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var machineField = typeof(State).GetField("Machine", flags);
            if (machineField != null) machineField.SetValue(s, m);
            
            foreach(var fld in s.GetType().GetFields(flags))
            {
                if (!typeof(State).IsAssignableFrom(fld.FieldType)) continue;
                if (fld.Name == "Parent") continue;
                
                var child = (State)fld.GetValue(s);
                if (child == null) continue;
                if (!ReferenceEquals(child.Parent, s)) continue;
                
                Wire(child, m, visited);
            }
        }
    }
}
