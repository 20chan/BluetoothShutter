using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RawInput_dll;

namespace RemoteController
{
    public partial class Form1 : Form
    {
        RawInput rawinput;
        KeyboardHook hook;
        bool isControllerPressing = false;
        bool shown = false;
        public Form1()
        {
            InitializeComponent();
            rawinput = new RawInput(Handle, false);
            rawinput.KeyPressed += Rawinput_KeyPressed;
            rawinput.AddMessageFilter();

            hook = new KeyboardHook();
            hook.KeyEvented += Hook_KeyEvented;
            hook.HookStart();

            this.FormClosed += (s, e) => hook.HookEnd();
        }

        ~Form1()
        {
            hook.HookEnd();
        }

        private void Rawinput_KeyPressed(object sender, RawInputEventArg e)
        {
            if (e.KeyPressEvent.Source == "Keyboard_01" && e.KeyPressEvent.VKeyName == "ENTER")
            {
                if (e.KeyPressEvent.KeyPressState == "MAKE")
                {
                    isControllerPressing = true;
                    if (shown)
                        BackColor = Color.Blue;
                    else
                        BackColor = Color.Purple;
                    shown = !shown;
                }
                else
                {
                    isControllerPressing = false;
                }
            }
        }

        bool hookOneStack = false;
        bool hookTwoStack = false;
        private bool Hook_KeyEvented(Keys arg1, KeyboardEventType arg2)
        {
            System.Diagnostics.Debug.WriteLine($"{arg1} {arg2}");
            if (arg1 == Keys.Return && arg2 == KeyboardEventType.KEYDOWN)
                hookOneStack = true;
            else if (hookOneStack && arg1 == Keys.VolumeUp && arg2 == KeyboardEventType.KEYDOWN)
            {
                hookTwoStack = true;
                return false;
            }
            else
                hookOneStack = hookTwoStack = false;

            if (isControllerPressing && (arg1 == Keys.VolumeDown || arg1 == Keys.VolumeUp))
                return false;
            return true;
        }
    }
}
