using System;
using System.Reflection;
using System.IO;
using ExternalDSL.StateMachineLib.States;
using ExternalDSL.StateMachineLib.Controller;
using ExternalDSL.DSLParser;

namespace ExternalDSL
{
    /// <summary>
    /// 
    /// An example of using an External Domain Specific Language to drive a state machine via a text file.
    /// This example allows the user to edit a text file with a custom programming language to create a simple state machine.
    /// 
    /// Created by Kory Becker
    /// http://www.primaryobjects.com/articledirectory.aspx
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string command = "";

            // Get application folder.
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Setup a state machine by reading the external domain specific language program.
            Parser parser = new Parser();
            StateMachine machine = parser.GetStateMachine(path + "\\dungeon.txt");
            Console.WriteLine("Loading of External DSL completed.\n");

            // Setup controller to run state machine.
            Controller controller = new Controller(machine);

            // Display some help.
            Console.WriteLine("> Q = Quit, ? = Available Commands");
            Console.WriteLine("> Entering the dungeon.");

            // Execute the starting state.
            controller.CurrentState.Execute();

            // Read commands from user to activate state machine.
            while (command.ToLower() != "q")
            {
                Console.Write("> ");

                // Read a command.
                command = Console.ReadLine();

                // Send the command to our state machine.
                HandleCommand(controller, command);
            }
        }

        #region Utility Methods

        /// <summary>
        /// Sends the command to the controller to handle.
        /// </summary>
        /// <param name="command">string</param>
        /// <param name="controller">state machine controller</param>
        private static void HandleCommand(Controller controller, string command)
        {
            if (!controller.Handle(command))
            {
                if (command == "?")
                {
                    // Display available commands for the given state.
                    foreach (string eventCode in controller.CurrentState.GetEventCodes())
                    {
                        Console.WriteLine(eventCode);
                    }
                }
                else if (command.ToLower() != "q")
                {
                    Console.WriteLine("Huh?");
                }
            }
        }

        #endregion
    }
}
