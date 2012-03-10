using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using SilentOrb.Utility;
using SilentOrb.Reflection;
using System.Xml;
using System.IO;

namespace SpeechFamiliar.UI
{
    public class Tree_List<T>
    {
        public T item;
        public List<Tree_List<T>> children = new List<Tree_List<T>>();

        public Tree_List(T new_item)
        {
            item = new_item;
        }

        public Tree_List<T> add(T child)
        {
            Tree_List<T> result = new Tree_List<T>(child);
            children.Add(result);
            return result;
        }

    }

    [PluginInfo("default")]
    public class Controller:Context
    {
        AutomationFocusChangedEventHandler focusHandler = null;
        AutomationElement lastTopLevelWindow;
        //     static internal string current_text = "";

        public Controller(string new_name, Speech_Engine parent_engine)
            : base(new_name, parent_engine)
        {
            //SubscribeToFocusChange();
            SpeechFamiliar.Scripting.Automation.Active_Window_Changed += new SpeechFamiliar.Scripting.Automation.Window_Event(Automation_Active_Window_Changed);
        }

        public Controller()
        {
            SpeechFamiliar.Scripting.Automation.Active_Window_Changed += new SpeechFamiliar.Scripting.Automation.Window_Event(Automation_Active_Window_Changed);
        }

        void Automation_Active_Window_Changed(Window_Info info)
        {
            return;
            AutomationElement element = AutomationElement.FromHandle(info.handle);

            //    Threading.start_new_thread(this, "update", new object[] {element});
            //update();
            //       get_text(element);
        }

        /// <summary>
        /// Create an event handler and register it.
        /// </summary>
        public void SubscribeToFocusChange()
        {
            focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }

        /// <summary>
        /// Handle the event.
        /// </summary>
        /// <param name="src">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            AutomationElement elementFocused = src as AutomationElement;
            //          get_text(elementFocused);
            //get_elements(root);

        }

        public override string get_pretext()
        {
            AutomationElement element = AutomationElement.FromHandle(SpeechFamiliar.Scripting.Automation.current_window.handle);
            return get_text(element);
        }

        protected string get_text(AutomationElement root)
        {

            AutomationElement active_element = AutomationElement.FocusedElement;
            TextPattern pattern;

            try
            {
                // AutomationPattern[] patterns = active_element.GetSupportedPatterns();
                pattern = active_element.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
            }
            catch
            {
                //Feedback.print("Error.");
                //string       if             current_text = DateTime.Now.ToShortTimeString() + " - none.";

                return null;
            }
             
            TextPatternRange range = pattern.DocumentRange;

            TextPatternRange[] selection = pattern.GetSelection();
            string temp = selection[0].GetText(20);
            selection[0].MoveEndpointByUnit(TextPatternRangeEndpoint.End, TextUnit.Character, -temp.Length);
            selection[0].MoveEndpointByUnit(TextPatternRangeEndpoint.Start, TextUnit.Character, -20);
            string result = selection[0].GetText(20);
            if (result == "")
                return null;

            //Feedback.print(amount + " - " + result + "\r\n");
            return result;

        }

        void get_elements(AutomationElement root)
        {
            AutomationElement topLevelWindow = GetTopLevelWindow(root);

            if (topLevelWindow == null)
            {
                return;
            }

            //Feedback.print("focus changed - " + topLevelWindow.Current.Name + "\r\n");

            try
            {
                Tree_List<AutomationElement> node = new Tree_List<AutomationElement>(topLevelWindow);
                WalkControlElements(topLevelWindow, node);
                log_nodes(topLevelWindow.Current.ClassName, node);
            }
            catch (Exception ex)
            {
                Feedback.print(ex);
            }
            Feedback.print("finished change - " + topLevelWindow.Current.Name + "\r\n", Feedback.Status.story);
            //     Feedback.print(topLevelWindow.Current.name+"\r\n");
            // If top-level window has changed, announce it.
            //if (topLevelWindow != lastTopLevelWindow)
            //{
            //    lastTopLevelWindow = topLevelWindow;

            //    Program.Main_Window.SetText(Program.Main_Window.text_active_window,
            //        topLevelWindow.Current.name);
            //}
            //else
            //{

            //    Program.Main_Window.SetText(Program.Main_Window.text_active_window,
            //        "Type: " +
            //        elementFocused.Current.LocalizedControlType+"  name: " + elementFocused.Current.name);

            //}


        }

