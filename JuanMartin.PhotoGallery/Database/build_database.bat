@ECHO OFF


SET mysql_bin="C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql"
SET path="C:\GitHub\JuanMartin.PhotoGallery\JuanMartin.PhotoGallery\Database"
SET login=local	
ECHO ^> Tables ^<
SET subpath="%path%\Tables"
PUSHD %subpath%
FOR %%f IN (*.sql) DO (
	ECHO %%f
	ECHO "cmd.exe /c '%mysql_bin% --login-path=%login% < %%f'"
)
PUSHD %path%
	
rem cmd.exe /c '"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql" --login-path=local < "C:\GitHub\JuanMartin.PhotoGallery\JuanMartin.PhotoGallery\Database\Tables\tblLocation.sql"'