module Bakeit

open GetOpts

[<EntryPoint>]
let main argv = 
    let opts = getopts argv
    opts |> Map.iter (fun k v -> printfn "%s : %A" k v)
    0