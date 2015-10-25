using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bazam.Extensions;
using Melek.Domain;

namespace MtGBar.Views.CardViews
{
    public partial class TransformCardView : UserControl
    {
        public TransformCardView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
        
        public TransformCard Card
        {
            get { return (TransformCard)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }
        
        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "Card", 
            typeof(TransformCard), 
            typeof(TransformCardView), 
            new PropertyMetadata(null)
        );
        
        public TransformPrinting Printing
        {
            get { return (TransformPrinting)GetValue(PrintingProperty); }
            set { SetValue(PrintingProperty, value); }
        }
        
        public static readonly DependencyProperty PrintingProperty = DependencyProperty.Register(
            "Printing", 
            typeof(TransformPrinting), 
            typeof(TransformCardView), 
            new PropertyMetadata(null)
        );
        
        public bool IsTransformed
        {
            get { return (bool)GetValue(IsTransformedProperty); }
            set { SetValue(IsTransformedProperty, value); }
        }
        
        public static readonly DependencyProperty IsTransformedProperty = DependencyProperty.Register(
            "IsTransformed", 
            typeof(bool), 
            typeof(TransformCardView), 
            new PropertyMetadata(false)
        );
        
        public BitmapImage NormalImage
        {
            get { return (BitmapImage)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }
        
        public static readonly DependencyProperty NormalImageProperty = DependencyProperty.Register(
            "NormalImage", 
            typeof(BitmapImage), 
            typeof(TransformCardView), 
            new PropertyMetadata(null)
        );

        public ICommand TransformCommand
        {
            get { return (ICommand)GetValue(TransformCommandProperty); }
            set { SetValue(TransformCommandProperty, value); }
        }

        public static readonly DependencyProperty TransformCommandProperty = DependencyProperty.Register(
            "TransformCommand",
            typeof(ICommand),
            typeof(TransformCardView),
            new PropertyMetadata(null)
        );

        public BitmapImage TransformedImage
        {
            get { return (BitmapImage)GetValue(TransformedImageProperty); }
            set { SetValue(TransformedImageProperty, value); }
        }

        public static readonly DependencyProperty TransformedImageProperty = DependencyProperty.Register(
            "TransformedImage", 
            typeof(BitmapImage), 
            typeof(TransformCardView), 
            new PropertyMetadata(null)
        );

        // TODO: should I refactor this into a converter to share between here and FlipCardView.xaml?
        public Func<object, object> TransformIEnumerableToString
        {
            get
            {
                return (object arg) => {
                    IEnumerable<string> typedArg = (arg as IEnumerable<string>);
                    if (typedArg != null) return typedArg.Concatenate<string>(" ");

                    return string.Empty;
                };
            }
        }

        public Func<object, object> TransformIEnumerableToCardTypes
        {
            get
            {
                return (object arg) => {
                    IEnumerable<CardType> typedArg = (arg as IEnumerable<CardType>);
                    if (typedArg != null) return typedArg.Concatenate<CardType>(" ").ToUpper();

                    return string.Empty;
                };
            }
        }
    }
}