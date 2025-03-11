# Plate Up! Mods

A collection of my mods for the game [Plate Up!](https://store.steampowered.com/app/1599600/PlateUp/)

# Mod List

<table>
      <tr>
        <td>Free Camera Control</td>
        <td>Allows you to manually control the position and zoom of the camera!</td>
        <td>[Readme](https://github.com/tpill90/PlateUp-Mods/blob/main/FreeCameraControl/README.md)</td>
        <td> https://steamcommunity.com/sharedfiles/filedetails/?id=3437341535 </td>
      </tr>
</table>

# Installation

https://steamcommunity.com/id/suuhduude/myworkshopfiles/?appid=1599600

# Development

Logs : `%localappdata%low/It's Happening/PlateUp/Player.log`

`Get-Content -Path .\Player.log -Wait | Where-Object { $_ -match "TestModTag" }`

https://github.com/Valheim-Modding/Wiki/wiki/Debugging-Plugins-via-IDE

## Development Environment Setup

How to setup the development enviroment for this project:

1. Install [Visual Studio 2022](https://visualstudio.microsoft.com) and add the C# workload.
2. Update `Environment.props` in the projects base path, and update the property `PlateUpInstallDir` to point to the path of your installed copy of Plate Up!.  The default value is set to point to the default Steam install directory, so if you havent changed where Steam is installed there shouldn't be anything for you to do.

## Building and Testing

1. Build the project with target `Debug`. The mod will automatically be copied over to the game's mod directory.  You only need to launch the game to test it!
2. Clicking `Start Without Debugging` in Visual Studio will automatically build and launch the game for you.

# Publishing

Update `MOD_VERSION` in `AssemblyInfo.cs` and build a new version.  Then run the Plate Up! mod uploader that is in the `C:\Program Files (x86)\Steam\steamapps\common\PlateUp\PlateUp\PlateUp_Data` directory.  Point the mod uploader to the `workshop` dir found in each mod's source code folder.

* How to format the Workshop description : [formatting help](https://steamcommunity.com/comment/Guide/formattinghelp)
* Markdown to Steam converter : [steamdown](https://steamdown.vercel.app/)