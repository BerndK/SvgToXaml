using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace WpfDemoApp.Images
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Icons
    {
        private static readonly ComponentResourceKey JOGKeyGeometry1KeyCompResKey = MakeKey("Icons.JOGKeyGeometry1Key");

        public static ResourceKey JOGKeyGeometry1Key => JOGKeyGeometry1KeyCompResKey;
        private static readonly ComponentResourceKey JOGKeyGeometry2KeyCompResKey = MakeKey("Icons.JOGKeyGeometry2Key");

        public static ResourceKey JOGKeyGeometry2Key => JOGKeyGeometry2KeyCompResKey;
        private static readonly ComponentResourceKey JOGDrawingGroupKeyCompResKey = MakeKey("Icons.JOGDrawingGroupKey");

        public static ResourceKey JOGDrawingGroupKey => JOGDrawingGroupKeyCompResKey;
        private static readonly ComponentResourceKey JOGDrawingImageKeyCompResKey = MakeKey("Icons.JOGDrawingImageKey");

        public static ResourceKey JOGDrawingImageKey => JOGDrawingImageKeyCompResKey;
        private static readonly ComponentResourceKey cloud_3_iconKeyGeometryKeyCompResKey = MakeKey("Icons.cloud_3_iconKeyGeometryKey");

        public static ResourceKey cloud_3_iconKeyGeometryKey => cloud_3_iconKeyGeometryKeyCompResKey;
        private static readonly ComponentResourceKey cloud_3_iconDrawingGroupKeyCompResKey = MakeKey("Icons.cloud_3_iconDrawingGroupKey");

        public static ResourceKey cloud_3_iconDrawingGroupKey => cloud_3_iconDrawingGroupKeyCompResKey;
        private static readonly ComponentResourceKey cloud_3_iconDrawingImageKeyCompResKey = MakeKey("Icons.cloud_3_iconDrawingImageKey");

        public static ResourceKey cloud_3_iconDrawingImageKey => cloud_3_iconDrawingImageKeyCompResKey;
        private static readonly ComponentResourceKey _3d_view_iconKeyGeometryKeyCompResKey = MakeKey("Icons._3d_view_iconKeyGeometryKey");

        public static ResourceKey _3d_view_iconKeyGeometryKey => _3d_view_iconKeyGeometryKeyCompResKey;
        private static readonly ComponentResourceKey _3d_view_iconDrawingGroupKeyCompResKey = MakeKey("Icons._3d_view_iconDrawingGroupKey");

        public static ResourceKey _3d_view_iconDrawingGroupKey => _3d_view_iconDrawingGroupKeyCompResKey;
        private static readonly ComponentResourceKey _3d_view_iconDrawingImageKeyCompResKey = MakeKey("Icons._3d_view_iconDrawingImageKey");

        public static ResourceKey _3d_view_iconDrawingImageKey => _3d_view_iconDrawingImageKeyCompResKey;
        private static ComponentResourceKey MakeKey(object id)
        {
            return new ComponentResourceKey(typeof(Icons), id);
        }
    }
}
