using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Evergine.LibraryLoader;

namespace Evergine.Bindings.Draco
{
    public class Draco
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Mesh
        {
            public IntPtr meshPtr;

            public UInt32 numFaces { get => Draco_DecompressedMesh_GetNumFaces(this); }
            public UInt32 numVertices { get => Draco_DecompressedMesh_GetNumVertices(this); }
            public UInt32 numAttributes { get => Draco_DecompressedMesh_GetNumAttributes(this); }

            public Attribute GetAttribute(int index)
            {
                Attribute attrib;
                Draco_DecompressedMesh_GetAttribute(this, index, out attrib);
                return attrib;
            }
            public Attribute GetAttributeByType(AttributeType attribType, int index)
            {
                Attribute attrib;
                Draco_DecompressedMesh_GetAttributeByType(this, attribType, index, out attrib);
                return attrib;
            }
            public Attribute GetAttributeByUniqueId(UInt32 uniqueId)
            {
                Attribute attrib;
                Draco_DecompressedMesh_GetAttributeByUniqueId(this, uniqueId, out attrib);
                return attrib;
            }

            public Data GetIndices()
            {
                Data data;
                Draco_DecompressedMesh_GetIndices(this, out data);
                return data;
            }
        };

        public enum AttributeType : Int32
        {
            INVALID = -1,

            POSITION = 0,
            NORMAL,
            COLOR,
            TEX_COORD,
            // A special id used to mark attributes that are not assigned to any known
            // predefined use case. Such attributes are often used for a shader specific
            // data.
            GENERIC,

            TANGENT,
            MATERIAL,
            JOINTS,
            WEIGHTS,

            NAMED_ATTRIBUTES_COUNT,
        }

        public enum DataType : UInt32
        {
            DT_INVALID = 0, // Not a legal value for DataType. Used to indicate a field has not been set.
            DT_INT8,
            DT_UINT8,
            DT_INT16,
            DT_UINT16,
            DT_INT32,
            DT_UINT32,
            DT_INT64,
            DT_UINT64,
            DT_FLOAT32,
            DT_FLOAT64,
            DT_BOOL,
            DT_TYPES_COUNT
        };

        static Draco()
        {
            var baseDir = Path.GetDirectoryName(typeof(Draco).Assembly.Location);

            LibraryLoader.LibraryLoader.Instance.Register(
                Library.Create(LibName)
                .AddConfig( ManualConfig.Create()
                    .SetWindows_x64(Path.Combine(baseDir, "runtimes/win-x64/native"))
                    .SetWindows_x86(Path.Combine(baseDir, "runtimes/win-x86/native"))
                )
                .SetPlatform(Platform.Windows, $"{LibName}.dll")
            ).Load();
        }

        public static uint GetSize(DataType t)
        {
            switch (t)
            {
                case DataType.DT_INT8:
                case DataType.DT_UINT8:
                case DataType.DT_BOOL:
                    return 1;
                case DataType.DT_INT16:
                case DataType.DT_UINT16:
                    return 2;
                case DataType.DT_INT32:
                case DataType.DT_UINT32:
                case DataType.DT_FLOAT32:
                    return 4;
                case DataType.DT_INT64:
                case DataType.DT_UINT64:
                case DataType.DT_FLOAT64:
                    return 8;
            }
            return 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Attribute
        {
            public IntPtr internalPtr;

            public AttributeType attributeType { get => Draco_Attribute_GetAttributeType(this); }
            public DataType dataType { get => Draco_Attribute_GetDataType(this); }
            public UInt32 numComponents { get => Draco_Attribute_GetNumComponents(this); }
            public UInt32 uniqueId { get => Draco_Attribute_GetUniqueId(this); }
        };

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct Data
        {
            public DataType dataType;
            public UInt32 dataSize;
            public IntPtr data;
        };

        enum DecompressReturnCode : int
        {
            ok = 0,
            nullParams = -1,
            cantParseGeomType = -2,
            cantParseTriangularMesh = -3,
            invalidGeomType = -3,

            pointCloudsNotSupportedYet = -1000,
        };

        // remember to call Release(mesh) afterwards
        public static Mesh Decompress(IntPtr compressedData, UIntPtr compressedDataSize)
        {
            Mesh mesh;
            DecompressReturnCode retCode = Draco_Decompress(compressedData, compressedDataSize, out mesh);
            return mesh;
        }

        public static void Release(Mesh m) { Draco_DecompressedMesh_Release(m); }
        public static void Release(Data d) { Draco_Data_Release(d); }

        // remember to call Release(data) afterwards
        public static Data GetData(Mesh m, Attribute a)
        {
            Data data;
            Draco_Attribute_GetData(m, a, out data);
            return data;
        }

        // --- Bindings to the native Draco imported library ---
        #region BINDINGS
        private const string LibName = "draco_tiny_dec";

        [DllImport(LibName)]
        private static extern unsafe DecompressReturnCode Draco_Decompress(
            IntPtr compressedData,
            UIntPtr compressedDataSize,
            out Mesh decompressedMesh);

        [DllImport(LibName)]
        private static extern unsafe void Draco_DecompressedMesh_Release(Mesh m);
        [DllImport(LibName)]
        private static extern unsafe void Draco_Data_Release(Data d);

        [DllImport(LibName)] private static extern unsafe UInt32 Draco_DecompressedMesh_GetNumFaces(Mesh m);
        [DllImport(LibName)] private static extern unsafe UInt32 Draco_DecompressedMesh_GetNumVertices(Mesh m);
        [DllImport(LibName)] private static extern unsafe UInt32 Draco_DecompressedMesh_GetNumAttributes(Mesh m);

        [DllImport(LibName)] private static extern unsafe void Draco_DecompressedMesh_GetAttribute(Mesh m, int index, out Attribute attribute);
        [DllImport(LibName)] private static extern unsafe void Draco_DecompressedMesh_GetAttributeByType(Mesh m, AttributeType attribType, int index, out Attribute attribute);
        [DllImport(LibName)] private static extern unsafe void Draco_DecompressedMesh_GetAttributeByUniqueId(Mesh m, UInt32 uniqueId, out Attribute attribute);
        [DllImport(LibName)] private static extern unsafe void Draco_DecompressedMesh_GetIndices(Mesh m, out Data data);

        [DllImport(LibName)] private static extern unsafe AttributeType Draco_Attribute_GetAttributeType(Attribute a);
        [DllImport(LibName)] private static extern unsafe DataType Draco_Attribute_GetDataType(Attribute a);
        [DllImport(LibName)] private static extern unsafe UInt32 Draco_Attribute_GetNumComponents(Attribute a);
        [DllImport(LibName)] private static extern unsafe UInt32 Draco_Attribute_GetUniqueId(Attribute a);

        [DllImport(LibName)] private static extern unsafe void Draco_Attribute_GetData(Mesh m, Attribute a, out Data data);
        #endregion
    }
}