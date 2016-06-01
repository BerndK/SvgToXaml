using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfDemoApp;

// ReSharper disable CheckNamespace

namespace RelativeBrushes
{
    public class BrushCollection : ObservableCollection<Brush>
    {
        public WeakReference Parent;
    }

    //[Bindable(BindableSupport.Yes)]
    public static class Props
    {
        
        public static readonly DependencyProperty ContentBrushProperty = DependencyProperty.RegisterAttached(
            "ContentBrush", typeof(Brush), typeof(Props), new PropertyMetadata(default(Brush), ContentBrushPropertyChangedCallback));

        private static void ContentBrushPropertyChangedCallback(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            var brushes = GetContentBrushes(dp) as BrushCollection;
            var brushesCreated = false;
            if (brushes == null)
            {
                brushes = new BrushCollection();
                brushesCreated = true;
            }
            if (brushes.Count == 1 && (ReferenceEquals(brushes[0], args.OldValue)))
                brushes[0] = args.NewValue as Brush;
            if (brushes.Count == 0)
                brushes.Add(args.NewValue as Brush);
            if (brushesCreated)
                SetContentBrushes(dp, brushes);
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
            "ContentBrushes", // possibly Shadow the name so the parser does not skip GetContentBrushes
            typeof(BrushCollection), typeof(Props), new PropertyMetadata(default(BrushCollection), ContentBrushesPropertyChangedCallback));

        private static void ContentBrushesPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var visual = dependencyObject as Visual;
            if (visual is Image && args.NewValue is BrushCollection)
            {
                var image = visual as Image;
                var brushes = (BrushCollection)args.NewValue;
                var imageSource = image.Source;

                SetBrushesToImageSource(imageSource, brushes);
                brushes.Parent = new WeakReference(image);
                brushes.CollectionChanged += BrushesOnCollectionChanged;
            }
        }

        private static void BrushesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var brushes = sender as BrushCollection;
            if (brushes != null)
            {
                if (brushes.Parent.IsAlive)
                {
                    var image = brushes.Parent.Target as Image;
                    if (image != null)
                    {
                        var imageSource = image.Source;
                        SetBrushesToImageSource(imageSource, brushes);
                    }
                }
            }
        }

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

        public static readonly DependencyProperty SourceExProperty = DependencyProperty.RegisterAttached(
            "SourceEx", typeof(ImageSource), typeof(Props), new PropertyMetadata(default(ImageSource), SourceExPropertyChangedCallback));

        private static void SourceExPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var visual = dependencyObject as Visual;
            if (visual is Image && args.NewValue is ImageSource)
            {
                var image = visual as Image;
                var brushes = GetContentBrushes(image);
                var imageSource = (ImageSource) args.NewValue;

                SetBrushesToImageSource(imageSource, brushes);

                image.Source = imageSource;
            }
        }

        public static void SetSourceEx(DependencyObject element, ImageSource value)
        {
            element.SetValue(SourceExProperty, value);
        }

        public static ImageSource GetSourceEx(DependencyObject element)
        {
            return (ImageSource)element.GetValue(SourceExProperty);
        }

        public static void SetBrushesToImageSource(ImageSource imageSource, BrushCollection brushCollection)
        {
            if (imageSource != null && brushCollection != null)
            {
                var brushProps = GetBrushesPropsFromImageSource(imageSource);
                
                for (int i = 0; i < brushProps.Length; i++)
                {
                    if (i < brushCollection.Count)
                    {
                        var brushProp = brushProps[i];
                        var brush = brushCollection[i];
                        brushProp.Dp.SetValue(brushProp.Prop, brush);
                    }
                }
            }
        }

        private static BrushProp[] GetBrushesPropsFromImageSource(ImageSource imageSource)
        {
            IEnumerable<BrushProp> brushProps = null;
            if (imageSource is DrawingImage)
            {
                var drawing = ((DrawingImage)imageSource).Drawing;
                brushProps = GetBrushesFromDrawing(drawing);
            }

            if (brushProps == null)
                brushProps = Enumerable.Empty<BrushProp>();

            return brushProps.Where(e => e.Dp != null).ToArray();
        }

        private static IEnumerable<BrushProp> GetBrushesFromDrawing(Drawing drawing)
        {
            if (drawing is DrawingGroup)
                return GetBrushesFromDrawingGroup((DrawingGroup)drawing);
            if (drawing is GeometryDrawing)
                return GetBrushesFromGeometryDrawing((GeometryDrawing)drawing);
            if (drawing is GlyphRunDrawing)
                return GetBrushesFromGlyphRunDrawing((GlyphRunDrawing)drawing);
            //ImageDrawing not handled here
            //VideoDrawing not handled here

            return Enumerable.Empty<BrushProp>();
        }

        private static IEnumerable<BrushProp> GetBrushesFromDrawingGroup(DrawingGroup drawingGroup)
        {
            return drawingGroup.Children.SelectMany(GetBrushesFromDrawing);
        }

        private static IEnumerable<BrushProp> GetBrushesFromGeometryDrawing(GeometryDrawing geometryDrawing)
        {
            yield return new BrushProp(geometryDrawing.Pen, Pen.BrushProperty);
            yield return new BrushProp(geometryDrawing, GeometryDrawing.BrushProperty);
        }

        private static IEnumerable<BrushProp> GetBrushesFromGlyphRunDrawing(GlyphRunDrawing glyphRunDrawing)
        {
            yield return new BrushProp(glyphRunDrawing, GlyphRunDrawing.ForegroundBrushProperty);
        }

        class BrushProp
        {
            public BrushProp(DependencyObject dp, DependencyProperty prop)
            {
                Dp = dp;
                Prop = prop;
            }

            public DependencyObject Dp { get; set; }
            public DependencyProperty Prop { get; set; }
        }
    }
}
