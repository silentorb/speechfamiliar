using System;
using System.Collections.Generic;
using System.Text;
using SpeechFamiliar.Scripting;
using SpeechFamiliar;
//using System.Speech.Recognition;
using System.Windows.Forms;
using SilentOrb.Utility;
using SilentOrb.Reflection;
using System.Threading;
using Text_Editing;

namespace SpeechFamiliar.Plugins
{
    [PluginInfo("text_editing")]
    public class Text_Editing : Plugin
    {
        enum Capitalization_Mode
        {
            normal = 0,
            next,
            all,
            auto,
            none
        }

        Capitalization_Mode mode = Capitalization_Mode.normal;

        //public Familiar_Word_List last_result
        //{
        //    get { return Familiar_Word.history[Familiar_Word.history.Count - 1]; }
        //}

        //public override string Name
        //{
        //    get { return "Text_Editing"; }
        //}

        //public Text_Editing()
        //{
        //    Familiar_Word.Process_Results += new Familiar_Word.SpeechProcess(Process_Results);

        //}

        public void dictation(Sentence sentence)
        {
            Feedback.print(sentence.words[0].source.text, Feedback.Status.info);
        }

        public Sentence number(Sentence sentence)
        {
            if (sentence.words.Count < 2)
                return sentence;

            //Feedback.print("cool", Feedback.Status.info);
            int result = 0;

            foreach (var word in sentence.words)
            {
                result += int.Parse(word.text);
            }

            var main_word = sentence.words[0];
            sentence.words.Clear();
            sentence.words.Add(main_word);
            main_word.text = result.ToString();

            return sentence;
        }

        //void Process_Results(object sender, Familiar_Word word)
        //{
        //    //if (word.confidence < 0.45)
        //    //    return;
        //    //string output = result.Text.Replace("$empty$", "");

        //    //string output = "";
        //    ////foreach (Familiar_Word word in result.words)
        //    ////{
        //    //    if (word.confidence > 0.45)
        //    //        //output += word.format();
        //    ////}

        //    string output = word.format();

        //    if (output != "")
        //    {
        //        if (mode == Capitalization_Mode.next)
        //        {
        //            output = output.Substring(0, 1).ToUpper() + output.Substring(1);
        //            mode = Capitalization_Mode.normal;
        //        }
        //        else if (mode == Capitalization_Mode.all)
        //        {
        //            output = output.ToUpper();
        //            mode = Capitalization_Mode.normal;
        //        }
        //        else if (mode == Capitalization_Mode.none)
        //        {
        //            output = output.ToLower();
        //            mode = Capitalization_Mode.normal;
        //        }
        //    }

        //    Automation.sendkeys(output);

        //    //string info = "> " + output + " [" + result.grammar_name + "]\r\n";
        //    //Feedback.print(info);

        //    if (word.properties[Word_Options.property.capitalize_next_word])
        //        mode = Capitalization_Mode.next;

        //    //Familiar_Word.history.Add(result);
        //}

        public void capitalize_next()
        {
            mode = Capitalization_Mode.next;
        }

        public void capitalize_all()
        {
            mode = Capitalization_Mode.all;
        }

        public void capitalize_none()
        {
            mode = Capitalization_Mode.none;
        }

        public void open_text_editor()
        {
            Thread thread = new Thread(delegate()
{
    Text_Editor editor = new Text_Editor();
    editor.ShowDialog();
});
            thread.Name = "text_editor";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void move_cursor(Sentence result)
        {
            int amount = 1;
            if (result.parameters.contains_key("amount"))
                amount = int.Parse(result.parameter["amount"].text);

            string command = String.Format("{{{0} {1}}}", result.parameter["direction"].text.ToUpper(), amount);

            if (result.parameters.contains_key("unit") && result.parameter["unit"].text.StartsWith("word"))
                command = "^" + command;

            Automation.sendkeys(command);
        }

        public void delete_text(Sentence result)
        {
            select_text(result, "{DELETE}");
        }

        public void copy_text(Sentence result)
        {
            select_text(result, "^c");
        }

        public void select_text(Sentence result)
        {
            select_text(result, "");
        }

        public void cut_text(Sentence result)
        {
            select_text(result, "^x");
        }

        void select_text(Sentence result, string suffix)
        {
            string command = "+";
            int amount = 1;
            bool success = int.TryParse(result.parameter["amount"].text, out amount);
            if (!success)
                amount = 1;

            Dictionary<string, string> horizontal = new Dictionary<string, string>();
            horizontal.Add("last", "LEFT");
            horizontal.Add("next", "RIGHT");

            Dictionary<string, string> vertical = new Dictionary<string, string>();
            vertical.Add("last", "UP");
            vertical.Add("next", "DOWN");
            string direction = result.parameter["direction"].text;

            //Dictionary<string, string> mode = null;

            switch (result.parameter["unit"].text)
            {
                case "word":
                    command = "+^{" + horizontal[result.parameter["direction"].text] + "}" + suffix;
                    break;
                case "words":
                    command = "+^{" + horizontal[result.parameter["direction"].text] + " "
                             + amount.ToString() + "}" + suffix;
                    break;

                case "characters":
                case "character":
                    command = "+{" + horizontal[result.parameter["direction"].text] + " "
                + amount.ToString() + "}" + suffix;
                    break;

                case "lines":
                    //  amount -= 1;
                    if (vertical[direction] == "UP")
                        command = "{HOME}{HOME}+"
                            + "{" + vertical[direction] + " " + amount.ToString() + "}" + suffix;
                    else
                    {
                        --amount;
                        command = "{HOME}{HOME}{DOWN}+"
                    + "{" + vertical[direction] + " " + amount.ToString() + "}+{END}" + suffix;
                    }

                    break;

                default:
                    Feedback.print("error parsing delete word command.", Feedback.Status.error);
                    return;
            }

            Feedback.print(command, Feedback.Status.result);
            Automation.sendkeys(command);
        }

        public void go_to_line(Sentence result)
        {
            Automation.sendkeys("^g");
            Automation.sendkeys(result.parameter["line"].text);
            Automation.sendkeys("{ENTER}");
        }
    }
}
