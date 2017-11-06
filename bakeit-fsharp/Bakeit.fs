module Bakeit

open System
open GetOpts
open Upload
open WebBrowser

let optItem (opts: Opts) key = opts.Item(key)

let toString (item : DocoptNet.ValueObject) = string item.Value

let toUInt (item : DocoptNet.ValueObject) = uint32 item.AsInt

let toList (item : DocoptNet.ValueObject) = item.AsList

let toBool (item : DocoptNet.ValueObject) =
    match item with
    | _ when item.IsTrue -> true
    | _ when item.IsFalse -> false
    | _ -> failwithf "Not a bool: %A" item        

let run argv =
    let debug = false
    let api_key = Ini.read()
    let opts = getopts argv
    if debug then opts |> Map.iter (fun k v -> printfn "%s : %A" k v)

    let lookup = optItem opts

    let file_name () = "<filename>" |> lookup |> toString

    let read_data f =
        match f with
        | "-" -> stdin.ReadToEnd()
        | _ -> System.IO.File.ReadAllText(f)

    let title () = match "--title" |> lookup |> toString with
                   | "" | "false"  -> file_name.ToString()
                   | str -> str

    let make_cfg () =
        if "--get-pastes" |> lookup |> toBool then
            { Upload.defaultCfg with
                    GetPastes = true;
                    ApiKey    = api_key
           }
        else
            { Upload.defaultCfg with
                    Data        = read_data <| file_name ();
                    ApiKey      = api_key;
                    Title       = title ();   
                    Language    = "--language" |> lookup |> toString;
                    Duration    = "--duration" |> lookup |> toUInt;
                    MaxViews    = "--max-views" |> lookup |> toUInt;
                    OpenBrowser = "--open-browser" |> lookup |> toBool
            }



    let cfg = make_cfg ()
    if cfg.GetPastes then
        printfn "%s" <| get_pastes cfg
    else
        let url = upload cfg
        printfn "%s" url
        if cfg.OpenBrowser then
            open_url url

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
