using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExternalDSL.StateMachineLib.Events
{
    public class Event : AbstractEvent
    {
        public Event()
        {
        }

        public Event(string name, string eventCode)
        {
            Name = name;
            Code = eventCode;
        }
    }
}
