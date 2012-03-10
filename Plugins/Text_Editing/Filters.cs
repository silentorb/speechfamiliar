using System;
using System.Collections.Generic;
using System.Text;
using SilentOrb.Reflection;
using SpeechFamiliar;

namespace Text_Editing
{
    //[PluginInfo("number2")]
    //public class Number_Filter : Familiar_Word.Filter
    //{
    //    public override void run()
    //    {
    //        if (sub_words.Count < 2)
    //            return;

    //        bool single_digits = true;
    //        foreach (Familiar_Word sub_word in sub_words)
    //        {
    //            if (!sub_word.is_number)
    //                return;

    //            if (sub_word.text.Length > 1)
    //                single_digits = false;
    //        }

    //        int value = 0;

    //        if (single_digits)
    //        {
    //            for (int x = 0; x < sub_words.Count; ++x)
    //            {
    //                value += Math.Max(1, sub_words[x].get_number())
    //                    * (int)Math.Pow(10, sub_words.Count - x);
    //            }
    //        }
    //        else
    //        {
    //            foreach (Familiar_Word sub_word in sub_words)
    //            {
    //                value += sub_word.get_number();
    //            }
    //        }

    //        text = value.ToString();
    //        sub_words.Clear();
    //        sub_words.Add(word);
    //    }
    //}
}
