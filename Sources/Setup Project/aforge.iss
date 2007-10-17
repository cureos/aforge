; AForge.NET setup project

[Setup]
AppName=AForge.NET Framework
AppVerName=AForge.NET Framework 1.5.1
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
Name: "sources"; Description: "Sources"; Types: full custom
Name: "samples"; Description: "Samples"; Types: full custom
Name: "iplab"; Description: "Image Processing Lab"; Types: full custom

[Files]
Source: "Files\Copyright.txt"; DestDir: "{app}"; Components: libs
Source: "Files\Release notes.txt"; DestDir: "{app}"; Components: libs
Source: "Files\Release\*"; DestDir: "{app}\Release"; Components: libs
Source: "Files\Docs\*"; DestDir: "{app}\Docs"; Components: docs
Source: "Files\Sources\*"; DestDir: "{app}\Sources"; Components: sources; Flags: recursesubdirs
Source: "Files\Samples\*"; DestDir: "{app}\Samples"; Components: samples; Flags: recursesubdirs
Source: "Files\IPLab\*"; DestDir: "{app}\IPLab"; Components: iplab

[Icons]
Name: "{group}\Documentation"; Filename: "{app}\Docs"
Name: "{group}\Image Processing Lab"; Filename: "{app}\IPLab\iplab.exe"
Name: "{group}\Project Home"; Filename: "http://code.google.com/p/aforge/"
Name: "{group}\Release Notes"; Filename: "{app}\Release notes.txt"

Name: "{group}\{cm:UninstallProgram,AForge.NET Framework}"; Filename: "{uninstallexe}"
