. .\Database.exe "Server=localhost;Database=ICanHasDotnetCore;Trusted_Connection=True;"
if($LastExitCode -ne 0)
{
	throw "Error occured, return code $LastExitCode"
}