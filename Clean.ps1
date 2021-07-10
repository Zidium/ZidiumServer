Remove-Item Release -force -Recurse -ErrorAction Ignore -Verbose

$binList = Get-ChildItem -path . -Directory -Include 'bin' -Recurse -force -ErrorAction Ignore
$binList | Remove-Item -force -Recurse -Verbose

$objList = Get-ChildItem -path . -Directory -Include 'obj' -Recurse -force -ErrorAction Ignore 
$objList | Remove-Item -force -Recurse -Verbose