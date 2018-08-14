module OsDetection

open System

type OS =
        | MacOS            
        | Windows
        | Linux

let getOS =
    match int Environment.OSVersion.Platform with
    | 4 | 128 -> Linux
    | 6       -> MacOS
    | _       -> Windows