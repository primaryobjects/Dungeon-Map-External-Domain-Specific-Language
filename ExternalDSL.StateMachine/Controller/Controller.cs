using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExternalDSL.StateMachineLib.States;

namespace ExternalDSL.StateMachineLib.Controller
{
    public class Controller
    {
        private State _state;
        private StateMachine _stateMachine;
        public State CurrentState
        {
            get
            {
                return _state;
            }
        }

        public Controller(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _state = stateMachine.Start;
        }

        public bool Handle(string eventCode)
        {
            if (_state.HasTransition(eventCode))
            {
                TransitionTo(_state.GetTargetState(eventCode));

                return true;
            }
            else if (_stateMachine.IsResetEvent(eventCode))
            {
                TransitionTo(_stateMachine.Start);

                return true;
            }
            else
            {
                // Ignore unknown commands.
                return false;
            }
        }

        private void TransitionTo(State target)
        {
            _state = target;
            _state.Execute();
        }
    }
}
