@echo off
set MsBuildPath="%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe"

if not exist %MsBuildPath% (
  echo %MsBuildPath%
  echo MSBuild path not found,publish failed!
  pause
  exit
)

call clear.bat

cd..

call update_version.vbs

cd Publish

nuget.exe restore ..\Kooboo.CMS-Release.sln

%MsBuildPath% ..\Kooboo.CMS-Release.sln /t:rebuild /l:FileLogger,Microsoft.Build.Engine;logfile=Publish.log; /p:VisualStudioVersion=15.0

%MsBuildPath% ..\Kooboo.CMS.Web\Kooboo.CMS.Web.csproj /t:ResolveReferences;Compile /t:_CopyWebApplication /p:Configuration=Release /p:WebProjectOutputDir=..\Publish\Web /p:OutputPath=..\Publish\Web\Bin /p:VisualStudioVersion=15.0

%MsBuildPath% ..\Kooboo.CMS.Content\Kooboo.CMS.Content.FileServer.sln /t:rebuild /l:FileLogger,Microsoft.Build.Engine;logfile=Publish_FileServer.log; /p:VisualStudioVersion=15.0

%MsBuildPath% ..\Kooboo.CMS.Content\Kooboo.CMS.Content.FileServer.Web\Kooboo.CMS.Content.FileServer.Web.csproj /p:VisualStudioVersion=15.0 /p:DeployOnBuild=true /p:PublishProfile=FileServer.Web.pubxml

call copy.bat

call zip.bat