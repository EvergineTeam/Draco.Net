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

# How to Build

## Build Native Libraries
Native libraries are built from [our fork](https://github.com/EvergineTeam/draco) of the draco library.

Download the repo
```ps1
git clone https://github.com/EvergineTeam/draco.git
cd draco
```

## Build for windows

You need:
- [CMake](https://cmake.org/download)
- Visual Studio C++

```ps1
cmake -B build -DCMAKE_BUILD_TYPE=Release -DDRACO_TINY_DECODE_SHARED_LIB=ON --log-level=VERBOSE
cmake --build build --target draco_tiny_dec --config Release
```

Your compiler .dll will be found in `build/Release/draco_tiny_dec.dll`

## Build for Wasm

You need:
- [CMake](https://cmake.org/download)
- [Emscripten SDK](https://emscripten.org/docs/getting_started/downloads.html)
- [Ninja](https://ninja-build.org/)

```ps1
mkdir build_wasm
cd build_wasm
cmake .. -DCMAKE_BUILD_TYPE=Release -G "Ninja" -DDRACO_TINY_DECODE_SHARED_LIB=ON --log-level=VERBOSE -DCMAKE_TOOLCHAIN_FILE=<EMSCRIPTEN_SDK>\upstream\emscripten\cmake\Modules\Platform\Emscripten.cmake -DCMAKE_CROSSCOMPILING_EMULATOR=<EMSCRIPTEN_SDK>/node/16.20.0_64bit/bin/node.exe -DDRACO_WASM=ON
ninja
```

This will compile a static library (.a) for Wasm. The outout be found in `build_wasm/libdraco_tiny_dec.a`.

NOTE:
Unlike in other platforms, in Wasm, we need to use a static library. Emscripten doesn't have good support for shared libraries and performance is much worse.

## Build nuget

Copy the compiled native libraries to this repo:
- Windows: Evergine.Bindings.Draco/runtimes/win-x64/native/draco_tiny_dec.dll
- Wasm: Evergine.Bindings.Draco/build/wasm/draco_tiny_dec.a

Run the `Generate-Nugets.ps1` script.

There is also a github Action for publishing the nuget to [](https://www.nuget.org/packages/Evergine.Bindings.Draco).

## Using the nuget in your project

Just add the nuget dependency to your project.

NOTE: for Wasm, you will also need to add the dependency to the `.Web` project! This is because the static library needs to be linked to the executable (not to a dll).

