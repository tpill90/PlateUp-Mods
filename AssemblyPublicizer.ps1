# TODO document that this is a necessary step in order to compile this project
# TODO not all of the assemblies get publicized correctly

$dlls = Get-ChildItem "C:\Program Files (x86)\Steam\steamapps\common\PlateUp\PlateUp\PlateUp_Data\Managed" -Filter "Kitchen*.dll"

foreach($dll in $dlls)
{
    .\libraries\AssemblyPublicizer.exe $dll.FullName
}

# Renaming to match the original assembly names
Get-ChildItem .\publicized_assemblies | Rename-Item -NewName { $_.Name -replace "_publicized", "" }

# Copying over to the game dir
# TODO parameterize this
# Copy-item .\publicized_assemblies -Recurse -Destination "C:\Program Files (x86)\Steam\steamapps\common\PlateUp\PlateUp\PlateUp_Data"