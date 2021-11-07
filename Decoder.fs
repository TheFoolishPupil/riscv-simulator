module Decoder

let decodeOpcode =
    function
    | i -> i &&& 0x7fu

let decodeFunct3 =
    function
    | i -> i >>> 12 &&& 0x7u
