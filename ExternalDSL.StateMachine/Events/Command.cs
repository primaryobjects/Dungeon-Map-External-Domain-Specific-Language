using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExternalDSL.StateMachineLib.Events
{
    public class Command : AbstractEvent
    {
        public Command()
        {
        }

        public Command(string name, string eventCode)
        {
            Name = name;
            Code = eventCode;
        }
    }
}
