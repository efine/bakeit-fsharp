module Upload

    open System
    open FSharp.Data
    open FSharp.Data.HttpRequestHeaders

    type Default<'a> =
    | Some of 'a
    | Default of 'a

    [<Struct>]
    type Cfg = {
        Data        : string
        ApiKey      : string
        Title       : string
        Language    : string
        Duration    : uint32
        MaxViews    : uint32
        OpenBrowser : bool
    }
    
    type PasteryError = JsonProvider<""" {"result": "error", "error_msg": "Missing keys: 'api_key'"} """, InferTypesFromValues = true>
    type PasteryPaste = JsonProvider<""" {"id": "xyzzy", "title": "", "url": "https://www.pastery.net/xyzzy/", "language": "text", "duration": 1439} """>
    type PasteryPastes = JsonProvider<""" {"pastes": [{"id": "bxbdaz", "title": "", "url": "https://www.pastery.net/bxbdaz/", "language": "text", "duration": 1439},
                                          {"id": "jgmmxy", "title": "", "url": "https://www.pastery.net/jgmmxy/", "language": "text", "duration": 1437}]} """>

    let defaultCfg = { Data = ""; ApiKey = ""; Title = ""; Language = ""; Duration = 60u; MaxViews = 0u; OpenBrowser = false }
    let strToBytes (s : string) = Text.Encoding.UTF8.GetBytes s

    let bodyAsString body =
        match body with
            | Binary b -> string b
            | Text t -> t

    let get_error_msg body =
        let err = PasteryError.Parse(body |> bodyAsString)
        sprintf "%s: %s" err.Result err.ErrorMsg

    let PASTERY_URL = @"https://www.pastery.net/api/paste/"

    let upload cfg =
        let response = Http.Request(PASTERY_URL, silentHttpErrors = true,
                                    query = ["api_key", cfg.ApiKey; "title", cfg.Title; "language", cfg.Language; "duration", cfg.Duration.ToString(); "max_views", cfg.MaxViews.ToString()],
                                    headers = [Accept HttpContentTypes.Json],
                                    body = BinaryUpload (strToBytes cfg.Data))
        
        match response.StatusCode with
        | n when n >= 300 && n < 400 -> failwithf "Unexpected redirect %d" n
        | 413                        -> failwithf "The chosen file was rejected by the server because it was too large, please try a smaller file."
        | 422                        -> failwithf "%s" <| get_error_msg response.Body
        | n when n >= 400 && n < 500 -> failwithf "There was a %d error due to the request" n
        | n when n >= 500            -> failwithf "There was a server error %d, please try again later." n
        | _                          -> PasteryPaste.Parse(response.Body |> bodyAsString)

    