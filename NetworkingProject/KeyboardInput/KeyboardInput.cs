using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;
namespace KeyboardInput
{
    public class KeyboardInput<T> where T : Control
    {
        private T control;
        private KeyOrder keyOrder;

        public bool Paste = false;
        public bool Undo = false;
        public bool Redo = false;

        public Dictionary<Keys,bool> keys;
        public Keys lastPressedKey = Keys.None;

        public delegate void UpdatedFunction(KeyboardInput<T> keyboardInput, bool KeyDown);
        public UpdatedFunction Updated;

        public KeyboardInput(T control, UpdatedFunction Updated)
        {
            this.control = control;
            this.Updated = Updated;
            this.keyOrder = new KeyOrder(this);
            keys = new Dictionary<Keys, bool>();
            keys.Add(Keys.V, false);
            keys.Add(Keys.Z, false);
            keys.Add(Keys.R, false);
            control.KeyDown += Control_KeyDown;
            control.KeyUp += Control_KeyUp;
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            update(e,false);
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            update(e,true);
        }

        private void update(KeyEventArgs e, bool down)
        {
            e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = false;
                return;
            }
            if (down)
            {
                lastPressedKey = e.KeyCode;
                keyOrder.Push(e.KeyCode);
            }
            else if (!down && lastPressedKey == e.KeyCode)
            {
                lastPressedKey = Keys.None;
            }

            if (!keys.ContainsKey(e.KeyCode)) { keys.Add(e.KeyCode, false); }
            keys[e.KeyCode] = down;
            Paste = Undo = Redo = false;
            if(e.Modifiers.HasFlag(Keys.Control) || e.Modifiers.HasFlag(Keys.Shift) || e.Modifiers.HasFlag(Keys.Alt))
            {
                if (e.Modifiers.HasFlag(Keys.Control))
                {
                    if (isKeyDown(Keys.V)) { Paste = true; }
                    if (isKeyDown(Keys.Z)) { Undo = true; }
                    if (isKeyDown(Keys.R)) { Redo = true; }
                }
            }
            Updated(this,down);
        }

        public bool isKeyDown(Keys key)
        {
            if (!keys.ContainsKey(key)) { keys.Add(key,false); }
            return keys[key];
        }

        
        public string getString()
        {
            if(Paste)
            {
                return "Paste:"+Clipboard.GetText();
            }
            else if(Undo)
            {
                return "Undo";
            }
            else if(Redo)
            {
                return "Redo";
            }
            return "key:"+lastPressedKey.ToString();
        }

        private class KeyOrder
        {
            Keys[] keysStored = new Keys[5];
            KeyboardInput.KeyboardInput<T> ki;

            public KeyOrder(KeyboardInput<T> ki)
            {
                this.ki = ki;
            }

            public void Push(Keys key)
            {
                if (keysStored[0] == key) { return; }
                for(int i = keysStored.Length-1; i > 0; i--)
                {
                    keysStored[i] = keysStored[i - 1];
                }
                keysStored[0] = key;
            }

            public bool KeysPressedInOrder(Keys[] keys)
            {
                for(int i = 0; i < keys.Length; i++)
                {
                    if(!ki.isKeyDown(keys[i]))
                    {
                        return false;
                    }
                }
                if (keys.Length > keysStored.Length) { return false; }
                int count = 0;
                for(int i = 0; i < keysStored.Length;i++)
                {
                    if (count == keys.Length) { return true; }
                    if (keys[i] == keys[count])
                    {
                        count++;
                        continue;
                    }
                    count = 0;
                }
                return false;
                /*string a = GetKeysString(keysStored);
                string b = GetKeysString(keys);
                return GetKeysString(keysStored).Contains(GetKeysString(keys));*/
            }

            public string GetKeysString(Keys[] keys)
            {
                string result = "";
                for (int i = 0; i < keys.Length; i++)
                {
                    result += keys[i].ToString();
                }
                return result;
            }
        }
    }
}
