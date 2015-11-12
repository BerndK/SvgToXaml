 
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace WpfDemoApp.ImagesC
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ImagesC
    {
        private static readonly ComponentResourceKey Color1KeyCompResKey = MakeKey("ImagesC.Color1Key");

        public static ResourceKey Color1Key
        {
            get { return Color1KeyCompResKey; }
        }
        private static readonly ComponentResourceKey Color2KeyCompResKey = MakeKey("ImagesC.Color2Key");

        public static ResourceKey Color2Key
        {
            get { return Color2KeyCompResKey; }
        }
        private static readonly ComponentResourceKey Color1BrushKeyCompResKey = MakeKey("ImagesC.Color1BrushKey");

        public static ResourceKey Color1BrushKey
        {
            get { return Color1BrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey Color2BrushKeyCompResKey = MakeKey("ImagesC.Color2BrushKey");

        public static ResourceKey Color2BrushKey
        {
            get { return Color2BrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconKeyGeometryKeyCompResKey = MakeKey("ImagesC.cloud_3_iconKeyGeometryKey");

        public static ResourceKey cloud_3_iconKeyGeometryKey
        {
            get { return cloud_3_iconKeyGeometryKeyCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconKeyColorBrushKeyCompResKey = MakeKey("ImagesC.cloud_3_iconKeyColorBrushKey");

        public static ResourceKey cloud_3_iconKeyColorBrushKey
        {
            get { return cloud_3_iconKeyColorBrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconDrawingGroupKeyCompResKey = MakeKey("ImagesC.cloud_3_iconDrawingGroupKey");

        public static ResourceKey cloud_3_iconDrawingGroupKey
        {
            get { return cloud_3_iconDrawingGroupKeyCompResKey; }
        }
        private static readonly ComponentResourceKey cloud_3_iconDrawingImageKeyCompResKey = MakeKey("ImagesC.cloud_3_iconDrawingImageKey");

        public static ResourceKey cloud_3_iconDrawingImageKey
        {
            get { return cloud_3_iconDrawingImageKeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGKeyGeometry1KeyCompResKey = MakeKey("ImagesC.JOGKeyGeometry1Key");

        public static ResourceKey JOGKeyGeometry1Key
        {
            get { return JOGKeyGeometry1KeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGKeyGeometry2KeyCompResKey = MakeKey("ImagesC.JOGKeyGeometry2Key");

        public static ResourceKey JOGKeyGeometry2Key
        {
            get { return JOGKeyGeometry2KeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGKeyColor1BrushKeyCompResKey = MakeKey("ImagesC.JOGKeyColor1BrushKey");

        public static ResourceKey JOGKeyColor1BrushKey
        {
            get { return JOGKeyColor1BrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGKeyColor2BrushKeyCompResKey = MakeKey("ImagesC.JOGKeyColor2BrushKey");

        public static ResourceKey JOGKeyColor2BrushKey
        {
            get { return JOGKeyColor2BrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGKeyColor3BrushKeyCompResKey = MakeKey("ImagesC.JOGKeyColor3BrushKey");

        public static ResourceKey JOGKeyColor3BrushKey
        {
            get { return JOGKeyColor3BrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGKeyColor4BrushKeyCompResKey = MakeKey("ImagesC.JOGKeyColor4BrushKey");

        public static ResourceKey JOGKeyColor4BrushKey
        {
            get { return JOGKeyColor4BrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGDrawingGroupKeyCompResKey = MakeKey("ImagesC.JOGDrawingGroupKey");

        public static ResourceKey JOGDrawingGroupKey
        {
            get { return JOGDrawingGroupKeyCompResKey; }
        }
        private static readonly ComponentResourceKey JOGDrawingImageKeyCompResKey = MakeKey("ImagesC.JOGDrawingImageKey");

        public static ResourceKey JOGDrawingImageKey
        {
            get { return JOGDrawingImageKeyCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconKeyGeometryKeyCompResKey = MakeKey("ImagesC._3d_view_iconKeyGeometryKey");

        public static ResourceKey _3d_view_iconKeyGeometryKey
        {
            get { return _3d_view_iconKeyGeometryKeyCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconKeyColorBrushKeyCompResKey = MakeKey("ImagesC._3d_view_iconKeyColorBrushKey");

        public static ResourceKey _3d_view_iconKeyColorBrushKey
        {
            get { return _3d_view_iconKeyColorBrushKeyCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconDrawingGroupKeyCompResKey = MakeKey("ImagesC._3d_view_iconDrawingGroupKey");

        public static ResourceKey _3d_view_iconDrawingGroupKey
        {
            get { return _3d_view_iconDrawingGroupKeyCompResKey; }
        }
        private static readonly ComponentResourceKey _3d_view_iconDrawingImageKeyCompResKey = MakeKey("ImagesC._3d_view_iconDrawingImageKey");

        public static ResourceKey _3d_view_iconDrawingImageKey
        {
            get { return _3d_view_iconDrawingImageKeyCompResKey; }
        }
        private static ComponentResourceKey MakeKey(object id)
        {
            return new ComponentResourceKey(typeof(ImagesC), id);
        }
    }
}
