<p align="center">
  <img src="https://raw.githubusercontent.com/BLaZeKiLL/VloxyHDRP/main/.github/assets/vloxy_logo.svg" width=256>
  <h1 align="center">Vloxy Engine HDRP</h1>
</p>

<p align="center">
  <!-- <a href="https://github.com/BLaZeKiLL/VloxyHDRP/releases">
    <img alt="GitHub release (latest by date)" src="https://img.shields.io/github/v/release/BLaZeKiLL/VloxyHDRP"> -->
  </a>
  <a href="https://openupm.com/packages/io.codeblaze.vloxyengine/">
    <img alt="OpenUPM" src="https://img.shields.io/npm/v/io.codeblaze.vloxyengine?label=openupm&amp;registry_uri=https://package.openupm.com" />
  </a>
  <!-- <a href="https://github.com/BLaZeKiLL/VloxyHDRP/actions">
    <img alt="Build Status" src="https://img.shields.io/github/actions/workflow/status/BLaZeKiLL/VloxyHDRP/build.yml?branch=main">
  </a> -->
  <!-- <a href="https://blazekill.github.io/vloxy-docs/">
    <img alt="GitHub Workflow Status" src="https://img.shields.io/github/actions/workflow/status/BLaZeKiLL/vloxy-docs/deploy.yml?branch=master&label=docs">
  </a> -->
  <a href="https://github.com/BLaZeKiLL/VloxyHDRP/blob/main/LICENSE.md">
    <img alt="GitHub" src="https://img.shields.io/github/license/BLaZeKiLL/VloxyHDRP">
  </a>
  <a href="https://www.youtube.com/c/CodeBlazeX">
    <img alt="YouTube Channel Subscribers" src="https://img.shields.io/youtube/channel/subscribers/UC_qfPIYfXOvg0SDAc8Z68WA?label=CodeBlaze&style=social">
  </a>
</p>

> :warning: **VloxyHDRP**: Is the next version of [VloxyEngine](https://github.com/BLaZeKiLL/VloxyEngine) and is unstable with some features lacking, using the original engine is recommended for production uses.

Performance oriented voxel engine for Unity. Latest release for the engine and sandbox application can be found [here](https://github.com/BLaZeKiLL/VloxyEngine/releases).

<img src="https://raw.githubusercontent.com/BLaZeKiLL/VloxyHDRP/main/.github/assets/1.png">

<img src="https://raw.githubusercontent.com/BLaZeKiLL/VloxyHDRP/main/.github/assets/2.png">

<img src="https://raw.githubusercontent.com/BLaZeKiLL/VloxyHDRP/main/.github/assets/3.png">

<img src="https://raw.githubusercontent.com/BLaZeKiLL/VloxyHDRP/main/.github/assets/4.png">

<img src="https://raw.githubusercontent.com/BLaZeKiLL/VloxyHDRP/main/.github/assets/5.png">

<img src="https://raw.githubusercontent.com/BLaZeKiLL/VloxyHDRP/main/.github/assets/6.png">

## Goals

| Description                     | Done |
|---------------------------------|:----:|
| Jobs & Burst                    |  ✔   |
| Extensible Api                  |  ✔   |
| Serialization                   |      |
| Streaming & Infinite generation |  ✔   |
| World Generation System         |  ✔   |
| Physics & Fluids                |  *   |
| Networking                      |      |

## Demos
Along with package releases a demo application showcasing the capabilities is also released for the following platforms
- Windows
- Android

Head over to the [release page](https://github.com/BLaZeKiLL/VloxyEngine/releases) to check them out

## Quick Start

- Get started by installing **Vloxy Engine** using one of the following methods
  - Unity Package latest can be found **[here](https://github.com/BLaZeKiLL/VloxyEngine/releases)**
  - OpenUPM, more info can be found **[here](https://openupm.com/packages/io.codeblaze.vloxyengine/)**
```bash title="OpenUPM Install Command"
openupm add io.codeblaze.vloxyengine
```

> While UPM is supported via OpenUPM, it is still recommended to add **Vloxy Engine** directly to the project as a package.
> With source access you would get the maximum control and freedom to tune the engine to your use case.


- Make sure the following dependencies are installed, they should be installed **automatically** regardless of the way you install **Vloxy Engine**
  - [Unity Maths](https://docs.unity3d.com/Packages/com.unity.mathematics@1.2/manual/index.html)
  - [Unity Burst](https://docs.unity3d.com/Packages/com.unity.burst@1.7/manual/index.html)
  - [Unity Collections](https://docs.unity3d.com/Packages/com.unity.collections@1.2/manual/index.html)

- After the package is imported you can open up one of the sample scene or import one of the world prefabs into your current scene.
In case you go the prefabs route, make sure to set the focus parameter on the world object around which the world would be generated.


## Documentation & Dev-logs
Documentation can be found [here](https://blazekill.github.io/vloxy-docs/), and it's source code is hosted [here](https://github.com/BLaZeKiLL/vloxy-docs)

I'll try to create devlogs for the major features in development as well as some tutorials which you can find on my [YouTube](https://www.youtube.com/c/CodeBlazeX) or on the Vloxy Engine website [blog](https://blazekill.github.io/vloxy-docs/blog).
