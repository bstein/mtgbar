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
    public partial class FlipCardView : UserControl
    {
        public FlipCardView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public FlipCard Card
        {
            get { return (FlipCard)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "Card",
            typeof(FlipCard),
            typeof(FlipCardView),
            new PropertyMetadata(null)
        );
        
        public BitmapImage CardImage
        {
            get { return (BitmapImage)GetValue(CardImageProperty); }
            set { SetValue(CardImageProperty, value); }
        }
        
        public static readonly DependencyProperty CardImageProperty = DependencyProperty.Register(
            "CardImage", 
            typeof(BitmapImage), 
            typeof(FlipCardView), 
            new PropertyMetadata(null)
        );

        public ICommand FlipCommand
        {
            get { return (ICommand)GetValue(FlipCommandProperty); }
            set { SetValue(FlipCommandProperty, value); }
        }

        public static readonly DependencyProperty FlipCommandProperty = DependencyProperty.Register(
            "FlipCommand",
            typeof(ICommand),
            typeof(FlipCardView),
            new PropertyMetadata(null)
        );

        public bool IsFlipped
        {
            get { return (bool)GetValue(IsFlippedProperty); }
            set { SetValue(IsFlippedProperty, value); }
        }

        public static readonly DependencyProperty IsFlippedProperty = DependencyProperty.Register(
            "IsFlipped",
            typeof(bool),
            typeof(FlipCardView),
            new PropertyMetadata(false)
        );

        public FlipPrinting Printing
        {
            get { return (FlipPrinting)GetValue(PrintingProperty); }
            set { SetValue(PrintingProperty, value); }
        }
        
        public static readonly DependencyProperty PrintingProperty = DependencyProperty.Register(
            "Printing", 
            typeof(FlipPrinting), 
            typeof(FlipCardView), 
            new PropertyMetadata(null)
        );

        public Func<object, object> TransformIEnumerableToString
        {
            get
            {
                return (object arg) => {
                    IEnumerable<string> typedArg = (arg as IEnumerable<string>);
                    if(typedArg != null) return typedArg.Concatenate<string>(" ");

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

        public Func<object, object> TransformIEnumerableToVisibility
        {
            get
            {
                return (object arg) => {
                    IList<string> typedArg = (arg as IList<string>);
                    return typedArg != null && typedArg.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                };
            }
        }
    }
}