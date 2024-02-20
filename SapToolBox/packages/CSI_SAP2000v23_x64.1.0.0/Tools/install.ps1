<#
Install.ps1

code source: http://stackoverflow.com/questions/38504099/how-can-i-get-the-substring-of-net-version-instead-of-directly-pointing-the-ne
Author:
http://stackoverflow.com/users/1306132/andrey-bushman
https://www.nuget.org/profiles/Bush

Minor modifications by matthew.taylor: https://www.nuget.org/profiles/Matthew.Taylor
This PowerShell script will be launched by NuGet each time when
this package will be installed into the Visual Studio project.

This script sets `CopyLocal` to `false` for each Revit assembly.
#>
param($installPath, $toolsPath, $package, $project)


$net_folders = [System.IO.Directory]::GetDirectories(`
$installPath, '*', 'TopDirectoryOnly');

$file_names = New-Object `
'System.Collections.Generic.HashSet[string]';

foreach ($net in $net_folders) {

    $files = [System.IO.Directory]::EnumerateFiles($net,"*.dll"`
    ,"AllDirectories");

        foreach ($file in $files) {

            $file_name = [System.IO.Path]::`
            GetFileNameWithoutExtension($file);

            $file_names.Add($file_name);
    }
}

foreach ($reference in $project.Object.References) {

    if($file_names.Contains($reference.Name)) {

        $reference.CopyLocal = $false;
        $reference.SpecificVersion = $false;
    }
}