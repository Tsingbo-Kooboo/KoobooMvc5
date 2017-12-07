nuget pack Kooboo.Core\Kooboo.Core.nuspec -OutputDirectory Released
nuget setApiKey 12487df2-6ae4-449f-a648-4237aba653b6

xcopy "Web\*.*" "WPI\Kooboo_CMS\*.*" /S /E /Y /H

cd WPI

call Publish

cd..

copy SDK\Kooboo_CMS.chm Released\Kooboo_CMS.chm

cd FileServer.Web
..\7z\7z a ..\Released\FileServer_Web.zip 
cd..


