module Bakeit

open System
open GetOpts
open Upload
open WebBrowser
open System.Reflection

let optAsStr (opts: Opts) key = string <| (opts.Item(key)).Value
let optAsUInt (opts: Opts) key = uint32 <| (opts.Item(key)).AsInt
let optAsList (opts: Opts) key = (opts.Item(key)).AsList
let optAsBool (opts: Opts) key =
    match opts.Item(key) with
    | x when x.IsTrue -> true
    | x when x.IsFalse -> false
    | x -> failwithf "%s is not a bool: %A" key x

let run argv =
    let api_key = Ini.read()
    let opts = getopts argv
    opts |> Map.iter (fun k v -> printfn "%s : %A" k v)

    let file_name () = optAsStr opts "<filename>"

    let read_data f =
        if f = "-" then 
            stdin.ReadToEnd()
        else
            System.IO.File.ReadAllText(f)

    let title = match optAsStr opts "--title" with
                | "" | "false"  -> file_name()
                | str -> str

    let cfg = { Upload.defaultCfg with
                    Data = read_data <| file_name ();
                    ApiKey = api_key;
                    Title = title;   
                    Language = optAsStr opts "--language";
                    Duration = optAsUInt opts "--duration";
                    MaxViews = optAsUInt opts "--max-views";
                    OpenBrowser = optAsBool opts "--open-browser" }

    let response = upload cfg

    printfn "%s" response.Url
    if cfg.OpenBrowser then
        open_url response.Url

    0


[<EntryPoint>]
let main argv =
    try
        run argv
    with
    | exc ->
        printfn "%s" exc.Message
        printf "%s" exc.StackTrace
        1
