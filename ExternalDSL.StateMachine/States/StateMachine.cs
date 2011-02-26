using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExternalDSL.StateMachineLib.Events;

namespace ExternalDSL.StateMachineLib.States
{
    public class StateMachine
    {
        public State Start;
        private List<Event> _resetEvents = new List<Event>();

        public StateMachine(State start)
        {
            Start = start;
        }

        public List<State> GetStates(State start)
        {
            List<State> result = new List<State>();

            // Add the starting state.
            result.Add(start);

            // Add all reachable states.
            result.AddRange(start.GetTargets());

            return result;
        }

        public void AddResetEvent(Event anEvent)
        {
            _resetEvents.Add(anEvent);
        }

        public bool IsResetEvent(string eventCode)
        {
            foreach (Event anEvent in _resetEvents)
            {
                if (anEvent.Code == eventCode)
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> GetResetEventCodes()
        {
            List<string> result = new List<string>();

            foreach (Event anEvent in _resetEvents)
            {
                result.Add(anEvent.Code);
            }

            return result;
        }
    }
}
