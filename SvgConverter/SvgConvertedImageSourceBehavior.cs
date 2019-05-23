using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;

namespace SvgConverter
{
    /// <summary>
    /// If the converter is run with /extractChildElements:false /useSvgConvertedImageSourceBehavior:true then this behavior can be used like so:
    ///  <Image Width="64" Height="64" >
    ///      <i:Interaction.Behaviors>
    ///          <local:SvgConvertedImageSourceBehavior Source="{StaticResource invoice_readyDrawingImage}">
    ///             <SolidColorBrush Color="Firebrick"/>
    ///             <SolidColorBrush Color="Red"/>
    ///         </local:SvgConvertedImageSourceBehavior>
    ///     </i:Interaction.Behaviors>
    ///  </Image>
    /// ...thereby enabling dynamic colors for the icons.
    /// </summary>
    [ContentProperty("Brushes")]
    public class SvgConvertedImageSourceBehavior : Behavior<Image>
    {
        public SvgConvertedImageSourceBehavior()
        {
            this.Brushes = new ArrayList();
        }

        public static readonly DependencyProperty BrushesAssistProperty = DependencyProperty.RegisterAttached("BrushesAssist", typeof(IList), typeof(SvgConvertedImageSourceBehavior));

        public static IList GetBrushesAssist(DependencyObject element)
        {
            return (IList)element.GetValue(BrushesAssistProperty);
        }
        public static void SetBrushesAssist(DependencyObject element, IList value)
        {
            element.SetValue(BrushesAssistProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(DrawingImage), typeof(SvgConvertedImageSourceBehavior));

        public DrawingImage Source
        {
            get { return (DrawingImage)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty BrushesProperty = DependencyProperty.Register("Brushes", typeof(IList), typeof(SvgConvertedImageSourceBehavior));

        public IList Brushes
        {
            get { return (IList)GetValue(BrushesProperty); }
            set { SetValue(BrushesProperty, value); }
        }
       
        protected override void OnAttached()
        {
            base.OnAttached();


            var clonedSource = Source.Clone();
            this.AssociatedObject.Source = clonedSource;

            if (Brushes != null)
            {
                SetBrushesAssist(clonedSource, Brushes);
            }
        }
    }
}
