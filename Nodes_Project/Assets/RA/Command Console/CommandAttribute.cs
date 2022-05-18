using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RA.CommandConsole
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class CommandAttribute : System.Attribute
    {
        private string id;
        private string description;
        private string format;

        public string Id { get { return id; } }
        public string Description { get { return description; } }
        public string Format { get { return format; } }

        public CommandAttribute(string id, string description, string format)
        {
            this.id = id;
            this.description = description;
            this.format = format;
        }
    }
}