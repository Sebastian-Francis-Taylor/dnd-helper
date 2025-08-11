module JsonStuff
open System.IO
open Newtonsoft.Json
open Types

let load_weapons (file_path: string) : Weapon list =
    let json = File.ReadAllText file_path
    JsonConvert.DeserializeObject<Weapon[]> json |> Array.toList

let weapons = load_weapons "weapons.json"