        public void log_nodes(string name, Tree_List<AutomationElement> node)
        {
            string destination = Global.configuration.log_path + "/debug/" + name + ".xml";
            //if (File.Exists(destination))
            //    return;

            XmlDocument document = new XmlDocument();
            XmlElement root = save_node(node, document);
            document.AppendChild(root);

            XmlTextWriter text_writer = new XmlTextWriter(destination, Encoding.Unicode);
            text_writer.Formatting = Formatting.Indented;
            document.WriteTo(text_writer);
            text_writer.Close();

        }

        public XmlElement save_node(Tree_List<AutomationElement> node, XmlDocument document)
        {
            XmlElement element = document.CreateElement("node");
            element.SetAttribute("name", node.item.Current.Name);
            element.SetAttribute("type", node.item.Current.ControlType.LocalizedControlType);
            element.SetAttribute("class", node.item.Current.ClassName);
            element.SetAttribute("kb", node.item.Current.IsKeyboardFocusable.ToString());
            element.SetAttribute("key", node.item.Current.AccessKey);

            foreach (AutomationPattern pattern in node.item.GetSupportedPatterns())
            {
                XmlElement child = document.CreateElement("pattern");
                //child.SetAttribute("id", pattern.Id.ToString());
                child.SetAttribute("type", pattern.ProgrammaticName);
                element.AppendChild(child);
            }
            foreach (Tree_List<AutomationElement> child in node.children)
            {
                element.AppendChild(save_node(child, document));
            }

            return element;
        }


        /// <summary>
        /// Cancel subscription to the event.
        /// </summary>
        public void UnsubscribeFocusChange()
        {
            if (focusHandler != null)
            {
                Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
            }
        }

        private AutomationElement GetTopLevelWindow(AutomationElement element)
        {
            TreeWalker walker = TreeWalker.ContentViewWalker;
            AutomationElement elementParent;
            AutomationElement node = element;
            try  // In case the element disappears suddenly, as menu items are 
            // likely to do.
            {
                if (node == AutomationElement.RootElement)
                {
                    return node;
                }
                // Walk up the tree to the child of the root.
                while (true)
                {
                    elementParent = walker.GetParent(node);
                    if (elementParent == null)
                    {
                        return null;
                    }
                    if (elementParent == AutomationElement.RootElement)
                    {
                        break;
                    }
                    node = elementParent;
                }
            }
            catch (ElementNotAvailableException)
            {
                node = null;
            }
            catch (ArgumentNullException)
            {
                node = null;
            }
            return node;
        }

        /// <summary>
        /// Walks the UI Automation tree and adds the control type of each element it finds 
        /// in the control view to a TreeView.
        /// </summary>
        /// <param name="rootElement">The root of the search on this iteration.</param>
        /// <param name="treeNode">The node in the TreeView for this iteration.</param>
        /// <remarks>
        /// This is a recursive function that maps out the structure of the subtree beginning at the
        /// UI Automation element passed in as rootElement on the first call. This could be, for example,
        /// an application window.
        /// CAUTION: Do not pass in AutomationElement.RootElement. Attempting to map out the entire subtree of
        /// the desktop could take a very long time and even lead to a stack overflow.
        /// </remarks>
        private void WalkControlElements(AutomationElement rootElement, Tree_List<AutomationElement> treeNode)
        {
            // Conditions for the basic views of the subtree (content, control, and raw) 
            // are available as fields of TreeWalker, and one of these is used in the 
            // following code.
            AutomationElement elementNode = TreeWalker.ContentViewWalker.GetFirstChild(rootElement);

            while (elementNode != null)
            {
                //                if (elementNode.Current.ClassName == "AfxWnd80u")
                //                {
                //                    try
                //                    {
                //                        elementNode.SetFocus();
                //                        Feedback.print("*okay*");
                //                    }
                //                    catch
                //                    {
                //                    }
                ////Control control =                    Control.FromHandle((IntPtr)elementNode.Current.NativeWindowHandle);
                ////control.Select();
                //                 //   SilentOrb.Utility.Windows.set_foreground_window((IntPtr)elementNode.Current.NativeWindowHandle);
                //                }
                Tree_List<AutomationElement> childTreeNode = treeNode.add(elementNode);
                WalkControlElements(elementNode, childTreeNode);
                elementNode = TreeWalker.ContentViewWalker.GetNextSibling(elementNode);
            }
        }

    }
}
