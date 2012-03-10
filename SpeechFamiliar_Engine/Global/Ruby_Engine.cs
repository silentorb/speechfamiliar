using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Scripting.Hosting;
using IronRuby;
using System.Reflection;
//using System.Windows;
using SilentOrb.Utility;
using System.Threading;

namespace Ruby
{
    public class Ruby_Engine
    {
        ScriptRuntime runtime = null;
        ScriptEngine engine = null;
        ScriptScope scope;

        public Ruby_Engine()
        {
            runtime = IronRuby.Ruby.CreateRuntime();
            engine = runtime.GetEngine("Ruby");
            scope = runtime.CreateScope();

            execute(@"require 'mscorlib'
require 'System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL'");
            require_assembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

        public void add_variable(string name, object variable)
        {
            //runtime.Globals.SetVariable(name, variable);
            engine.SetVariable(scope, name, variable);
        }

        public object execute(string code)
        {
            try
            {
                var writer = new MemoryStream();
                runtime.IO.SetOutput(writer, Console.OutputEncoding);
                var action = engine.CreateScriptSourceFromString(code);
                object result = action.Execute(scope);
                Feedback.print(Encoding.UTF8.GetString(writer.GetBuffer(), 0, (int)writer.Length), Feedback.Status.result);
                return result;
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
                return null;
            }
        }

        public void execute_file(string file_name)
        {
            execute(File.ReadAllText(file_name));
        }

        public void require_assembly(Assembly assembly)
        {
            execute(string.Format(
@"require '{0}'
", assembly.FullName));
        }

        public void load_assembly(string file_name)
        {
            execute(string.Format(
@"load '{0}'
", file_name));
        }
        public void execute_thread(string code)
        {
            try
            {
                Thread thread = new Thread(delegate()
 {
     var writer = new MemoryStream();
     runtime.IO.SetOutput(writer, Console.OutputEncoding);

     var action = engine.CreateScriptSourceFromString(code);
     action.Execute(scope);
     Feedback.print(Encoding.UTF8.GetString(writer.GetBuffer(), 0, (int)writer.Length), Feedback.Status.result);

 });
                thread.Name = "test";
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }
        }
    }
}
