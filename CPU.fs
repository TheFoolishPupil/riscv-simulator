module CPU

open Decoder

let rec mainLoop program (pc: int ref) (reg: int array) =
    match pc with
    | _ when (pc.Value >>> 2) = List.length program -> printf "Program executed"
    | _ ->

        printf "PC: %u \t\t" pc.Value

        let index = (pc.Value >>> 2)
        let instr = program.[index]
        let opcode = decodeOpcode instr
        let funct3 = decodeFunct3 instr
        let funct7 = decodeFunct7 instr

        let rd = decodeRd instr
        let rs1 = decodeRs1 instr
        let rs2 = decodeRs2 instr

        let immI = decodeImmI instr


        match opcode with
        | 0x37u ->
            let immU = decodeImmU instr
            printf "LUI x%i %i" rd immU
            reg.[int rd] <- immU

        | 0x17u ->
            let immU = decodeImmU instr
            printf "AUIPC x%i %i" rd immU
            pc := (pc.Value + int immU - 4) // TODO: This casting might cause problems

        | 0x6fu ->
            let immJ = decodeImmJ instr
            printf "JAL x%i %i" rd immJ

        | 0x67u -> printf "JALR x%i x%i %i" rd rs1 immI


        | 0x63u -> // B-type
            let immB = decodeImmB instr

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
            let immS = decodeImmS instr

            match funct3 with
            | 0b000u -> printf "SB x%i x%i %i" rs1 rs2 immS

            | 0b001u -> printf "SH x%i x%i %i" rs1 rs2 immS

            | 0b010u -> printf "SW x%i x%i %i" rs1 rs2 immS

            | _ -> printf "S-type"


        | 0x13u -> // I-type
            let shamt = decodeShamt instr

            match funct3 with
            | 0b000u ->
                printf "ADDI x%i x%i %i" rd rs1 immI
                reg.[rd] <- (reg.[rs1] + int immI)

            | 0b010u ->
                printf "SLTI x%i x%i %i" rd rs1 immI
                reg.[rd] <- if reg.[rs1] < immI then 1 else 0

            | 0b011u ->
                printf "SLTIU x%i x%i %i" rd rs1 immI
                reg.[rd] <- if reg.[rd] < immI then 0 else 1

            | 0b100u ->
                printf "XORI x%i x%i %i" rd rs1 immI
                reg.[rd] <- reg.[rs1] ^^^ int immI

            | 0b110u ->
                printf "ORI x%i x%i %i" rd rs1 immI
                reg.[rd] <- reg.[rs1] ||| int immI

            | 0b111u ->
                printf "ANDI x%i x%i %i" rd rs1 immI
                reg.[rd] <- reg.[rs1] &&& immI

            | 0b001u ->
                printf "SLLI x%i x%i %i" rd rs1 rs2
                reg.[rd] <- reg.[rs1] <<< shamt

            | 0b101u ->
                match funct7 with
                | 0b0u ->
                    printf "SRLI x%i x%i %u" rd rs1 shamt
                    reg.[rd] <- int (uint32 reg.[rs1] >>> shamt)

                | 0b0100000u ->
                    printf "SRAI x%i x%i %u" rd rs1 shamt
                    reg.[rd] <- reg.[rs1] >>> shamt

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

            | 0b001u ->
                printf "SLL x%i x%i x%i" rd rs1 rs2
                reg.[rd] <- reg.[rs1] <<< reg.[rs2]

            // TODO: how to handle SLT vs SLTU
            | 0b010u ->
                printf "SLT x%i x%i x%i" rd rs1 rs2
                reg.[rd] <- if reg.[rs1] < reg.[rs2] then 1 else 0

            | 0b011u ->
                printf "SLTU x%i x%i x%i" rd rs1 rs2
                reg.[rd] <- if reg.[rs1] < reg.[rs2] then 1 else 0

            | 0b100u ->
                printf "XOR x%i x%i x%i" rd rs1 rs2
                reg.[rd] <- reg.[rs1] ^^^ reg.[rs2]

            | 0b101u ->
                match funct7 with
                | 0b0u ->
                    printf "SRL x%i x%i x%i" rd rs1 rs2
                    reg.[rd] <- int (uint32 reg.[rs1] >>> reg.[rs2])

                | 0b0100000u ->
                    printf "SRA x%i x%i x%i" rd rs1 rs2
                    reg.[rd] <- reg.[rs1] >>> reg.[rs2]

                | _ -> failwith "Invalid funct7 for SRL/SRA"

            | 0b110u ->
                printf "OR x%i x%i x%i" rd rs1 rs2
                reg.[rd] <- reg.[rs1] ||| reg.[rs2]
            | 0b111u ->
                printf "AND x%i x%i x%i" rd rs1 rs2
                reg.[rd] <- reg.[rs1] &&& reg.[rs2]

            | _ -> printf "R-type"


        | 0x73u -> printf "ecall"

        | _ -> printf "??: %u" opcode

        // Print contents of registers
        printf "\nReg: "
        reg |> Array.iter (printf "%i ")
        printf "\n\n"

        pc := pc.Value + 4
        mainLoop program pc reg
