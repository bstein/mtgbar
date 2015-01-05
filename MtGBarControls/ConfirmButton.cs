using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MtGBarControls
{
    [TemplateVisualState(Name = "Confirming", GroupName = "ConfirmStates")]
    [TemplateVisualState(Name = "NotConfirming", GroupName = "ConfirmStates")]
    public class ConfirmButton : Control
    {
        static ConfirmButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ConfirmButton),
                new FrameworkPropertyMetadata(typeof(ConfirmButton))
            );
        }

        public ConfirmButton()
        {
            VisualStateManager.GoToState(this, "NotConfirming", false);
        }

        #region Dependency properties
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", 
            typeof(ICommand), 
            typeof(ConfirmButton), 
            new PropertyMetadata(null)
        );

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", 
            typeof(object), 
            typeof(ConfirmButton), 
            new PropertyMetadata(null)
        );

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", 
            typeof(object), 
            typeof(ConfirmButton), 
            new PropertyMetadata(null)
        );
        #endregion
    }
}