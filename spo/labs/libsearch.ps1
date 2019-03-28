
if($args.Length -eq 0) {
    "No arguments!"
    Return
}

$arglocs = @{}

$dirs = (Get-Item Env:LIB).Value.Split(";")
foreach ($dir in $dirs) {
    $files = Get-ChildItem -Path $dir | Where-Object {$_.Extension -eq ".lib"}
    foreach ($file in $files) { 
        dumpbin /EXPORTS $file.Name > lib.txt
        foreach ($arg in $args)  {
            $found = Select-String -Path $file.PSPath -Pattern $arg -Quiet
            if ($found) { 
                if (! $arglocs.Contains($arg)) { 
                    $arglocs.Add($arg, (New-Object System.Collections.Generic.List[string]) ) 
                }
                $arglocs[$arg].Add($file.Name)
            }
        }
    }
}

foreach ($arg in $args) {
    if ($arglocs.Contains($arg)) { 
        "Found " + $arg + " in: " + ( $arglocs[$arg].ToArray() -Join ", " )
    }
    else {
        "Couldn't find " + $arg
    }
}

Remove-Item -Path .\lib.txt