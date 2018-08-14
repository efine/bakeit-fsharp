module WebBrowser

open OsDetection

type Browser =
             | Default
             | Firefox
             | InternetExplorer
             | Chrome
             | Opera
             | Safari

type BrowserApp =
    | Default of string
    | App of string
    | UnsupportedOS of OS
    | UnsupportedBrowser of Browser

let exec cmd =
     System.Diagnostics.Process.Start("cmd", cmd) |> ignore

let open_url url =
    exec <| "/c start " + url

let open_url_with browser url =
    match browser with
    | Safari -> "/c start safari " + url
    | _ -> failwithf "Unsupported browser %A" browser