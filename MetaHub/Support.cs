using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaHub
{
    public class Finish_EventArgs : EventArgs
    {
        public bool success = true;
        public string errorMessage = "";
    }

    public enum Operation_Status
    {
        Failure = 0,
        Success = 1,
        Running = 2,
        Waiting = 3
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InputAttribute : Attribute
    {
        public Type UIType = null;
        public Object[] Arguments = null;

        //public InputAttribute()
        //{
        //}

        //public PortInput(Type ui)
        //{
        //    UIType = ui;
        //}

        //public PortInput(Type ui, Object[] args)
        //{
        //    UIType = ui;
        //    Arguments = args;
        //}

    }
}
