using System.Windows;
using System.Windows.Controls;

namespace MtGBarControls
{
    [TemplateVisualState(Name = "Has", GroupName = "ValueStates")]
    [TemplateVisualState(Name = "NotHas", GroupName = "ValueStates")]
    public class WatermarkTextBox : TextBox
    {
        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WatermarkTextBox), 
                new FrameworkPropertyMetadata(typeof(WatermarkTextBox))
            );
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateStates(false);
            this.TextChanged += WatermarkTextBox_TextChanged;
        }

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStates(true);
        }

        private void UpdateStates(bool transitions)
        {
            if (string.IsNullOrEmpty(this.Text)) {
                VisualStateManager.GoToState(this, "NotHas", transitions);
            }
            else {
                VisualStateManager.GoToState(this, "Has", transitions);
            }
        }

        public static DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
            "WatermarkText",
            typeof(string),
            typeof(WatermarkTextBox),
            new PropertyMetadata(null)
        );

        public string WatermarkText
        {
            get { return GetValue(WatermarkTextProperty).ToString(); }
            set { SetValue(WatermarkTextProperty, value); }
        }
    }
}