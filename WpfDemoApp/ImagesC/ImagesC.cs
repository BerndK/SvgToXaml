 
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace WpfDemoApp.ImagesC
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ImagesC
    {
        private static readonly ComponentResourceKey Color1CompResKey = MakeKey("ImagesC.Color1");

        public static ResourceKey Color1
        {
            get { return Color1CompResKey; }
        }
        private static readonly ComponentResourceKey Color2CompResKey = MakeKey("ImagesC.Color2");

        public static ResourceKey Color2
        {
            get { return Color2CompResKey; }
        }
        private static readonly ComponentResourceKey Color1BrushCompResKey = MakeKey("ImagesC.Color1Brush");

        public static ResourceKey Color1Brush
        {
            get { return Color1BrushCompResKey; }
        }
        private static readonly ComponentResourceKey Color2BrushCompResKey = MakeKey("ImagesC.Color2Brush");

        public static ResourceKey Color2Brush
        {
            get { return Color2BrushCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconGeometryCompResKey = MakeKey("ImagesC._3d_view_iconGeometry");

        public static ResourceKey _3d_view_iconGeometry
        {
            get { return _3d_view_iconGeometryCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconColorBrushCompResKey = MakeKey("ImagesC._3d_view_iconColorBrush");

        public static ResourceKey _3d_view_iconColorBrush
        {
            get { return _3d_view_iconColorBrushCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconDrawingGroupCompResKey = MakeKey("ImagesC._3d_view_iconDrawingGroup");

        public static ResourceKey _3d_view_iconDrawingGroup
        {
            get { return _3d_view_iconDrawingGroupCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconDrawingImageCompResKey = MakeKey("ImagesC._3d_view_iconDrawingImage");

        public static ResourceKey _3d_view_iconDrawingImage
        {
            get { return _3d_view_iconDrawingImageCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconGeometryCompResKey = MakeKey("ImagesC.cloud_3_iconGeometry");

        public static ResourceKey cloud_3_iconGeometry
        {
            get { return cloud_3_iconGeometryCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconColorBrushCompResKey = MakeKey("ImagesC.cloud_3_iconColorBrush");

        public static ResourceKey cloud_3_iconColorBrush
        {
            get { return cloud_3_iconColorBrushCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconDrawingGroupCompResKey = MakeKey("ImagesC.cloud_3_iconDrawingGroup");

        public static ResourceKey cloud_3_iconDrawingGroup
        {
            get { return cloud_3_iconDrawingGroupCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconDrawingImageCompResKey = MakeKey("ImagesC.cloud_3_iconDrawingImage");

        public static ResourceKey cloud_3_iconDrawingImage
        {
            get { return cloud_3_iconDrawingImageCompResKey; }
        }
        private static readonly ComponentResourceKey JOGGeometry1CompResKey = MakeKey("ImagesC.JOGGeometry1");

        public static ResourceKey JOGGeometry1
        {
            get { return JOGGeometry1CompResKey; }
        }
        private static readonly ComponentResourceKey JOGGeometry2CompResKey = MakeKey("ImagesC.JOGGeometry2");

        public static ResourceKey JOGGeometry2
        {
            get { return JOGGeometry2CompResKey; }
        }
        private static readonly ComponentResourceKey JOGColor1BrushCompResKey = MakeKey("ImagesC.JOGColor1Brush");

        public static ResourceKey JOGColor1Brush
        {
            get { return JOGColor1BrushCompResKey; }
        }
        private static readonly ComponentResourceKey JOGColor2BrushCompResKey = MakeKey("ImagesC.JOGColor2Brush");

        public static ResourceKey JOGColor2Brush
        {
            get { return JOGColor2BrushCompResKey; }
        }
        private static readonly ComponentResourceKey JOGColor3BrushCompResKey = MakeKey("ImagesC.JOGColor3Brush");

        public static ResourceKey JOGColor3Brush
        {
            get { return JOGColor3BrushCompResKey; }
        }
        private static readonly ComponentResourceKey JOGColor4BrushCompResKey = MakeKey("ImagesC.JOGColor4Brush");

        public static ResourceKey JOGColor4Brush
        {
            get { return JOGColor4BrushCompResKey; }
        }
        private static readonly ComponentResourceKey JOGDrawingGroupCompResKey = MakeKey("ImagesC.JOGDrawingGroup");

        public static ResourceKey JOGDrawingGroup
        {
            get { return JOGDrawingGroupCompResKey; }
        }
        private static readonly ComponentResourceKey JOGDrawingImageCompResKey = MakeKey("ImagesC.JOGDrawingImage");

        public static ResourceKey JOGDrawingImage
        {
            get { return JOGDrawingImageCompResKey; }
        }
        private static ComponentResourceKey MakeKey(object id)
        {
            return new ComponentResourceKey(typeof(ImagesC), id);
        }
    }
}
