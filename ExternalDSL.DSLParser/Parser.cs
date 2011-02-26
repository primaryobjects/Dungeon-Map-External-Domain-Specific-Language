using System.Collections.Generic;
using System.IO;
using ExternalDSL.StateMachineLib.States;
using ExternalDSL.StateMachineLib.Events;
using ExternalDSL.StateMachineLib.Controller;

namespace ExternalDSL.DSLParser
{
    public class Parser
    {
        #region Constants

        private const string _commandsToken = "commands";
        private const string _eventsToken = "events";
        private const string _stateToken = "state";
        private const string _actionsToken = "action";
        private const string _transitionsToken = "transitions";
        private const string _resetToken = "end";

        private const string _startCode = "startCode";
        private const string _endCode = "endCode";

        #endregion

        private Dictionary<string, Command> _commandList = new Dictionary<string, Command>();
        private Dictionary<string, Event> _eventList = new Dictionary<string, Event>();
        private Dictionary<string, State> _stateList = new Dictionary<string, State>();

        private List<Utility.SymbolTableForState> _symbolTableForStateList = new List<Utility.SymbolTableForState>();
        private StateMachine _stateMachine;
        private Controller _controller;

        public Parser()
        {
            // Setup a state machine with an internal DSL to handle parsing our code file.
            // You could actually create an external DSL code file to program the Parser itself, but for this example, we'll just utilize the code with an internal DSL.

            #region Setup Parser State Machine

            // States
            State idle = new State("idle");
            State waitingForCommand = new State("waitingForCommand");
            State waitingForCommandCode = new State("waitingForCommandCode");
            State waitingForEvent = new State("waitingForEvent");
            State waitingForEventCode = new State("waitingForEventCode");
            State waitingForState = new State("waitingForState");
            State waitingForAction = new State("waitingForAction");
            State waitingForTransition = new State("waitingForTransition");
            State waitingForTransitionState = new State("waitingForTransitionState");

            // Events
            Event readCommandStart = new Event("readCommandStart", _commandsToken);
            Event readCommandCodeStart = new Event("readCommandCodeStart", _startCode);
            Event readCommandCodeEnd = new Event("readCommandCodeEnd", _endCode);

            Event readEventStart = new Event("readEventStart", _eventsToken);
            Event readEventCodeStart = new Event("readEventCodeStart", _startCode);
            Event readEventCodeEnd = new Event("readEventCodeEnd", _endCode);

            Event readActionStart = new Event("readActionStart", _actionsToken);
            Event readActionEnd = new Event("readActionEnd", _endCode);

            Event readStateStart = new Event("readStateStart", _stateToken);

            Event readTransitionStart = new Event("readTransitionEvent", _transitionsToken);
            Event readTransitionState = new Event("readTransitionState", "=>");
            Event readTransitionStateEnd = new Event("readTransitionStateEnd", _endCode);
            Event endTransition = new Event("endTransition", ".");

            Event resetEvent = new Event("resetEvent", _resetToken);

            // Transitions
            idle.AddTransition(readCommandStart, waitingForCommand);
            idle.AddTransition(readEventStart, waitingForEvent);
            idle.AddTransition(readStateStart, waitingForState);

            waitingForCommand.AddTransition(readCommandCodeStart, waitingForCommandCode);
            waitingForCommandCode.AddTransition(readCommandCodeEnd, waitingForCommand);

            waitingForEvent.AddTransition(readEventCodeStart, waitingForEventCode);
            waitingForEventCode.AddTransition(readEventCodeEnd, waitingForEvent);

            waitingForState.AddTransition(readActionStart, waitingForAction);
            waitingForState.AddTransition(readTransitionStart, waitingForTransition);

            waitingForAction.AddTransition(readActionEnd, waitingForState);

            waitingForTransition.AddTransition(readTransitionState, waitingForTransitionState);
            waitingForTransition.AddTransition(endTransition, waitingForState);
            waitingForTransitionState.AddTransition(readTransitionStateEnd, waitingForTransition);
            
            // Setup state machine.
            _stateMachine = new StateMachine(idle);

            _stateMachine.AddResetEvent(resetEvent);

            // Setup controller.
            _controller = new Controller(_stateMachine);

            #endregion
        }

