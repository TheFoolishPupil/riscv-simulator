module Decoder

// Helper function for handling sign extended numbers.
// Very possible its not a good solution and it should be rethinked.
let twosC n bits =
    let maxv = (1 <<< (n - 1)) - 1

    match bits with
    | x when x > maxv ->
        let m = 1 <<< n
        x - m
    | x -> x

let decodeOpcode =
    function
    | i -> i &&& 0x7fu

let decodeFunct3 =
    function
    | i -> i >>> 12 &&& 0x7u

let decodeFunct7 =
    function
    | (i: uint32) -> i >>> 25 // Masking might be needed here, in which case the typing would not be

let decodeRd =
    function
    | i -> int (i >>> 7 &&& 0x1fu)

let decodeRs1 =
    function
    | i -> int (i >>> 15 &&& 0x1fu)

let decodeRs2 =
    function
    | i -> int (i >>> 20 &&& 0x1fu)

let decodeShamt =
    function
    | i -> int (i >>> 20 &&& 0x1fu)

let decodeImmI =
    function
    | i -> twosC 12 (int (i >>> 20 &&& 0xFFFu))

let decodeImmU =
    function
    | (i: uint32) -> int (i >>> 12) <<< 12

let decodeImmS =
    function
    | i ->
        let i1 = i >>> 7 &&& 0x1fu
        let i2 = i >>> 25
        twosC 12 (int (i2 <<< 5 ||| i1))

let decodeImmB =
    function
    | i ->
        let b11 = i >>> 7 &&& 0x1u
        let b41 = i >>> 8 &&& 0xfu
        let b105 = i >>> 25 &&& 0x3fu
        let b12 = i >>> 31 &&& 0x1u

        twosC 12 (int (((b12 <<< 1 ||| b11) <<< 6 ||| b105) <<< 4 ||| b41))
        <<< 1 // TODO: Why do we need to multiply by two here???

let decodeImmJ =
    function
    | i ->
        let b1912 = i >>> 12 &&& 0xffu
        let b11 = i >>> 20 &&& 0x1u
        let b101 = i >>> 21 &&& 0x3ffu
        let b20 = i >>> 31

        twosC
            20
            (int (
                ((b20 <<< 8 ||| b1912) <<< 1 ||| b11) <<< 10
                ||| b101
            ))
