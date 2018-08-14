module Ini

open System
open System.IO
open IniParser

[<Literal>]
let CFG_PATH = @".config\bakeit.cfg"

[<Literal>]
let HOME = "USERPROFILE"

// The definition of Result in FSharp.Core
[<StructuralEquality; StructuralComparison>]
[<CompiledName("FSharpResult`2")>]
[<Struct>]
type Result<'T,'TError> = 
    | Ok of ResultValue:'T 
    | Error of ErrorValue:'TError

let home_dir =
    match Environment.GetEnvironmentVariable HOME with
    | null -> Error (sprintf "No %s environment variable!" HOME)
    | dirname -> Ok dirname


let get_section section (config : INIContents) =
    match List.tryFind (fun (sec_key, _sec_vals) -> sec_key.Equals(section)) config with
    | Some (_sec_key, secdata) -> secdata
    | None -> let h = match home_dir with
                      | Ok h' -> h'
                      | Error _ -> "(unknown)"
              failwithf @"[%s] section not found. Please add a [%s] section to the %s\%s file and try again."
                        section section h CFG_PATH

let get_val (key: string) (section: string) (secdata : (string * string) list) =
    match List.tryFind (fun (k, _v) -> k.Equals(key)) secdata with
    | Some (_k, v) -> v
    | None ->
        failwithf @"No %s entry found. Please add an entry for %s to the [%s] section with your API key in it. You can find the latter on your account page on https://www.pastery.net."
                  key key section

//--------------------------------------------------------------------
let load_from_string ini_text =
    match parse ini_text with
    | INIFile.Values ini_data -> ini_data
    | INIFile.Error _msg ->
        failwithf @"Config file not found. Make sure you have a config file
                   at ~/%s with a [pastery] section containing
                   your Pastery API key, which you can get from your
                   https://www.pastery.net account page." CFG_PATH
//--------------------------------------------------------------------
let read () =
    match home_dir with
    | Ok dir ->   
        System.IO.File.ReadAllText(Path.Combine(dir, CFG_PATH)).Trim()
        |> load_from_string
        |> get_section "pastery"
        |> get_val "api_key" "pastery"
    | Error msg -> failwith msg

//--------------------------------------------------------------------

  
//--------------------------------------------------------------------
