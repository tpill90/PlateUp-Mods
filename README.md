# Plate Up! Mods

A collection of my mods for the game [Plate Up!](https://store.steampowered.com/app/1599600/PlateUp/).  All of my mods can be seen in the [Steam Workshop](https://steamcommunity.com/id/suuhduude/myworkshopfiles/?appid=1599600).

# Mod List

<table>
      <tr>
        <td>Free Camera Control</td>
        <td>Allows you to manually control the position and zoom of the camera!</td>
        <td> <a href="https://github.com/tpill90/PlateUp-Mods/blob/main/FreeCameraControl/README.md">Readme</a> </td>
        <td> <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=3437341535">Steam Workshop</a> </td>
      </tr>
      <tr>
        <td>Declutter Crane Mode UI</td>
        <td>Hides all UI elments from the Crane mode except for current money.  Now you can easily see your whole restaurant with nothing in the way! </td>
        <td> <a href="https://github.com/tpill90/PlateUp-Mods/blob/master/DeclutterCraneModeUI/README.md">Readme</a> </td>
        <td> <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=3443928799">Steam Workshop</a> </td>
      </tr>
</table>

# Installation
Mods can be easily installed by clicking `Subscribe` on the mod's workshop page.

# Development

## Dev Environment Setup

How to setup the development enviroment for this project:

1. Install [Visual Studio 2022](https://visualstudio.microsoft.com) and add the C# workload.
2. Update `Environment.props` in the projects base path, and update the property `PlateUpInstallDir` to point to the path of your installed copy of Plate Up!.  The default value is set to point to the default Steam install directory, so if you havent changed where Steam is installed there shouldn't be anything for you to do.

## Building and Testing

1. Build the project with target `Debug`. The mod will automatically be copied over to the game's mod directory.  You only need to launch the game to test it!
2. Clicking `Start Without Debugging` in Visual Studio will automatically build and launch the game for you.

## Viewing Logs

Inside each mod's directory will be a Powershell script named `_TailLogs.ps1` which can be run with the command `.\_TailLogs.ps1`.  This will watch PlateUp's log file and only display log messages for your specific mod.

# Publishing

Update `MOD_VERSION` in `AssemblyInfo.cs` and build a new version in `Release` mode.  Then run the Plate Up! mod uploader that is in the `C:\Program Files (x86)\Steam\steamapps\common\PlateUp\PlateUp\PlateUp_Data` directory.  Point the mod uploader to the `workshop` dir found in each mod's source code folder.



# Links

* How to format the Workshop description : [Formatting Help](https://steamcommunity.com/comment/Guide/formattinghelp)
* Markdown to Steam converter : [Steamdown](https://steamdown.vercel.app/)