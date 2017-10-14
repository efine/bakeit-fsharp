module Ini

open System
open System.IO

[<Literal>]
let CFG_PATH = ".config/bakeit.cfg"

//--------------------------------------------------------------------

let read =
    (*
    ConfPath = Path.Combine home_dir CFG_PATH
    Config = load_from_file(ConfPath)
    Section = section(pastery, Config)
    get(api_key, Section, pastery)
    Some(api_key)
    *)
    0

//--------------------------------------------------------------------

// The definition of Result in FSharp.Core
[<StructuralEquality; StructuralComparison>]
[<CompiledName("FSharpResult`2")>]
[<Struct>]
type Result<'T,'TError> = 
    | Ok of ResultValue:'T 
    | Error of ErrorValue:'TError

let home_dir =
    match Environment.GetEnvironmentVariable "HOME" with
    | null -> Error "No HOME environment variable!"
    | dirname -> Ok dirname
  
//--------------------------------------------------------------------
(*
let load_from_file filename =
    match INI:parse filename with
    | 
        {ok, Config} ->
            Config;
        _Err ->
            Msg = ["Config file not found. Make sure you have a config file "
                   "at ~/", ?CFG_PATH, " with a [pastery] section containing "
                   "your Pastery API key, which you can get from your "
                   "https://www.pastery.net account page."],
            throw({error, list_to_binary(Msg)})
    end.

%%--------------------------------------------------------------------
section(Section, Config) ->
    case lists:keysearch(Section, 1, Config) of
        {value, {_, Props}} ->
            Props;
        _ ->
            Msg = io_lib:format(
                    "[~p] section not found. Please add a [~p] section to the "
                    "~s/~s file and try again.",
                    [Section, Section, "~", ?CFG_PATH]
                   ),
            throw({error, list_to_binary(Msg)})
    end.

%%--------------------------------------------------------------------
get(Key, Section, SectionName) ->
    case lists:keysearch(Key, 1, Section) of
        {value, {_, Val}} ->
            bakeit_util:to_s(Val);
        _ ->
            Msg = io_lib:format(
                    "No ~p entry found. Please add an entry for ~p to the "
                    "[~p] section with your API key in it. You can find the "
                    "latter on your account page on https://www.pastery.net.",
                    [Key, Key, SectionName]
                   ),
            throw({error, list_to_binary(Msg)})
    end.

    *)