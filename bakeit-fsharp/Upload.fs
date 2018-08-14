module Upload

    open System
    open FSharp.Data
    open FSharp.Data.HttpRequestHeaders

    [<Struct>]
    type Cfg = {
        Data        : string
        ApiKey      : string
        Title       : string
        Language    : string
        Duration    : uint32
        MaxViews    : uint32
        OpenBrowser : bool
        GetPastes   : bool
    }
    
    type PasteryPaste =
        JsonProvider<""" {"id": "xyzzy",
                          "title": "",
                          "url": "https://www.pastery.net/xyzzy/",
                          "language":
                          "text",
                          "duration": 1439} """, InferTypesFromValues = true>

    type PasteryPastes =
        JsonProvider<""" {"pastes": [{"id": "bxbdaz", "title": "",
                                      "url": "https://www.pastery.net/bxbdaz/",
                                      "language": "text",
                                      "duration": 1439},
                                     {"id": "jgmmxy", "title": "",
                                      "url": "https://www.pastery.net/jgmmxy/",
                                      "language": "text",
                                      "duration": 1437}
                                    ]} """, InferTypesFromValues = true>

    type PasteryError =
        JsonProvider<""" {"result": "error",
                          "error_msg": "Missing keys: 'api_key'"}
                     """, InferTypesFromValues = true>

    let defaultCfg = { Data = "";
                       ApiKey = ""; Title = ""; Language = "";
                       Duration = 60u; MaxViews = 0u;
                       OpenBrowser = false; GetPastes = false }

    let strToBytes (s : string) = Text.Encoding.UTF8.GetBytes s

    let bodyAsString body =
        match body with
            | Binary b -> string b
            | Text t -> t

    let get_error_msg body =
        let err = PasteryError.Parse(body |> bodyAsString)
        err.Result + ": " + err.ErrorMsg

    [<Literal>] 
    let PASTERY_URL = @"https://www.pastery.net/api/paste/"

    let err_too_big () =
        failwith ("The chosen file was rejected by the server because it " +
                  "was too large, please try a smaller file.")

    let err_server_5xx n =
        failwithf "There was a server error %d, please try again later." n
    
    let err_4xx n =
        failwithf "There was a %d error due to the request" n

    let err_unexpected_redirect n =
        failwithf "Unexpected redirect %d" n

    let handle_upload_response response =
        PasteryPaste.Parse(response.Body |> bodyAsString).Url

    let handle_get_paste_response response =
        PasteryPastes.Parse(response.Body |> bodyAsString).JsonValue.ToString()

    let get_pastes cfg =
        let response = Http.Request(PASTERY_URL, httpMethod = "GET",
                                    silentHttpErrors = true,
                                    query = ["api_key", cfg.ApiKey],
                                    headers = [Accept HttpContentTypes.Json])
        
        match response.StatusCode with
        | n when n >= 200 && n < 300 -> handle_get_paste_response response
        | n when n >= 300 && n < 400 -> err_unexpected_redirect n
        | 422                        -> failwithf "%s" <| get_error_msg response.Body
        | n when n >= 500            -> failwithf "server error %A" n
        | n                          -> failwithf "unexpected server response %A" n


    let upload cfg =
        let response = Http.Request(PASTERY_URL, silentHttpErrors = true,
                                    query = ["api_key", cfg.ApiKey;
                                             "title", cfg.Title;
                                             "language", cfg.Language;
                                             "duration", cfg.Duration.ToString();
                                             "max_views", cfg.MaxViews.ToString()],
                                    headers = [Accept HttpContentTypes.Json],
                                    body = BinaryUpload (strToBytes cfg.Data))            

        match response.StatusCode with
        | n when n >= 200 && n < 300 -> handle_upload_response response
        | n when n >= 300 && n < 400 -> err_unexpected_redirect n
        | 413                        -> err_too_big ()
        | 422                        -> failwithf "%s" <| get_error_msg response.Body
        | n when n >= 400 && n < 500 -> err_4xx n
        | n when n >= 500            -> err_server_5xx n
        | n                          -> failwithf "Unexpected server status code %d" n
