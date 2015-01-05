using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Jammerware.Utilities;

namespace MtGBar.Infrastructure.UIHelpers.Behaviors
{
    public class EscapeCloseBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.KeyUp += (hearthstone, soSexy) => {
                if (soSexy.Key == Key.Escape) {
                    AnimationBuddy.Animate(AssociatedObject, "Opacity", 0, 200, (fucking, done) => {
                        AssociatedObject.Hide();
                    });
                }
            };

            AssociatedObject.Activated += (time, forThings) => {
                AnimationBuddy.Animate(AssociatedObject, "Opacity", 1, 200, (fucking, done) => {
                    if (AssociatedObject.IsLoaded) {
                        AssociatedObject.Show();
                    }
                });
            };
        }
    }
}