        /// <summary>
        /// Creates a state machine from a DSL code file.
        /// </summary>
        /// <param name="fileName">Filename to load</param>
        /// <returns>StateMachine</returns>
        public StateMachine GetStateMachine(string fileName)
        {
            string content = File.ReadAllText(fileName);

            return ParseDSL(content);
        }

        /// <summary>
        /// Main DSL code file parser. Uses the Parser state machine to read the code file and process the program.
        /// </summary>
        /// <param name="content">text containing DSL program code</param>
        /// <returns>StateMachine</returns>
        private StateMachine ParseDSL(string content)
        {
            StateMachine stateMachine = null;
            State currentState = null;
            Command command = null;
            Event anEvent = null;
            State state = null;
            Transition transition = null;

            // Encode strings before parsing.
            content = Utility.PreProcess(content);

            string separators = " \r\n\t";
            string[] tokens = content.Split(separators.ToCharArray());

            foreach (string token in tokens)
            {
                // Decode strings from token.
                string tokenValue = Utility.PostProcess(token);

                string tokenLwr = tokenValue.ToLower();

                // Pass the token to our state machine to handle.
                bool handled = _controller.Handle(tokenLwr);

                if (!handled && tokenLwr.Length > 0)
                {
                    // Process the token under our current state.
                    switch (_controller.CurrentState.Name)
                    {
                        case "waitingForCommand":
                        {
                            // Read a Command Name.
                            command = new Command();
                            command.Name = tokenValue;

                            // Move state to read Command Code.
                            _controller.Handle(_startCode);

                            break;
                        }
                        case "waitingForCommandCode":
                        {
                            // Read a Command Code.
                            command.Code = tokenValue;

                            _commandList.Add(command.Name, command);

                            // Move state back to read Command Name.
                            _controller.Handle(_endCode);

                            break;
                        }
                        case "waitingForEvent":
                        {
                            // Read an Event Name.
                            anEvent = new Event();
                            anEvent.Name = tokenValue;

                            // Move state to read an Event Code.
                            _controller.Handle(_startCode);

                            break;
                        }
                        case "waitingForEventCode":
                        {
                            // Read an Event Code.
                            anEvent.Code = tokenValue;

                            _eventList.Add(anEvent.Name, anEvent);

                            // Move state back to read an Event Name.
                            _controller.Handle(_endCode);

                            break;
                        }
                        case "waitingForState":
                        {
                            // Read a State Name, stay in this state until next command read.
                            state = new State(tokenValue);
                            currentState = state;

                            _stateList.Add(tokenValue, state);

                            break;
                        }
                        case "waitingForAction":
                        {
                            // Read an Action Name.
                            state.AddAction(_commandList[tokenValue]);

                            // Move state back to reading a State.
                            _controller.Handle(_endCode);

                            break;
                        }
                        case "waitingForTransition":
                        {
                            // Read a Transition Trigger, stay in this state until symbol read for transition Target.
                            transition = new Transition();
                            transition.Source = currentState;
                            transition.Trigger = _eventList[tokenValue];

                            break;
                        }
                        case "waitingForTransitionState":
                        {
                            // Read a Transition Target, if the target state is known, assign it now. Otherwise, store it in the symbol table for mapping later.
                            if (_stateList.ContainsKey(tokenValue))
                            {
                                transition.Target = _stateList[tokenValue];
                                currentState.AddTransition(transition.Trigger, transition.Target);
                            }
                            else
                            {
                                // Add reference to bind later.
                                _symbolTableForStateList.Add(new Utility.SymbolTableForState(currentState, transition, "Target", _stateList, tokenValue));
                            }

                            // Move state back to reading a Transition.
                            _controller.Handle(_endCode);

                            break;
                        }
                    }
                }
            }

            // Resolve any references to unknown states.
            Utility.ResolveReferences(_symbolTableForStateList);

            // Create the state machine with the starting state.
            stateMachine = new StateMachine(_stateList["idle"]);

            return stateMachine;
        }
    }
}
