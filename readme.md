# Rayman 3 GBA port to MonoGame
This project is a recreation of Rayman 3 GBA to [MonoGame](https://monogame.net), with the goal of porting the game to PC and other platforms. This is *not* a decompilation or source port, but rather a recreation of the game in C#. It is however heavily based on the original engine, attempting to keep much of the same structure as seen by decompiling the game using Ghidra. The goal is to have the game be functionally identical to the GBA game, with additional enhancements as options.

![Title screen](img/title_screen.png)

## GbaEngine
The original game uses [Ubisoft's GbaEngine](https://raymanpc.com/wiki/en/GbaEngine), an engine which was written in C and built from Ubisoft's GBC engine. The engine is object-oriented, which makes it work well for a C# re-creation, and consists of several, mostly independent, modules.

This is the structure of the original engine, as seen in the version of it used by Rayman 3. Later versions, and ones in different branches (developed by different Ubisoft studios) have several changes.

#### GbaCommon
- **GbaSDK** - reusable code for any GBA game, such as display drivers, a graphics engine for allocating tiles in an optimized way and network code for multiplayer.
- **GbaStdLib** - mainly a memory manager for allocating memory and sound code for playing music.

#### GbaSimilar
- **GbaAnimation** -  different types of animated objects which can be played with the animation player as well as palette and sprite managers.
- **GbaTileGraphics** - playfield, along with different types of game layers, and cameras for scrolling.
- **Gba2DPlatform** - components for a 2D platformer, such as actors, captors, scenes, knots and more.
- **Gba3d** - components for 3D rendering through software (only used in the game credits)

#### GbaSpecific
- **Rayman2** - game project

Mockups of the source code file structures for all dated Rayman 3 prototypes, as well as other known prototypes for the same engine, have been made under [gbaengine/structure](gbaengine/structure).

### ROM
The majority of the game data in the ROM is stored as resource blocks in a data table. Each resource has a list of dependencies, which are additional resources that it links to. [BinarySerializer.Ubisoft.GbaEngine](https://github.com/BinarySerializer/BinarySerializer.Ubisoft.GbaEngine) is being used to deserialize this data from the ROM.

### Frame
A `frame` is the object which handles the current game loop. Only one frame instance can be executing at a time and is managed by the `FrameManager` class. For example, the intro and main menu are two examples of different frames. The same goes for each level.

### Playfield
A `playfield` is a collection of different game layers. These can be tile graphics or physical layers (for collision). Layers are grouped into `clusters` which are what the camera scrolls. Each cluster has a different scroll speed, thus allowing layers to produce a parallax scrolling effect. Besides this, additional effects can be applied such as animated tiles and palette swapping.

### Scene
Each level in the game consists of a scene. A scene mainly consists of a playfield, a `camera actor`, a collection of `game objects` and `dialogs`. A dialog is a some form of UI element that renders to the screen, such as the HUD and pause menu.

#### Actor
An actor is a game object which has an animated object and a finite-state machine (FSM) to execute code for its current state. Actors can inherit from different base classes which give it different properties. For example, an actor inheriting from `ActionActor` can use actions defined in the game data, which in turn indicate which animation is to be played and optionally has movement data. The movement data is however only used if the actor inherits from `MovableActor`.

Additionally, actors are grouped based on their life-time. `Normal actors` have defined activation zones, which in the game is determined by the knots, which act as sectors, defining which objects are active where in the level.

`Always actors` are actors which remain active until manually disabled.

Besides this there are also special types of actors. The `main actors` are always defined as the first actors in the level (as always objects). In this game it is usually Rayman, unless the level has a different play style.

`Projectile actors` are actors which are spawned by other actors. An example is the water splash from the piranhas. These are usually defined as always objects, but not always.

#### Captor
A captor is an object which defines a collision box. When an actor collides with the box it triggers events, which usually involve enabling another object or playing a sound.

### States and messages
Most objects in the game communicate by having a `ProcessMessage` function. This allows it to retrieve a specific type of message, identified by an id, and optional parameters. Most objects also contain a finite-state machine (FSM) which runs each frame. This determines the current state the object is in and which behavior it should perform.

## MonoGame port
This MonoGame port of the engine is still early in development. There are some noticeable changes from the original engine, such as relying less on singleton instances and using floats instead of fixed-point integers.

### Rendering
Since the original game was developed for the GBA the rendering is handled very differently from how it would be in a modern engine. Data is loaded into VRAM, which can only hold so much data. Level objects have pre-calculated addresses for where they are to be loaded into vram during different parts of the level and graphics can be defined to either be loaded dynamically (when in use) or statically (once during load).

Additionally, the game takes advantage of several features on the GBA to produce some of its effects. Level transitions are handled using windows, the clouds scrolling at different speeds in the background is done using vsync callbacks which occur after each scanline is drawn etc.

Initially this project was set up to manually draw each game frame pixel by pixel and thus emulate how the GBA handles drawing. This however came with major performance costs and would complicate allowing higher resolution. This was thus later changed into rendering using textures. Each sprite is now a texture, as well as the backgrounds. Certain effects might not be replicated exactly the same as on a GBA, such as affine sprite rendering and alpha blending, but the goal is to have it appear as similar as possible.

![Zoomed out example](img/zoom_out.png)
*The goal is to allow the game to render in higher resolution and different aspect ratios*

### Debug mode
![Debug mode](img/debug_mode.png)
Pressing the `tab` key while playing will toggle the debug mode. This is set up using ImGUI and is meant to help debugging the game.

## Decompiling
The engine is being recreated thanks to Ghidra allowing the original game's code to be decompiled. Both the GBA and N-Gage versions are being used to re-create the code, so that any differences between them can be correctly handled. The N-Gage version is also easier to decompile due to it having fewer compiler optimizations, such as no apparent function inlining.

Various prototypes of games on this engine were compiled with assertions in the code, which contain debug strings for if the assertions fail. These are incredibly useful as they contain function names, variable names, source file paths and more. Thanks to this information we have a very good idea of how the engine was structured and how things were named.

## Playing
In order to play the game you will need to place your game ROMs in the `Games` folder, inside a sub-folder for each game. The name of the ROM file doesn't matter, as long as it has the correct file extension (.gba for GBA and .app for N-Gage). Additionally the N-Gage version requires a .dat file with the same name as the .app file.

This is an example of a folder structure which will allow you to play both the GBA and N-Gage versions:

```
├───GBA
│   ├── ROM.gba
└───N-Gage
    ├── rayman3.app
    └── rayman3.dat
```

The game saves are stored in .sav files with the same name as the ROM. The format is identical to that used by GBA emulators, and save files can thus be interchanged.

### Button mapping
The button mapping will be made customizable in the future, along with controller support. As of now this is the current mapping:

| **Description**          | **Input**          |
|--------------------------|--------------------|
| GBA A-button             | Space              |
| GBA B-button             | S                  |
| GBA Select-button        | C                  |
| GBA Start-button         | V                  |
| GBA D-pad                | Arrow keys         |
| GBA R-button             | W                  |
| GBA L-button             | Q                  |
| Toggle pause             | Ctrl+P             |
| Speed up game            | Left shift         |
| Run one frame            | Ctrl+F             |
| Toggle debug mode        | Tab                |
| Toggle menu              | Escape             |
| Toggle showing collision | T                  |
| Toggle no-clip           | Z                  |
| Increase no-clip speed   | Space              |
| Scroll camera            | Right mouse button |
| Toggle fullscreen        | Alt+Enter          |

Besides these you can also hold down `left shift` while resizing the window to maintain the aspect ratio.

You can also launch the game with a BizHawk .bk2 TAS file which will have it play the button inputs from that.

## Want to help?
Do you want to help out? Feel free to contact me if so! This is currently a side project and might take a long time to finish, especially when being worked on by only myself.