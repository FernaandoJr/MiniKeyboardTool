; Inno Setup script for MiniKeyboardTool (per-user install, no admin required)
; Compiled by the vendored compiler: tools/innosetup/ISCC.exe installer/minikeyboardtool.iss

#define AppId "{{B7E1A12C-4A5D-4E0F-9D2E-830E3432A9C4}"
#define AppName "MiniKeyboardTool"
#define AppExeName "MINI KeyBoard.exe"
#define AppVersion GetVersionNumbersString("..\dist\MINI KeyBoard.exe")

[Setup]
AppId={#AppId}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=FernaandoJr
AppPublisherURL=https://github.com/FernaandoJr/MiniKeyboardTool
DefaultDirName={localappdata}\Programs\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
UninstallFilesDir={app}
Compression=lzma2/max
SolidCompression=yes
; Lets the installer close a running instance politely, and lets the
; in-app updater hand off to a setup run while the old EXE is still open.
AppMutex=MiniKeyboardToolAppSingleInstance
CloseApplications=yes
OutputDir=..\dist
OutputBaseFilename=MiniKeyboardTool-{#AppVersion}-setup
WizardStyle=modern
SetupIconFile=..\src\HIDTester\app.ico
UninstallDisplayName={#AppName}
UninstallDisplayIcon={app}\{#AppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; \
    GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Everything the portable layout needs: EXE + config + HidLibrary + Theraot
Source: "..\dist\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; \
    Tasks: desktopicon

[UninstallDelete]
; remove every file we didn't explicitly install (logs, temp, leftovers)
Type: filesandordirs; Name: "{app}"
