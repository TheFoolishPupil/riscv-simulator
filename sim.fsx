open System
open System.IO

let twosC n bits =
    let maxv = (1 <<< (n - 1)) - 1

    match bits with
    | x when x > maxv ->
        let m = 1 <<< n
        (x - m)
    | x -> x

let f = File.ReadAllBytes("tests/addlarge.bin")

let program =
    [| 0 .. 4 .. Array.length f - 1 |]
    |> Array.map (fun i -> BitConverter.ToUInt32(f, i))
    |> Array.toList

let reg: int array = Array.zeroCreate 32

let mutable pc = 0

let rec mainLoop program =
    match pc with
    | _ when (pc >>> 2) = List.length program -> printf "Program executed"
    | _ ->

        printf "PC: %u  " pc

        let index = (pc >>> 2)
        let instr = program.[index]
        let opcode = instr &&& 0x7fu
        let funct3 = instr >>> 12 &&& 0x7u
        let funct7 = instr >>> 25
        let rd = instr >>> 7 &&& 0x1fu
        let rs1 = instr >>> 15 &&& 0x1fu
        let rs2 = instr >>> 20 &&& 0x1fu
        let immI = instr >>> 20 &&& 0xFFFu
        let immU = instr >>> 12

        let immS =
            let i1 = instr >>> 7 &&& 0x1fu
            let i2 = instr >>> 25
            i2 <<< 5 ||| i1

        let immB =
            let b11 = instr >>> 7 &&& 0x1u
            let b41 = instr >>> 8 &&& 0xfu
            let b105 = instr >>> 25 &&& 0x3fu
            let b12 = instr >>> 31 &&& 0x1u
            ((b12 <<< 1 ||| b11) <<< 6 ||| b105) <<< 4 ||| b41

        let immJ =
            let b1912 = instr >>> 12 &&& 0xffu
            let b11 = instr >>> 20 &&& 0x1u
            let b101 = instr >>> 21 &&& 0x3ffu
            let b20 = instr >>> 31

            ((b20 <<< 8 ||| b1912) <<< 1 ||| b11) <<< 10
            ||| b101

        match opcode with
        | 0x37u ->
            printf "LUI\n"
            reg.[int rd] <- int (immU <<< 12)

        | 0x17u ->
            printf "AUIPC\n"
            pc <- (pc + int immU - 4) // TODO: This casting might cause problems

        | 0x6fu -> printf "JAL\n"

        | 0x67u -> printf "JALR\n"

        | 0x63u -> printf "B-type\n"

        | 0x03u -> printf "I-type Load\n"

        | 0x23u -> printf "S-type\n"

        | 0x13u ->
            printf "I-type Arithmetic %i\n" (twosC 12 (int immI))
            reg.[int rd] <- (reg.[int rs1] + twosC 12 (int immI))

        | 0x33u -> printf "R-type\n"

        | 0x73u -> printf "ecall\n"

        | _ -> printf "Unsupported instruction: %u\n" opcode

        // Print contents of registers
        reg |> Array.iter (printf "%i ")
        printf "\n\n"


        pc <- (pc + 4)
        mainLoop program

mainLoop program


let result: byte array =
    Array.zeroCreate ((Array.length reg) * 4)

Buffer.BlockCopy(reg, 0, result, 0, Array.length result)

File.WriteAllBytes("tests/result.bin", result)
