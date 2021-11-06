open System
open CPU
open IO

[<EntryPoint>]
let main args =

    if Array.length args <= 1 then
        failwith "No program provided."

    let programPath = args.[1]
    let resultsPath = args.[2]

    let program = loadProgram (programPath)
    let pc = ref 0
    let reg: int array = Array.zeroCreate 32

    mainLoop program pc reg

    saveProgram resultsPath reg

    0 // Return integer
