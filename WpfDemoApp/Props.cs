using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
// ReSharper disable CheckNamespace

namespace Brushes
{
    public class BrushCollection : Collection<Brush>
    {
    }

    //[Bindable(BindableSupport.Yes)]
    public static class Props
    {
        
        public static readonly DependencyProperty ContentBrushProperty = DependencyProperty.RegisterAttached(
            "ContentBrush", typeof(Brush), typeof(Props), new PropertyMetadata(default(Brush), ContentBrushPropertyChangedCallback));

        private static void ContentBrushPropertyChangedCallback(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            var brushes = GetContentBrushes(dp) as BrushCollection;
            if (brushes == null)
            {
                brushes = new BrushCollection();
                SetContentBrushes(dp, brushes);
            }
            if (brushes.Count == 1 && (ReferenceEquals(brushes[0], args.OldValue)))
                brushes[0] = args.NewValue as Brush;
            if (brushes.Count == 0)
                brushes.Add(args.NewValue as Brush);
        }

        public static void SetContentBrush(DependencyObject element, Brush value)
        {
            element.SetValue(ContentBrushProperty, value);
        }

        public static Brush GetContentBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(ContentBrushProperty);
        }

        public static readonly DependencyProperty ContentBrushesProperty = DependencyProperty.RegisterAttached(
            "ContentBrushes", // Shadow the name so the parser does not skip GetContentBrushes
            typeof(BrushCollection), typeof(Props), new PropertyMetadata(default(BrushCollection)));

        public static void SetContentBrushes(DependencyObject element, BrushCollection value)
        {
            element.SetValue(ContentBrushesProperty, value);
        }

        public static BrushCollection GetContentBrushes(DependencyObject element)
        {
            var collection = (BrushCollection)element.GetValue(ContentBrushesProperty);
            if (collection == null)
            {
                collection = new BrushCollection();
                element.SetValue(ContentBrushesProperty, collection);
            }
            return collection;
        }
    }
}
