copy "..\..\Kooboo.CMS.Account.Persistence.EntityFramework\bin\Release\Kooboo.CMS.Account.Persistence.EntityFramework.dll" "EntityFramework\Kooboo.CMS.Account.Persistence.EntityFramework.dll"

copy "..\..\packages\EntityFramework.6.1.3\lib\net45\*.dll" "EntityFramework\*.*"

..\7z\7z a ..\Released\Account_Providers.zip EntityFramework\*.*