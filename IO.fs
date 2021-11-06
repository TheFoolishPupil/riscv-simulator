module IO

open System
open System.IO

let loadProgram path =
    let f = File.ReadAllBytes(path)

    [| 0 .. 4 .. Array.length f - 1 |]
    |> Array.map (fun i -> BitConverter.ToUInt32(f, i))
    |> Array.toList

let saveProgram path reg =
    let result: byte array =
        Array.zeroCreate ((Array.length reg) * 4)

    Buffer.BlockCopy(reg, 0, result, 0, Array.length result)
    File.WriteAllBytes(path, result)
