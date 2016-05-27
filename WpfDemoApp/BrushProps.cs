using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WpfDemoApp
{
    public class BrushCollection : Collection<Brush>
    {
    }

    //[Bindable(BindableSupport.Yes)]
    public static class BrushProps
    {
        
        public static readonly DependencyProperty ContentBrushProperty = DependencyProperty.RegisterAttached(
            "ContentBrush", typeof(Brush), typeof(BrushProps), new PropertyMetadata(default(Brush)));

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
            typeof(BrushCollection), typeof(BrushProps), new PropertyMetadata(default(BrushCollection)));

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
