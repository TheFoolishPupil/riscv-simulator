open System
open CPU
open IO

[<EntryPoint>]
let main args =

    if Array.isEmpty args then
        failwith "No program provided."

    let programPath = args.[0]

    let program = loadProgram (programPath)
    let pc = ref 0
    let reg: int array = Array.zeroCreate 32

    mainLoop program pc reg

    saveResult programPath reg


    0 // Return integer
