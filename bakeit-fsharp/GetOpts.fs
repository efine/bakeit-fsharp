module GetOpts

open DocoptNet

type Opts = Map<string, ValueObject>

let getopts (argv : string[]) : Opts =
    let usage = @"Bakeit.

Usage:
  bakeit.exe [options] <filename>
    
Options:
  -h, --help                  Show this help.
  -t <t>, --title=<t>         The title of the paste.
  -l <l>, --language=<l>      The language highlighter to use.
  -d <d>, --duration=<d>      The duration (in minutes) before the paste expires.
  -v <v>, --max-views=<v>     How many times this paste can be viewed before it expires.
  -b, --open-browser          Automatically open a browser window when done.
  -V, --version               Show the version and quit.

"
    let docopt = new DocoptNet.Docopt()
    let opts = docopt.Apply (usage, argv, help=true, exit=true)
    let folder (m : Map<string, ValueObject>) (KeyValue(k, v)) = m.Add (k, v)
    Seq.fold folder Map.empty opts