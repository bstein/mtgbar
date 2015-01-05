using System;
using System.Text;
using System.Windows.Forms;

namespace MtGBar.ViewModels
{
    public class DisplayViewModel
    {
        public Screen Display { get; private set; }
        public string FriendlyName { get; private set; }
        public int? Index { get; private set; }

        public DisplayViewModel(int? index) : this(null, index)
        {
        }

        public DisplayViewModel(Screen screen, int? index)
        {
            this.Display = screen;
            this.Index = index;

            if (screen != null && index == null) throw new InvalidOperationException("If you're creating a DisplayViewModel with a Screen object, supply its index. Noob.");
            else if (index != null) {
                StringBuilder friendlyName = new StringBuilder("Display ");
                friendlyName.Append((index.Value + 1).ToString());
                friendlyName.Append(" (");

                // come up with a cool name for the location of the screen
                string locationString = string.Empty;
                string horizontalityString = string.Empty;
                string verticalityString = string.Empty;

                if (screen.Bounds.Location.X == 0 && screen.Bounds.Location.Y == 0) {
                    locationString = "center";
                }
                else {
                    if (screen.Bounds.Location.X < 0) {
                        horizontalityString = "left";
                    }
                    else if (screen.Bounds.Location.X > 0) {
                        horizontalityString = "right";
                    }
                    
                    if (screen.Bounds.Location.Y < 0) {
                        verticalityString = "above";
                    }
                    else if (screen.Bounds.Location.Y > 0) {
                        verticalityString = "below";
                    }
                }

                if (string.IsNullOrEmpty(locationString)) {
                    if (!string.IsNullOrEmpty(horizontalityString) && !string.IsNullOrEmpty(verticalityString)) {
                        locationString = horizontalityString + " & " + verticalityString;
                    }
                    else if (!string.IsNullOrEmpty(horizontalityString)) {
                        locationString = horizontalityString;
                    }
                    else if (!string.IsNullOrEmpty(verticalityString)) {
                        locationString = verticalityString;
                    }
                }
                

                if (locationString != string.Empty) {
                    friendlyName.Append(locationString + ", ");
                }

                friendlyName.Append(screen.Bounds.Width.ToString());
                friendlyName.Append("x");
                friendlyName.Append(screen.Bounds.Height.ToString());
                friendlyName.Append(")");

                this.FriendlyName = friendlyName.ToString();
            }
            else if (screen == null) {
                this.FriendlyName = "[your primary monitor]";
            }
        }
    }
}