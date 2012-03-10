using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaHub
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class UIAttribute : Attribute
    {
        readonly string _text;
        public int order = 100;

        //public int order
        //{
        //    get { return _order; }
        //    set { _order = value; }
        //}

        public string text
        {
            get
            {
                return this._text;
            }
        }

        public UIAttribute(string new_text)
        {
            _text = new_text;
        }

        //public UIAttribute(string new_text, int new_order)
        //{
        //    _text = new_text;
        //    _order = new_order;
        //}

    }
}
