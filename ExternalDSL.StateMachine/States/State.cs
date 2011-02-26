using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExternalDSL.StateMachineLib.Events;

namespace ExternalDSL.StateMachineLib.States
{
    public class State
    {
        public string Name;
        private List<Command> _actions = new List<Command>();
        private Dictionary<string, Transition> _transitions = new Dictionary<string, Transition>();

        public State(string name)
        {
            Name = name;
        }

        public void AddTransition(Event anEvent, State state)
        {
            if (state != null)
            {
                _transitions.Add(anEvent.Code, new Transition(this, anEvent, state));
            }
        }

        public void AddAction(Command action)
        {
            _actions.Add(action);
        }

        public List<State> GetTargets()
        {
            List<State> result = new List<State>();

            foreach (KeyValuePair<string, Transition> item in _transitions)
            {
                result.Add(item.Value.Target);
            }

            return result;
        }

        public List<string> GetEventCodes()
        {
            List<string> result = new List<string>();

            foreach (KeyValuePair<string, Transition> item in _transitions)
            {
                result.Add(item.Value.EventCode);
            }

            return result;
        }

        public bool HasTransition(string eventCode)
        {
            return _transitions.ContainsKey(eventCode);
        }

        public State GetTargetState(string eventCode)
        {
            return _transitions[eventCode].Target;
        }

        public void Execute()
        {
            Console.WriteLine("Executing " + Name);

            foreach (Command command in _actions)
            {
                // Execute command
                Console.WriteLine(command.Name + " (" + command.Code + ")");
            }
        }
    }
}
