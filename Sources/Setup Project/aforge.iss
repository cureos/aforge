; AForge.NET setup project

[Setup]
AppName=AForge.NET Framework
AppVerName=AForge.NET Framework 1.1.0
AppPublisher=AForge
AppPublisherURL=http://code.google.com/p/aforge
AppSupportURL=http://code.google.com/p/aforge
AppUpdatesURL=http://code.google.com/p/aforge
DefaultDirName={pf}\AForge.NET
DefaultGroupName=AForge.NET
AllowNoIcons=yes
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes
LicenseFile=gpl.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Components]
Name: "libs"; Description: "AForge.NET libraries"; Types: full compact custom; Flags: fixed
Name: "docs"; Description: "Documentation"; Types: full custom

[Files]
Source: "Files\Copyright.txt"; DestDir: "{app}"; Components: libs
Source: "Files\Release notes.txt"; DestDir: "{app}"; Components: libs
Source: "Files\Release\*"; DestDir: "{app}\Release"; Components: libs
Source: "Files\Docs\*"; DestDir: "{app}\Docs"; Components: docs

[Icons]
Name: "{group}\Documentation\AForge"; Filename: "{app}\Docs\AForge.Core.chm"
Name: "{group}\Documentation\AForge.Math"; Filename: "{app}\Docs\AForge.Math.chm"
Name: "{group}\Documentation\AForge.Imaging"; Filename: "{app}\Docs\AForge.Imaging.chm"
Name: "{group}\Documentation\AForge.Neuro"; Filename: "{app}\Docs\AForge.Neuro.chm"
Name: "{group}\Documentation\AForge.Genetic"; Filename: "{app}\Docs\AForge.Genetic.chm"

Name: "{group}\{cm:UninstallProgram,AForge.NET Framework}"; Filename: "{uninstallexe}"
