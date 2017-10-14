(*
 *    Copyright 2014 Anthony Perez (@amazingant)
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *)

module BIni

open 
type INIContents = (string * (string * string) list) list

type INIFile =
    | Error of string
    | Values of INIContents

module INI =
    let parse (ini: string) : INIFile =
        let clearNone v = List.fold (fun x y -> match y with None -> x | Some(z) -> z :: x) [] v
        let escape =
            pchar '\\' >>.
            anyOf "\"\\/trn" |>>
                function
                | 't' -> '\t'
                | 'r' -> '\r'
                | 'n' -> '\n'
                |  c  ->   c
        let valueParser: Parser<(string * string) option, unit> = parse {
            do! spaces
            let! comment = opt <| skipChar '#'
            match comment with
            | Some(_) ->
                do! skipRestOfLine true
                return None
            | None ->
                let! name = many1 (noneOf [' ';'[';']';'=']) .>> spaces .>> skipChar '='
                let! value = (many <| skipChar ' ') >>. many (noneOf ['\n';'\\'] <|> escape)
                do! skipRestOfLine true
                let name = String.Concat(name)
                let value = String.Concat(value).Trim()
                return Some(name, value)
        }
        let sectionParser: Parser<(string * (string * string) list) option, unit> = parse {
            do! spaces
            let! comment = opt <| skipChar '#'
            match comment with
            | Some(_) ->
                do! skipRestOfLine true
                return None
            | None ->
                let! section = spaces >>. skipChar '[' >>. many1 (noneOf [' ';'[';']']) .>> skipChar ']' .>> skipRestOfLine true
                let! values = many valueParser
                let section = String.Concat(section)
                let values = clearNone values
                return Some(String.Concat(section), values)
        }
        match run (many sectionParser) ini with
        | Success (v, _, _) -> Values(clearNone v)
        | Failure (msg, err, _) -> Error(String.Format("There was a problem parsing the settings file:\n{0}", msg))