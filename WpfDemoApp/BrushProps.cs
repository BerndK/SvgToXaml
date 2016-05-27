using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace WpfDemoApp
{

    //[Bindable(BindableSupport.Yes)]
    public static class BrushProps
    {
        //public static readonly DependencyProperty ContentBrushProperty = DependencyProperty.Register(
        //    "ContentBrush", typeof(Brush), typeof(Image), new PropertyMetadata(default(Brush)));

        //public Brush ContentBrush
        //{
        //    get { return (Brush) GetValue(ContentBrushProperty); }
        //    set { SetValue(ContentBrushProperty, value); }
        //}

        //public static readonly DependencyProperty ContentBrushesProperty = DependencyProperty.Register(
        //    "ContentBrushes", typeof(Collection<Brush>), typeof(ImageBrushes), new PropertyMetadata(default(Collection<Brush>)));

        //public static void SetContentBrush(DependencyObject element, Brush value)
        //{
        //    element.SetValue(ContentBrushProperty, value);
        //}

        //public static Brush GetContentBrush(DependencyObject element)
        //{
        //    return (Brush) element.GetValue(ContentBrushProperty);
        //}
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
            "ContentBrushes", typeof(Collection<Brush>), typeof(BrushProps), new PropertyMetadata(default(Collection<Brush>)));

        public static void SetContentBrushes(DependencyObject element, Collection<Brush> value)
        {
            element.SetValue(ContentBrushesProperty, value);
        }

        public static Collection<Brush> GetContentBrushes(DependencyObject element)
        {
            return (Collection<Brush>) element.GetValue(ContentBrushesProperty);
        }

    }
}
