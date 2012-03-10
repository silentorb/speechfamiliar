using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;
using IronRuby;

namespace Ruby
{
    public class Ruby_Engine
    {
        ScriptRuntime runtime = null;
        ScriptEngine engine = null;

        public Ruby_Engine()
        {
            runtime = IronRuby.Ruby.CreateRuntime();

            ScriptRuntimeSetup setup = new ScriptRuntimeSetup();
            //               setup.LanguageSetups.Add(new LanguageSetup("English", "English"));
            //               ScriptRuntime runtime = ScriptRuntime.Create();// (setup);
            //               ScriptEngine engine = runtime.GetEngine("Ruby");  
            //// TODO: should check that the values are identifiers  
            engine = runtime.GetEngine("Ruby");
            //string code = "GameServer.print 'wow'";
            //           string code = "File.open('c:/t.txt', 'w') {|f| f.write('wow')}";
            //run_time.LoadAssembly(System.Reflection.Assembly.GetCallingAssembly());
            //var scope=engine.CreateScope();
            //scope.

        }

        public void add_variable(string name,object variable)
        {
            runtime.Globals.SetVariable(name, variable);
        }
         
        public void execute(string code)
        {
            var action = engine.CreateScriptSourceFromString(code);
            action.Execute();
        }

        public void execute_file(string file_name)
        {
            var action = engine.CreateScriptSourceFromFile(file_name);
            action.Execute();
        }
    }
}
