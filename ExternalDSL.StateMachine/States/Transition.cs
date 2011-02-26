using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExternalDSL.StateMachineLib.Events;

namespace ExternalDSL.StateMachineLib.States
{
    public class Transition
    {
        public State Source;
        public State Target;
        public Event Trigger; 
        public string EventCode
        {
            get
            {
                return Trigger.Code;
            }
        }

        public Transition()
        {
        }

        public Transition(State source, Event trigger, State target)
        {
            Source = source;
            Trigger = trigger;
            Target = target;
        }
    }
}
