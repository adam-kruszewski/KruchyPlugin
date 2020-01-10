﻿using System;
using System.ComponentModel.Design;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Kruchy.Plugin.Utils._2017
{
    public class DynamicItemMenuCommand : OleMenuCommand
    {
        private Predicate<int> matches;
        private DTE2 dte2;
        private int rootItemId = 0;

        private static int sequence = 10;
        private int uniqueID;

        public DynamicItemMenuCommand(
            CommandID rootId,
            Predicate<int> matches,
            EventHandler invokeHandler,
            EventHandler beforeQueryStatusHandler)
            : base(invokeHandler, null /*changeHandler*/, beforeQueryStatusHandler, rootId)
        {
            if (matches == null)
            {
                throw new ArgumentNullException("matches");
            }

            this.matches = matches;
            uniqueID = sequence++;
        }

        public override bool DynamicItemMatch(int cmdId)
        {
            // Call the supplied predicate to test whether the given cmdId is a match.
            // If it is, store the command id in MatchedCommandid
            // for use by any BeforeQueryStatus handlers, and then return that it is a match.
            // Otherwise clear any previously stored matched cmdId and return that it is not a match.
            if (this.matches(cmdId))
            {
                this.MatchedCommandId = cmdId;
                return true;
            }

            this.MatchedCommandId = 0;
            return false;
        }
    }
}