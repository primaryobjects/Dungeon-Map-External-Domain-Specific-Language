using System;
using System.Collections.Generic;
using System.Reflection;
using ExternalDSL.StateMachineLib.States;
using System.Text.RegularExpressions;

namespace ExternalDSL.DSLParser
{
    internal static class Utility
    {
        #region Constants

        private const string _stringMarker = "\"";
        private const string _stringSpacerEncoder = "»space»";

        #endregion

        #region Structs

        /// <summary>
        /// Symbol table used for resolving unknown states.
        /// </summary>
        public struct SymbolTableForState
        {
            public State CurrentState;
            public object Transition;
            public string PropertyName;
            public Dictionary<string, State> Dictionary;
            public string Token;

            public SymbolTableForState(State currentState, object transition, string propertyName, Dictionary<string, State> dictionary, string token)
            {
                CurrentState = currentState;
                Transition = transition;
                PropertyName = propertyName;
                Dictionary = dictionary;
                Token = token;
            }
        };

        #endregion

        /// <summary>
        /// Any variables found for state that did not yet exist (were located further down in the code file) will be resolved here.
        /// </summary>
        public static void ResolveReferences(List<Utility.SymbolTableForState> symbolTableForStateList)
        {
            // Go through each item added to the lookup state list to resolve.
            for (int i = 0; i < symbolTableForStateList.Count; i++)
            {
                // Get the type and field for the target property name to assign.
                Type type = symbolTableForStateList[i].Transition.GetType();
                FieldInfo field = type.GetField(symbolTableForStateList[i].PropertyName);

                // Get the state and value for the transition.
                object obj = symbolTableForStateList[i].Transition;
                object value = symbolTableForStateList[i].Dictionary[symbolTableForStateList[i].Token];

                // Assign the state to the transition.
                field.SetValue(obj, value);

                // Add the transition to the parent state.
                Transition transition = (Transition)obj;
                symbolTableForStateList[i].CurrentState.AddTransition(transition.Trigger, transition.Target);
            }
        }

        /// <summary>
        /// Pre-processes DSL code text to filter out special characters for processing.
        /// Currently, filters quotes in order to read strings as tokens.
        /// </summary>
        /// <param name="content">string</param>
        /// <returns>string</returns>
        public static string PreProcess(string content)
        {
            // Filter string values and replace spaces with _ so they form as a single token.
            Regex regEx = new Regex("" + _stringMarker + ".+" + _stringMarker);
            MatchCollection matches = regEx.Matches(content);
            foreach (Match match in matches)
            {
                string value = match.Value.Replace(" ", _stringSpacerEncoder);
                content = content.Replace(match.Value, value);
            }

            content = content.Replace("\"", "");

            return content;
        }

        /// <summary>
        /// Post-processes DSL code to remove any encoding performed by PreProcess.
        /// Currently, replaces spacing-character where they were removed from strings in PreProcess.
        /// </summary>
        /// <param name="content">string</param>
        /// <returns>string</returns>
        public static string PostProcess(string content)
        {
            // Re-process string values (with underscores as spaces) to re-form as strings.
            content = content.Replace(_stringSpacerEncoder, " ");
            return content;
        }
    }
}
