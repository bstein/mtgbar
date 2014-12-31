using System.Collections.Generic;
using System.Text;
using Bazam.KeyAdept.Infrastructure;
using Bazam.Modules;

namespace MtGBar.Infrastructure.DataNinjitsu.Models
{
    public class HotkeyDescription
    {
        public List<Modifier> Modifiers { get; set; }
        public Key Key { get; set; }

        private HotkeyDescription()
        {
            Modifiers = new List<Modifier>();
        }

        public HotkeyDescription(string hotkeyString) : this()
        {
            hotkeyString = hotkeyString.Replace(" ", string.Empty);
            if (hotkeyString.Contains("ALT")) Modifiers.Add(Modifier.Alt);
            if (hotkeyString.Contains("CTRL")) Modifiers.Add(Modifier.Ctrl);
            if (hotkeyString.Contains("SHIFT")) Modifiers.Add(Modifier.Shift);
            if (hotkeyString.Contains("WIN")) Modifiers.Add(Modifier.Win);

            int lastPlus = hotkeyString.LastIndexOf('+');
            Key = EnuMaster.Parse<Key>(hotkeyString.Substring(lastPlus + 1));
        }

        public HotkeyDescription(IEnumerable<Modifier> modifiers, Key key) : this()
        {
            Modifiers.AddRange(modifiers);
            Key = key;
        }

        public Hotkey ToHotkey()
        {
            return new Hotkey(Key, Modifiers.ToArray());
        }

        public override bool Equals(object obj)
        {
            bool retVal = false;

            if (obj != null && obj.GetType() == typeof(HotkeyDescription)) {
                HotkeyDescription other = (HotkeyDescription)obj;

                if (other.Modifiers.Count == this.Modifiers.Count) {
                    bool allKeysFound = true;
                    foreach (Modifier modifierKey in other.Modifiers) {
                        if (!this.Modifiers.Contains(modifierKey)) {
                            allKeysFound = false;
                        }
                    }

                    return allKeysFound;
                }
            }
            return retVal;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();

            foreach (Modifier mod in Modifiers) {
                string desc = string.Empty;
                if (mod == Modifier.Alt) {
                    desc = "ALT";
                }
                else if(mod == Modifier.Ctrl) {
                    desc = "CTRL";
                }
                else if(mod == Modifier.Shift) {
                    desc = "SHIFT";
                }
                else if(mod == Modifier.Win) {
                    desc = "WIN";
                }
                
                retVal.Append(desc + " + ");
            }

            retVal.Append(Key.ToString().ToUpper());
            return retVal.ToString();
        }
    }
}