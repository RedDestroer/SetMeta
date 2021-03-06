﻿using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class OptionSet
    {
        public OptionSet()
        {
            Options = new Dictionary<string, Option>();
            Groups = new Dictionary<string, Group>();
            Constants = new Dictionary<string, Constant>();
            Suggestions = new Dictionary<string, Suggestion>();
        }

        public string Version { get; set; }
        public IDictionary<string, Option> Options { get; }
        public IDictionary<string, Group> Groups { get; }
        public IDictionary<string, Constant> Constants { get; }
        public IDictionary<string, Suggestion> Suggestions { get; }
    }
}