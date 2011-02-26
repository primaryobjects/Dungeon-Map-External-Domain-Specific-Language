using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExternalDSL.StateMachineLib.Events
{
    public abstract class AbstractEvent
    {
        public string Name;
        public string Code;

        public AbstractEvent()
        {
        }

        public AbstractEvent(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
