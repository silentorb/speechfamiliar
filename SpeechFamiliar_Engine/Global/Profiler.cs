using System;
using System.Collections.Generic;
using System.Text;
using SilentOrb.Utility;

namespace SpeechFamiliar
{
    public static class Profiler
    {
        static DateTime clock;
        static string feedback;

        static public void initialize()
        {
            clock = DateTime.Now;
            feedback = "";
        }

        static public void trace(string text)
        {
            var duration = DateTime.Now - clock;
            //   Feedback.print(string.Format("PROFILER: {0}, {1}\r\n", duration.Milliseconds, text));

            lock (feedback)
            {
                feedback += string.Format("PROFILER: {0}, {1}\r\n", duration.Milliseconds, text);
            }
            clock = DateTime.Now;
        }

        static public void print_result()
        {
        //    Feedback.print(feedback);
        }

    }
}
