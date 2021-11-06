open System
open System.IO
open CPU

let f = File.ReadAllBytes("tests/addlarge.bin")

let program =
    [| 0 .. 4 .. Array.length f - 1 |]
    |> Array.map (fun i -> BitConverter.ToUInt32(f, i))
    |> Array.toList

let reg: int array = Array.zeroCreate 32

let pc = ref 0



[<EntryPoint>]
let main argv =

    mainLoop program pc reg

    let result: byte array =
        Array.zeroCreate ((Array.length reg) * 4)

    Buffer.BlockCopy(reg, 0, result, 0, Array.length result)
    File.WriteAllBytes("tests/result.bin", result)

    0
