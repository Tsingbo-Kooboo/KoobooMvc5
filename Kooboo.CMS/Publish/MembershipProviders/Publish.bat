
copy "..\..\Kooboo.CMS.Membership.Persistence.EntityFramework\bin\Release\Kooboo.CMS.Membership.Persistence.EntityFramework.dll" "EntityFramework\"

copy "..\..\packages\EntityFramework.6.1.3\lib\net45\*.dll" "EntityFramework\*.*"

..\7z\7z a ..\Released\Membership_Providers.zip EntityFramework\*.*
