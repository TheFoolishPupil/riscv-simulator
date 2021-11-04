open System
open System.IO


let f = File.ReadAllBytes("tests/addlarge.bin")

let program =
    [| 0 .. 4 .. Array.length f - 1 |]
    |> Array.map (fun i -> BitConverter.ToInt32(f, i))
    |> Array.toList

let reg: int array = Array.zeroCreate 32

let rec mainLoop program pc =
    match pc with
    | _ when int (pc >>> 2) = List.length program -> printf "Program executed"
    | _ ->

        let index = (pc >>> 2)
        let instr = program.[int index]
        let opcode = instr &&& 0x7f
        let funct3 = instr >>> 12 &&& 0x7
        let funct7 = instr >>> 25
        let rd = instr >>> 7 &&& 0x1f
        let rs1 = instr >>> 15 &&& 0x1f
        let rs2 = instr >>> 20 &&& 0x1f
        let immI = instr >>> 20
        let immU = instr >>> 12

        let immS =
            let i1 = instr >>> 7 &&& 0x1f
            let i2 = instr >>> 25
            i2 <<< 5 ||| i1

        let immB =
            let b11 = instr >>> 7 &&& 0x1
            let b41 = instr >>> 8 &&& 0xf
            let b105 = instr >>> 25 &&& 0x3f
            let b12 = instr >>> 31
            ((b12 <<< 1 ||| b11) <<< 6 ||| b105) <<< 4 ||| b41

        let immJ =
            let b1912 = instr >>> 12 &&& 0xff
            let b11 = instr >>> 20 &&& 0x1
            let b101 = instr >>> 21 &&& 0x3ff
            let b20 = instr >>> 31

            ((b20 <<< 8 ||| b1912) <<< 1 ||| b11) <<< 10
            ||| b101

        match opcode with
        | 0x37 -> printf "LUI\n"

        | 0x17 -> printf "AUIPC\n"

        | 0x6f -> printf "JAL\n"

        | 0x67 -> printf "JALR\n"

        | 0x63 -> printf "B-type\n"

        | 0x03 -> printf "I-type Load\n"

        | 0x23 -> printf "S-type\n"

        | 0x13 ->
            printf "I-type Arithmetic\n"
            reg.[rd] <- (reg.[rs1] + immI)

        | 0x33 -> printf "R-type\n"

        | 0x73 -> printf "ecall\n"

        | _ -> failwith ("Unsupported instruction: " + (string opcode))

        // Print contents of registers
        reg |> Array.iter (printf "%u ")
        printf "\n\n"

        mainLoop program (pc + 4)

mainLoop program 0
