open System
open System.IO

let rec toBinAcc s =
    function
    | 0 -> if String.length (s) = 0 then "0" else s
    | n when n >= 1 -> toBinAcc (string (n &&& 1) + s) (n >>> 1)
    | _ -> "0b" + s

let printBin x = toBinAcc "" x
