﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Realm.Command.Interfaces;
using Realm.Entity;
using Realm.Entity.Entities;
using Realm.Library.Common;

namespace Realm.Command.Parsers
{
    /// <summary>
    ///
    /// </summary>
    public class CommandParser : Parser
    {
        private IVariableHelper Helper { get; set; }

        private readonly Dictionary<string, Func<string, IGameEntity, string>> _functionMap = new Dictionary
            <string, Func<string, IGameEntity, string>>()
            {
                {"timestamp", GetTimestamp},
                {"datestamp", GetDatestamp},
                {"property", GetProperty}
            };

        /// <summary>
        ///
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="commandManager"> </param>
        public CommandParser(IVariableHelper helper, ICommandManager commandManager)
            : base(commandManager)
        {
            Helper = helper;
        }

        /// <summary>
        /// Populates the command argument dictionary with the given data
        /// </summary>
        public IDictionary<string, object> PopulateCommandArgs(IBiota oActor, IEntity obj1,
            IEntity obj2, string aKeyword, bool aMessage = false)
        {
            return new Dictionary<string, object>
                {
                    {"actor", oActor},
                    {"object1", obj1},
                    {"object2", obj2},
                    {"keyword", aKeyword},
                    {"message", aMessage}
                };
        }

        /// <summary>
        /// Takes incoming data and populates the ReportData object
        /// </summary>
        public static ReportData ToReportData(IEntity oActor = null, IEntity oVictim = null,
            object oDirectObject = null, object oIndirectObject = null, object oSpace = null, object oExtra = null)
        {
            return new ReportData
                {
                    Actor = oActor,
                    Victim = oVictim,
                    DirectObject = oDirectObject,
                    IndirectObject = oIndirectObject,
                    Extra = oExtra,
                    Space = oSpace
                };
        }

        /// <summary>
        /// Parses incoming data for the given variable
        /// </summary>
        public string ParseVariable(string var, ReportData data)
        {
            Validation.IsNotNull(data, "data");

            string returnVal = string.Empty;

            Delegate func = Helper.GetDelegate(var);
            if (func.IsNotNull())
                returnVal = (string)func.DynamicInvoke(data);

            return returnVal;
        }

        /// <summary>
        /// Replaces any occurrences of a #string# entry with the property value
        /// </summary>
        public string ParseRegex(string str, IGameEntity entity = null)
        {
            var newText = str;

            const string pattern = "(?<=\\#)[A-Z_a-z]+(?=\\#)";
            while (Regex.IsMatch(newText, pattern, RegexOptions.IgnoreCase))
            {
                var match = Regex.Match(newText, pattern, RegexOptions.IgnoreCase);
                var foundString = newText.Substring(match.Index, match.Length);

                Func<string, IGameEntity, string> func = GetFunction(foundString);
                newText = newText.Replace("#" + foundString + "#", func.Invoke(foundString, entity));
            }
            return newText;
        }

        /// <summary>
        /// Retrieves the matching function out of the table or returns the default
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private Func<string, IGameEntity, string> GetFunction(string function)
        {
            Func<string, IGameEntity, string> func;
            try
            {
                func = _functionMap[function];
                if (func.IsNull())
                    func = _functionMap["property"];
            }
            catch
            {
                func = _functionMap["property"];
            }
            return func;
        }
        private static string GetTimestamp(string text, IGameEntity entity)
        {
            return DateTime.Now.TimeOfDay.ToString();
        }
        private static string GetDatestamp(string text, IGameEntity entity)
        {
            return DateTime.Now.ToString();
        }
        private static string GetProperty(string text, IGameEntity entity)
        {
            string returnVal = string.Empty;

            if (entity.IsNotNull())
            {
                var ctx = entity.GetContext("PropertyContext");
                if (ctx.IsNotNull())
                {
                    var obj = ctx.CastAs<PropertyContext>().GetProperty<object>(text);
                    if (obj.IsNotNull())
                        returnVal = obj.ToString();
                }
            }

            return returnVal;
        }
    }
}