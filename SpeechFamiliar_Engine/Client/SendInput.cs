using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SpeechFamiliar.UI
{
    static public class send_input
    {

        enum key : ushort
        {
            LBUTTON = 0x01,
            RBUTTON = 0x02,
            CANCEL = 0x03,
            MBUTTON = 0x04,    /* NOT contiguous with L & RBUTTON */

            BACKSPACE = 0x08,
            TAB = 0x09,

            CLEAR = 0x0C,
            RETURN = 0x0D,
            ENTER = 0x0D,

            SHIFT = 0x10,
            CONTROL = 0x11,
            MENU = 0x12,
            PAUSE = 0x13,
            CAPITAL = 0x14,

            ESCAPE = 0x1B,
            ESC = 0x1B,

            CONVERT = 0x1c,
            NOCONVERT = 0x1d,

            SPACE = 0x20,
            SPACEBAR = 0x20,
            PGUP = 0x21,
            PGDN = 0x22,
            END = 0x23,
            HOME = 0x24,
            LEFT = 0x25,
            UP = 0x26,
            RIGHT = 0x27,
            DOWN = 0x28,
            SELECT = 0x29,
            PRINT = 0x2A,
            EXECUTE = 0x2B,
            SNAPSHOT = 0x2C,
            INSERT = 0x2D,
            DELETE = 0x2E,
            HELP = 0x2F,

            /* VK_0 thru VK_9 are the same as ASCII '0' thru '9' (0x30 - 0x39) */
            /* VK_A thru VK_Z are the same as ASCII 'A' thru 'Z' (0x41 - 0x5A) */

            LWIN = 0x5B,
            RWIN = 0x5C,
            APPS = 0x5D,

            SLEEP = 0x5F,

            NUMPAD0 = 0x60,
            NUMPAD1 = 0x61,
            NUMPAD2 = 0x62,
            NUMPAD3 = 0x63,
            NUMPAD4 = 0x64,
            NUMPAD5 = 0x65,
            NUMPAD6 = 0x66,
            NUMPAD7 = 0x67,
            NUMPAD8 = 0x68,
            NUMPAD9 = 0x69,
            MULTIPLY = 0x6A,
            ADD = 0x6B,
            SEPARATOR = 0x6C,
            SUBTRACT = 0x6D,
            DECIMAL = 0x6E,
            DIVIDE = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,

            NUMLOCK = 0x90,
            SCROLL = 0x91,

            /*
             * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
             * Used only as parameters to GetAsyncKeyState() and GetKeyState().
             * No other API or message will distinguish left and right keys in this way.
             */
            LSHIFT = 0xA0,
            RSHIFT = 0xA1,
            LCONTROL = 0xA2,
            RCONTROL = 0xA3,
            LMENU = 0xA4,
            RMENU = 0xA5,

            EXTEND_BSLASH = 0xE2,
            OEM_102 = 0xE2,

            PROCESSKEY = 0xE5,

            ATTN = 0xF6,
            CRSEL = 0xF7,
            EXSEL = 0xF8,
            EREOF = 0xF9,
            PLAY = 0xFA,
            ZOOM = 0xFB,
            NONAME = 0xFC,
            PA1 = 0xFD,
            OEM_CLEAR = 0xFE,


            SEMICOLON = 0xBA,
            EQUAL = 0xBB,
            COMMA = 0xBC,
            HYPHEN = 0xBD,
            PERIOD = 0xBE,
            SLASH = 0xBF,
            BACKQUOTE = 0xC0,

            BROWSER_BACK = 0xA6,
            BROWSER_FORWARD = 0xA7,
            BROWSER_REFRESH = 0xA8,
            BROWSER_STOP = 0xA9,
            BROWSER_SEARCH = 0xAA,
            BROWSER_FAVORITES = 0xAB,
            BROWSER_HOME = 0xAC,
            VOLUME_MUTE = 0xAD,
            VOLUME_DOWN = 0xAE,
            VOLUME_UP = 0xAF,
            MEDIA_NEXT_TRACK = 0xB0,
            MEDIA_PREV_TRACK = 0xB1,
            MEDIA_STOP = 0xB2,
            MEDIA_PLAY_PAUSE = 0xB3,
            LAUNCH_MAIL = 0xB4,
            LAUNCH_MEDIA_SELECT = 0xB5,
            LAUNCH_APP1 = 0xB6,
            LAUNCH_APP2 = 0xB7,

            LBRACKET = 0xDB,
            BACKSLASH = 0xDC,
            RBRACKET = 0xDD,
            APOSTROPHE = 0xDE,
            OFF = 0xDF,



            DBE_ALPHANUMERIC = 0x0f0,
            DBE_SBCSCHAR = 0x0f3,
            DBE_DBCSCHAR = 0x0f4,
            DBE_ROMAN = 0x0f5,
            DBE_NOROMAN = 0x0f6,
            DBE_ENTERWORDREGISTERMODE = 0x0f7,
            DBE_ENTERIMECONFIGMODE = 0x0f8,
            DBE_FLUSHSTRING = 0x0f9,
            DBE_CODEINPUT = 0x0fa,
            DBE_NOCODEINPUT = 0x0fb,
            DBE_DETERMINESTRING = 0x0fc,
            DBE_ENTERDLGCONVERSIONMODE = 0x0fd
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int x;
            public int y;
            public uint mouse_data;
            public uint flags;
            public uint time;
            public IntPtr extra_info;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBOARD_INPUT
        {
            public ushort virtual_key;
            public ushort scan_code;
            public uint flags;
            public uint time;
            public IntPtr extra_info;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            uint uMsg;
            ushort wParamL;
            ushort wParamH;
        }

        //[StructLayout(LayoutKind.Explicit)]
        //struct INPUT
        //{
        //    [FieldOffset(0)]
        //    public int type;
        //    [FieldOffset(0)] //*
        //    public MOUSEINPUT mouse;
        //    [FieldOffset(0)] //*
        //    public KEYBOARD_INPUT keyboard;
        //    [FieldOffset(0)] //*
        //    public HARDWAREINPUT hardware;
        //}

        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdHardwareInputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mouse;

            [FieldOffset(0)]
            public KEYBOARD_INPUT keyboard;

            [FieldOffset(0)]
            public HARDWAREINPUT hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public MouseKeybdHardwareInputUnion mkhi;
        }


        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        //static void press(int scanCode)
        //{
        //    sendKey(scanCode, true);
        //}

        //static void release(int scanCode)
        //{
        //    sendKey(scanCode, false);
        //}

        //static void send_key(int scanCode, bool press)
        //{
        //    INPUT[] input = new INPUT[1];
        //    input[0] = new KEYBOARD_INPUT();
        //    input[0].type = INPUT_KEYBOARD;
        //    input[0].flags = KEY_SCANCODE;

        //    if ((scanCode & 0xFF00) == 0xE000)
        //    { // extended key? 
        //        input[0].flags |= KEY_EXTENDED;
        //    }

        //    if (press)
        //    { // press? 
        //        input[0].scanCode = (ushort)(scanCode & 0xFF);
        //    }
        //    else
        //    { // release? 
        //        input[0].scanCode = scanCode;
        //        input[0].flags |= KEY_UP;
        //    }

        //    uint result = SendInput(1, input, Marshal.SizeOf(input[0]));

        //    if (result != 1)
        //    {
        //        throw new Exception("Could not send key: " + scanCode);
        //    }
        //}

        const int INPUT_KEYBOARD = 1;
        const int KEY_EXTENDED = 0x0001;
        const uint KEY_DOWN = 0x0000;
        const uint KEY_UP = 0x0002;
        const uint KEY_SCANCODE = 0x0008;

        public static void sendkeys(string text)
        {
//     Profiler.trace("sendkeys");
            input_buffer buffer = new input_buffer();

            buffer.process(text);
            buffer.send();
            System.Threading.Thread.Sleep(10);
        }

        class key_definition
        {
            public ushort code;
            public bool shift;

            public key_definition(ushort new_code, bool new_shift)
            {
                code = new_code;
                shift = new_shift;
            }
        }

        class input_buffer
        {
            List<INPUT> items = new List<INPUT>();
            List<ushort> keys_down = new List<ushort>();
            Dictionary<string, key_definition> other_keys = new Dictionary<string, key_definition>();

            void define_key(string input, key output)
            {
                other_keys.Add(input, new key_definition((ushort)output, false));
            }

            void define_key(string input, key output, bool shift)
            {
                other_keys.Add(input, new key_definition((ushort)output, shift));
            }

            void define_key(string input, char output)
            {
                other_keys.Add(input, new key_definition((ushort)output, false));
            }

            void define_key(string input, char output, bool shift)
            {
                other_keys.Add(input, new key_definition((ushort)output, shift));
            }

            public input_buffer()
            {
                define_key(".", key.PERIOD);
                define_key(",", key.COMMA);
                define_key("/", key.SLASH);
                define_key(";", key.SEMICOLON);
                define_key("'", key.APOSTROPHE);
                define_key("[", key.LBRACKET);
                define_key("]", key.RBRACKET);
                define_key("\\", key.BACKSLASH);
                define_key("-", key.HYPHEN);
                define_key("=", key.EQUAL);
                define_key("`", key.BACKQUOTE);

                define_key("!", '1', true);
                define_key("@", '2', true);
                define_key("#", '3', true);
                define_key("$", '4', true);
                define_key("%", '5', true);
                define_key("^", '6', true);
                define_key("&", '7', true);
                define_key("*", '8', true);
                define_key("(", '9', true);
                define_key(")", '0', true);

                define_key("_", key.HYPHEN, true);
                define_key("+", key.EQUAL, true);
                define_key(":", key.SEMICOLON, true);
                define_key("{", key.LBRACKET, true);
                define_key("}", key.RBRACKET, true);
                define_key("\"", key.APOSTROPHE, true);
                define_key("<", key.COMMA, true);
                define_key(">", key.PERIOD, true);
                define_key("?", key.SLASH, true);
                define_key(" ", key.SPACEBAR, true);
                define_key("|", key.BACKSLASH, true);

            }
            public void process(string text)
            {
                //add((ushort)key.SHIFT, KEY_DOWN);
                //add((ushort)key.DOWN, KEY_DOWN | KEY_EXTENDED);
                //add((ushort)key.DOWN, KEY_UP | KEY_EXTENDED);
                //add((ushort)key.SHIFT, KEY_UP);

                //return;
                /*                object[] values = Enum.GetValues(typeof(key));
                                object[] names = Enum.GetValues(typeof(key));
                                foreach (object item in values)
                                {
                                    virtual_keys.Add(
                                }
                  */
                for (int x = 0; x < text.Length; ++x)
                {
                    string character = text.Substring(x, 1);
                    //buffer.add(key.UP);
                    if (character == "^")
                        hold(key.CONTROL);
                    else if (character == "%")
                        hold(key.MENU);
                    else if (character == "+")
                        hold(key.SHIFT);
                    else if (Regex.IsMatch(character, @"[A-Z]"))
                    {
                        hold(key.SHIFT);
                        add(character[0]);
                    }
                    else if (Regex.IsMatch(character, @"[a-z0-9]"))
                        add(character.ToUpper()[0]);
                    else if (character == "{")
                    {
                        if (x < text.Length - 1)
                        {
                            if (text[x + 1] == '{' || text[x + 1] == '}')
                            {
                                check_other_character(text[x + 1].ToString());
                                x += 2;
                                continue;
                            }
                        }

                        int y = text.IndexOf('}', x + 1) - 1;
                        string result = text.Substring(x + 1, y - x);
                        x = y + 1;

                        if (result == "WIN")
                            hold(key.LWIN);

                        string[] split = result.Split(new char[] { ' ' });
                        try
                        {
                            object value = Enum.Parse(typeof(key), split[0], true);

                            if (split.Length > 1)
                            {
                                for (int z = 0; z < int.Parse(split[1]); ++z)
                                {
                                    add((ushort)value, KEY_DOWN | KEY_EXTENDED);
                                    add((ushort)value, KEY_UP | KEY_EXTENDED);
                                }

                                clear_modifiers();
                            }
                            else
                            {
                                add((ushort)value, KEY_DOWN | KEY_EXTENDED);
                                add((ushort)value, KEY_UP | KEY_EXTENDED);
                            }

                            clear_modifiers();
                        }
                        catch
                        {
                            check_other_character(split[0]);
                        }

                    }
                    else
                    {
                        check_other_character(text[x].ToString());
                    }
                }
            }

            void check_other_character(string character)
            {
                if (other_keys.ContainsKey(character))
                {
                    if (other_keys[character].shift)
                        hold(key.SHIFT);

                    add(other_keys[character].code);
                }
            }

            public void hold(key key_code)
            {
                hold((ushort)key_code);
            }

            public void hold(ushort key_code)
            {
                keys_down.Add(key_code);
                add(key_code, KEY_DOWN);
            }

            public void send()
            {
                if (items.Count > 0)
                {
                    INPUT[] output = items.ToArray();
                    SendInput((uint)output.Length, output, Marshal.SizeOf(output[0]));
                }

            }

            public void add(key key_code)
            {
                add((ushort)key_code);
            }

            public void add(ushort key_code)
            {
                add(key_code, KEY_DOWN);
                add(key_code, KEY_UP);
                clear_modifiers();
            }

            void clear_modifiers()
            {
                foreach (ushort code in keys_down)
                {
                    add(code, KEY_UP);
                }

                keys_down.Clear();
            }

            void add(ushort key_code, uint flags)// bool start, bool extended)
            {
                INPUT action = new INPUT();
                action.type = INPUT_KEYBOARD;
                action.mkhi.keyboard = new KEYBOARD_INPUT();
                action.mkhi.keyboard.virtual_key = key_code;
                action.mkhi.keyboard.flags = flags;
                //if (!start)
                //    action.keyboard.flags = KEY_UP;
                //if (extended)
                //    action.keyboard.flags |= KEY_EXTENDED;
                items.Add(action);
            }


        }


    }

}
