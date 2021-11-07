module CPU

open Decoder

let twosC n bits =
    let maxv = (1 <<< (n - 1)) - 1

    match bits with
    | x when x > maxv ->
        let m = 1 <<< n
        x - m
    | x -> x

let rec mainLoop program (pc: int ref) (reg: int array) =
    match pc with
    | _ when (pc.Value >>> 2) = List.length program -> printf "Program executed"
    | _ ->

        printf "PC: %u \t\t" pc.Value

        let index = (pc.Value >>> 2)
        let instr = program.[index]
        let opcode = decodeOpcode instr
        let funct3 = instr >>> 12 &&& 0x7u
        let funct7 = instr >>> 25
        let rd = int (instr >>> 7 &&& 0x1fu)
        let rs1 = int (instr >>> 15 &&& 0x1fu)
        let rs2 = int (instr >>> 20 &&& 0x1fu)
        let shamt = instr >>> 20 &&& 0x1fu
        let immI = twosC 12 (int (instr >>> 20 &&& 0xFFFu))
        let immU = int (instr >>> 12) <<< 12

        let immS =
            let i1 = instr >>> 7 &&& 0x1fu
            let i2 = instr >>> 25
            twosC 12 (int (i2 <<< 5 ||| i1))

        let immB =
            let b11 = instr >>> 7 &&& 0x1u
            let b41 = instr >>> 8 &&& 0xfu
            let b105 = instr >>> 25 &&& 0x3fu
            let b12 = instr >>> 31 &&& 0x1u
            twosC 12 (int (((b12 <<< 1 ||| b11) <<< 6 ||| b105) <<< 4 ||| b41))

        let immJ =
            let b1912 = instr >>> 12 &&& 0xffu
            let b11 = instr >>> 20 &&& 0x1u
            let b101 = instr >>> 21 &&& 0x3ffu
            let b20 = instr >>> 31

            twosC
                20
                (int (
                    ((b20 <<< 8 ||| b1912) <<< 1 ||| b11) <<< 10
                    ||| b101
                ))

        match opcode with
        | 0x37u ->
            printf "LUI x%i %i" rd immU
            reg.[int rd] <- immU

        | 0x17u ->
            printf "AUIPC x%i %i" rd immU
            pc := (pc.Value + int immU - 4) // TODO: This casting might cause problems

        | 0x6fu -> printf "JAL x%i %i" rd immJ

        | 0x67u -> printf "JALR x%i x%i %i" rd rs1 immI

        | 0x63u -> // B-type
            match funct3 with
            | 0b000u -> printf "BEQ x%i x%i %i" rs1 rs2 immB

            | 0b001u -> printf "BNE x%i x%i %i" rs1 rs2 immB

            | 0b100u -> printf "BLT x%i x%i %i" rs1 rs2 immB

            | 0b101u -> printf "BGE x%i x%i %i" rs1 rs2 immB

            | 0b110u -> printf "BLTU x%i x%i %i" rs1 rs2 immB

            | 0b111u -> printf "BGEU x%i x%i %i" rs1 rs2 immB

            | _ -> printf "B-type"

        | 0x03u -> // I-type load
            match funct3 with
            | 0b000u -> printf "LB x%i x%i %i" rd rs1 immI

            | 0b001u -> printf "LH x%i x%i %i" rd rs1 immI

            | 0b010u -> printf "LW x%i x%i %i" rd rs1 immI

            | 0b100u -> printf "LBU x%i x%i %i" rd rs1 immI

            | 0b101u -> printf "LHU x%i x%i %i" rd rs1 immI

            | _ -> printf "I-type load"

        | 0x23u -> // S-type
            match funct3 with
            | 0b000u -> printf "SB x%i x%i %i" rs1 rs2 immS

            | 0b001u -> printf "SH x%i x%i %i" rs1 rs2 immS

            | 0b010u -> printf "SW x%i x%i %i" rs1 rs2 immS

            | _ -> printf "S-type"

        | 0x13u -> // I-type
            match funct3 with
            | 0b000u ->
                printf "ADDI x%i x%i %i" rd rs1 immI
                reg.[rd] <- (reg.[rs1] + int immI)

            | 0b010u ->
                printf "SLTI x%i x%i %i" rd rs1 immI
                reg.[rd] <- if reg.[rs1] < immI then 1 else 0

            | 0b011u ->
                printf "SLTIU x%i x%i %i" rd rs1 immI

                reg.[rd] <-
                    if reg.[rd] < twosC 12 (int immI) then
                        1
                    else
                        0

            | 0b100u ->
                printf "XORI x%i x%i %i" rd rs1 immI
                reg.[rd] <- reg.[rs1] ^^^ int immI

            | 0b110u -> printf "ORI x%i x%i %i" rd rs1 immI

            | 0b111u -> printf "ANDI x%i x%i %i" rd rs1 immI

            | 0b001u -> printf "SLLI x%i x%i %i" rd rs1 rs2

            | 0b101u ->
                match funct7 with
                | 0b0u -> printf "SRLI x%i x%i %u" rd rs1 shamt

                | 0b0100000u -> printf "SRAI x%i x%i %u" rd rs1 shamt

                | _ -> failwith "Invalid funct7 for SRLI/SRAI"

            | _ -> printf "I-type"

        | 0x33u -> // R-type
            match funct3 with

            | 0b000u ->
                match funct7 with
                | 0u ->
                    printf "ADD x%i x%i x%i" rd rs1 rs2
                    reg.[rd] <- reg.[rs1] + reg.[rs2]
                | 0b0100000u ->
                    printf "SUB x%i x%i x%i" rd rs1 rs2
                    reg.[rd] <- reg.[rs1] - reg.[rs2]
                | _ -> failwith "Invalid funct7 for ADD/SUB"

            | 0b001u -> printf "SLL x%i x%i x%i" rd rs1 rs2

            | 0b010u -> printf "SLT x%i x%i x%i" rd rs1 rs2

            | 0b011u -> printf "SLTU x%i x%i x%i" rd rs1 rs2

            | 0b100u -> printf "XOR x%i x%i x%i" rd rs1 rs2

            | 0b101u ->
                match funct7 with
                | 0b0u -> printf "SRL x%i x%i x%i" rd rs1 rs2

                | 0b0100000u -> printf "SRA x%i x%i x%i" rd rs1 rs2

                | _ -> failwith "Invalid funct7 for SRL/SRA"

            | 0b110u -> printf "OR x%i x%i x%i" rd rs1 rs2

            | 0b111u -> printf "AND x%i x%i x%i" rd rs1 rs2

            | _ -> printf "R-type"

        | 0x73u -> printf "ecall"

        | _ -> printf "??: %u" opcode

        // Print contents of registers
        printf "\nReg: "
        reg |> Array.iter (printf "%i ")
        printf "\n\n"

        pc := pc.Value + 4
        mainLoop program pc reg
