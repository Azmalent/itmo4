if($args.Length -ne 1) {
    "Usage: generator <length>"
    Return
}

try {
    $length = [int] $args[0]
}
catch {
    "Error: incorrect number format"
    Return
}

if ($length -le 0) {
    "Error: length must be positive"
    Return
}

$rng = New-Object -TypeName System.Random -ArgumentList ([System.DateTime]::Now).Millisecond

for ($i = 0; $i -lt $length; $i++) {
    $number = $rng.Next(0, 10000000) 
    $number
}