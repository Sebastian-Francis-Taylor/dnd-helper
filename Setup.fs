module Setup

open System.IO
open System.Runtime.InteropServices
open System
open System.Net.Http

let get_operating_system () : string =
    if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
        "windows"
    elif RuntimeInformation.IsOSPlatform OSPlatform.OSX then
        "macos"
    elif RuntimeInformation.IsOSPlatform OSPlatform.Linux then
        "linux"
    else
        "other"

let get_config_path (os_name: string) : string =
    match os_name with
    | "windows" -> Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "dnd-helper")
    | "macos" ->
        Path.Combine(
            Environment.GetFolderPath Environment.SpecialFolder.UserProfile,
            "Library",
            "Application Support",
            "dnd-helper"
        )
    | "linux" -> Path.Combine(Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".config", "dnd-helper")
    | _ ->
        // Failsafe if os is super exotic (Looking at you templeOS)
        Path.Combine(".", "dnd-helper")

let get_weapons_path (os_name: string) : string =
    Path.Combine(get_config_path os_name, "weapons.json")

let download_json_from_github (raw_url: string) (save_path: string) =
    task {
        use client = new HttpClient()
        printfn "client defined"
        let! json = client.GetStringAsync raw_url
        printfn "string defined"
        File.WriteAllText(save_path, json)
        printfn "file written"
        printfn "Downloaded JSON to %s" save_path
    }

let clone_database () =
    try
        let weapons_url =
            "https://raw.githubusercontent.com/Sebastian-Francis-Taylor/dnd-helper/main/weapons.json"
        printfn "debug: weapons_url = %s" weapons_url

        let save_path =
            Path.Combine(get_config_path (get_operating_system ()), "weapons.json")
        printfn "debug: save_path = %s" save_path

        printfn "downloading from github"
        // issue is here
        download_json_from_github weapons_url save_path
        |> Async.AwaitTask
        |> Async.RunSynchronously

        printfn "Weapons database downloaded to %s" save_path
    with
    | :? HttpRequestException as ex -> printfn "Failed to download weapons database: %s" ex.Message
    | :? IOException as ex -> printfn "File system error: %s" ex.Message
    | ex -> printfn "Unexpected error: %s" ex.Message

let create_config_dir (os_name: string) : unit = 
    Directory.CreateDirectory(get_config_path os_name) |> ignore
