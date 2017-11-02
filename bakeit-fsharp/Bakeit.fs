module Bakeit

open GetOpts
open Upload

#if REMOVE_THIS

type BakeItArg =
    | StrArg of string
    | UIntArg of uint32
    | BoolArg of bool
    | ListArg of seq<string>

let lookup (opts: Opts) key =
    match opts.Item(key) with
    | it when it.IsInt -> UIntArg (uint32 <| it.AsInt)
    | it when it.IsString -> StrArg (string <| it)
    | it when it.IsTrue -> BoolArg true
    | it when it.IsFalse -> BoolArg false
    | it when it.IsList -> ListArg (Seq.cast<string> <| it.AsList)
    | it -> failwithf "Unexpected option type for %A" it

 #endif

let optAsStr (opts: Opts) key = string <| (opts.Item(key)).Value
let optAsUInt (opts: Opts) key = uint32 <| (opts.Item(key)).AsInt
let optAsList (opts: Opts) key = (opts.Item(key)).AsList
let optAsBool (opts: Opts) key =
    match opts.Item(key) with
    | x when x.IsTrue -> true
    | x when x.IsFalse -> false
    | x -> failwithf "%s is not a bool: %A" key x

[<EntryPoint>]
let main argv =
    let api_key = Ini.read()
    let opts = getopts argv
    opts |> Map.iter (fun k v -> printfn "%s : %A" k v)

//    let file_name = optAsStr opts "<filename>"

    let cfg = { Upload.defaultCfg with
                    Data = "";
                    ApiKey = api_key;
                    Title = optAsStr opts "--title";
                    Language = optAsStr opts "--language";
                    Duration = optAsUInt opts "--duration";
                    MaxViews = optAsUInt opts "--max-views";
                    OpenBrowser = optAsBool opts "--open-browser" }

    printfn "cfg = %A" cfg
    System.Console.ReadLine() |> ignore
    0