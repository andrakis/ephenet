using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephenet.Interface
{
    /// <summary>
    /// Provides a way to "guard" access against requiring constant use of
    /// the InvokeRequired, Invoke and BeginInvoke methods.
    /// </summary>
    public abstract class InvokeGuard
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();
        object variables_lock = new Object();

        public InvokeGuard() { }

        protected delegate void Callback();
        protected void lockVariables(Callback callback)
        {
            lock (variables_lock) { callback(); }
        }

        public void Declare(string name, object value)
        {
            lockVariables(() => variables[name] = value);
        }

        public void Update(string name, object value)
        {
            lockVariables(() => variables[name] = value);
        }


    }
}
