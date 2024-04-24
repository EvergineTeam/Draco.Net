Basic [Draco](https://github.com/EvergineTeam/draco) C# bindings.

Basic usage:

```C#

Draco.Mesh mesh = Draco.Decompress(compressedData, compressedDataSize);

for(UInt32 aitribI = 0; attribI < mesh.numAttributes; attribI)
{
    Attribute attrib = mesh.GetAttribute(attribI);

    Draco.Data data = Draco.GetData(mesh, attrib);
    if(data.dataType == DataType.DT_FLOAT32)
    {
        var verts = (float*)data.data;
        for(UInt32 vertI = 0; vertI < mesh.numVertices; vertI++)
        {
            for(UInt compI = 0; compI < attrib.numComponents; compI++)
            {
                // ...
            }
        }
    }
    
    Draco.Release(data);
}

Draco.Release(mesh);

```