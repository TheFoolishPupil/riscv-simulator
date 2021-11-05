open System
open System.IO

// Binary representation of int
Convert.ToString(7, 2)

// IO
let reg: int array = [| 0; 500; 600; 700; 0 |]

let result: byte array =
    Array.zeroCreate ((Array.length reg) * 4)

Buffer.BlockCopy(reg, 0, result, 0, Array.length result)

File.WriteAllBytes("tests/test_write.bin", result)

let f =
    File.ReadAllBytes("tests/test_write.bin")

let program =
    [| 0 .. 4 .. Array.length f - 1 |]
    |> Array.map (fun i -> BitConverter.ToInt32(f, i))
    |> Array.toList
