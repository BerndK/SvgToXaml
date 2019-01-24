using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace IconResources.Images
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Icons
    {
        private static readonly ComponentResourceKey cloud_3_iconGeometryKeyCompResKey = MakeKey("Icons.cloud_3_iconGeometryKey");
        public static ResourceKey cloud_3_iconGeometryKey => cloud_3_iconGeometryKeyCompResKey;

        private static readonly ComponentResourceKey cloud_3_iconDrawingGroupKeyCompResKey = MakeKey("Icons.cloud_3_iconDrawingGroupKey");
        public static ResourceKey cloud_3_iconDrawingGroupKey => cloud_3_iconDrawingGroupKeyCompResKey;

        private static readonly ComponentResourceKey cloud_3_iconDrawingImageKeyCompResKey = MakeKey("Icons.cloud_3_iconDrawingImageKey");
        public static ResourceKey cloud_3_iconDrawingImageKey => cloud_3_iconDrawingImageKeyCompResKey;

        private static readonly ComponentResourceKey _3d_view_iconGeometryKeyCompResKey = MakeKey("Icons._3d_view_iconGeometryKey");
        public static ResourceKey _3d_view_iconGeometryKey => _3d_view_iconGeometryKeyCompResKey;

        private static readonly ComponentResourceKey _3d_view_iconDrawingGroupKeyCompResKey = MakeKey("Icons._3d_view_iconDrawingGroupKey");
        public static ResourceKey _3d_view_iconDrawingGroupKey => _3d_view_iconDrawingGroupKeyCompResKey;

        private static readonly ComponentResourceKey _3d_view_iconDrawingImageKeyCompResKey = MakeKey("Icons._3d_view_iconDrawingImageKey");
        public static ResourceKey _3d_view_iconDrawingImageKey => _3d_view_iconDrawingImageKeyCompResKey;

        private static readonly ComponentResourceKey JOGGeometry1KeyCompResKey = MakeKey("Icons.JOGGeometry1Key");
        public static ResourceKey JOGGeometry1Key => JOGGeometry1KeyCompResKey;

        private static readonly ComponentResourceKey JOGGeometry2KeyCompResKey = MakeKey("Icons.JOGGeometry2Key");
        public static ResourceKey JOGGeometry2Key => JOGGeometry2KeyCompResKey;

        private static readonly ComponentResourceKey JOGDrawingGroupKeyCompResKey = MakeKey("Icons.JOGDrawingGroupKey");
        public static ResourceKey JOGDrawingGroupKey => JOGDrawingGroupKeyCompResKey;

        private static readonly ComponentResourceKey JOGDrawingImageKeyCompResKey = MakeKey("Icons.JOGDrawingImageKey");
        public static ResourceKey JOGDrawingImageKey => JOGDrawingImageKeyCompResKey;

        private static ComponentResourceKey MakeKey(object id)
        {
            return new ComponentResourceKey(typeof(Icons), id);
        }
    }
}
