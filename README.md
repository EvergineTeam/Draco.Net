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

## Build for windows and UWP

You need:
- Python3
- [CMake](https://cmake.org/download)
- Visual Studio C++

Just run the `build.py` script
```ps1
python build.py
```

## Build for Wasm

You need:
- Python3
- [CMake](https://cmake.org/download)
- [Emscripten SDK](https://emscripten.org/docs/getting_started/downloads.html)
- [Ninja](https://ninja-build.org/)

Run the build.py script, and provide the path to the Emscripten SDK:

```ps1
python build.py --emscripten_sdk C:/APPS/emsdk
```

NOTE:
Unlike in other platforms, in Wasm, we need to use a static library. Emscripten doesn't have good support for shared libraries and performance is much worse.

## Build for Android

You need:
- Python3
- [CMake](https://cmake.org/download)
- [Android NDK](https://developer.android.com/ndk/downloads)
- [Ninja](https://ninja-build.org/)

Run the build.py script, and provide the path to the Android NDK:

```ps1
python build.py --android_ndk C:/APPS/android-ndk-r26d
```

## Compile for all platforms

If you can compile all platforms with a single cmd:

```ps1
python build.py --emscripten_sdk C:/APPS/emsdk --android_ndk C:/APPS/android-ndk-r26d
```

## Build nuget

You will find all the compiled libraries in the "build/OUT" folder.

Copy the contents of "build/OUT" to this repo inside "Evergine.Bindings.Draco".

Run the `Generate-Nugets.ps1` script.

There is also a github Action for publishing the nuget to [](https://www.nuget.org/packages/Evergine.Bindings.Draco).

## Using the nuget in your project

Just add the nuget dependency to your project.

NOTE: for Wasm, you will also need to add the dependency to the `.Web` project! This is because the static library needs to be linked to the executable (not to a dll).

