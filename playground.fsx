open System
open System.IO

// A place to try out code with FSharp Interactive

// Binary representation of int
Convert.ToString(7, 2)

// Convert signextended twos compliment to int
let trans n bits =
    let maxv = (1 <<< (n - 1)) - 1

    match bits with
    | x when x > maxv ->
        let m = 1 <<< n
        (x - m)
    | x -> x
