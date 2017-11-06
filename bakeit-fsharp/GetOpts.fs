module GetOpts

[<Literal>]
let version = "0.1"

open DocoptNet

type Opts = Map<string, ValueObject>

let getopts (argv : string[]) : Opts =
    let usage = @"Bakeit.

Usage:
  bakeit.exe -g
  bakeit.exe [options] <filename>
    
Options:
  -h, --help                  Show this help.
  -g, --get-pastes            Get JSON describing all pastes [default: false].
  -t <t>, --title=<t>         The title of the paste [default: false].
  -l <l>, --language=<l>      The language highlighter to use [default: text].
  -d <d>, --duration=<d>      The duration (in minutes) before the paste expires [default: 1440].
  -v <v>, --max-views=<v>     How many times this paste can be viewed before it expires [default: 20].
  -b, --open-browser          Automatically open a browser window when done [default: false].
  -V, --version               Show the version and quit.

"
    let docopt = new DocoptNet.Docopt()
    let opts = docopt.Apply (usage, argv, help=true, exit=true)
    let folder (m : Map<string, ValueObject>) (KeyValue(k, v)) = m.Add (k, v)
    Seq.fold folder Map.empty opts