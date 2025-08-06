module JsonStuff

open System.IO
open Newtonsoft.Json

let load_weapons (file_name: string): Weapon list = 
    let json = File.ReadAllText(file_name)
    JsonConvert.DeserializeObject<Weapon list>(json)

let weapons = load_weapons "weapons.json"

let get_weapons_by_name name =
    weapons |> List.tryFind (fun w -> w.Name = name)
