// custom modules
open dice
open JsonStuff
open Setup
open Types

open System.IO
open System.Runtime.InteropServices
open System
open System.Net.Http

let rec print_weapons weapons index =
    match weapons with
    | [] -> ()
    | head :: tail ->
        printfn "%d: %s" index head.Name
        print_weapons tail (index + 1)

let deal_damage (damage_dice: string) : int =
    match damage_dice with
    | "d4" -> roll 4 None None
    | "d6" -> roll 6 None None
    | "d8" -> roll 8 None None
    | "d10" -> roll 10 None None
    | "d12" -> roll 12 None None
    | "d20" -> roll 20 None None
    | "d100" -> roll 100 None None
    | _ -> roll 0 None None

let rec attack (weapon: Weapon) (turn: int) (advantage: bool) (disadvantage: bool) =
    if turn = 0 then
        printfn "Attack is over"

    let dice_roll = roll 20 (Some advantage) (Some disadvantage)

    let critical_hit =
        match dice_roll with
        | 20 -> true
        | _ -> false

    let hitbonus = weapon.Hitbonus
    let roll_to_hit = dice_roll + hitbonus
    let double_attack = List.contains "Double Attack" weapon.Properties

    printf "roll: %d hit? [y/n]? " roll_to_hit
    let input = System.Console.ReadLine()
    printf "\n"

    let hit =
        match input with
        | "y" -> true
        | "n" -> false
        | _ ->
            printfn "Invalid input"
            attack weapon turn advantage disadvantage
            // Code won't reach here, dummy value
            false

    let rec damage (double_attack: bool): int =
        let hit_damage =
            match hit with 
            | true -> deal_damage weapon.Damage_Dice
            | false -> weapon.Damage_On_Miss

        match double_attack with 
        | true -> hit_damage + damage false        
        | false -> hit_damage

    let dealt_damage = damage double_attack

    printfn "you dealt %d" dealt_damage

    match critical_hit with
    | true -> attack weapon (turn + 1) advantage disadvantage
    | false -> attack weapon (turn - 1) advantage disadvantage

let rec combat_loop (weapons_list: Weapon list) turn =

    if turn = 0 then
        printf "Combat ended"

    printf "Please select a weapon from the list [1,2,..]"
    print_weapons weapons_list 1

    let input = Console.ReadLine()
    let int_input = Int32.Parse input

    let weapon = List.item int_input weapons_list

    let defalt_attacks = 1

    match input with
    | "whack" -> attack weapon defalt_attacks
    | _ ->
        printfn "invalid input try 'whack' or 'range'"
        combat_loop weapons_list turn




[<EntryPoint>]
let main args =

    clone_database()

    let os_name = get_operating_system ()
    let config_path = get_config_path os_name
    let weapons_list: Weapon List = load_weapons config_path

    let has_arg arg = Array.contains arg args
    let hasSurge = has_arg "surge"
    let hasHaste = has_arg "haste"

    let number_of_attacks =
        // change values so it actually alligns with the rules as written
        match hasSurge, hasHaste with
        | true, true -> 3
        | true, false -> 2
        | false, true -> 2
        | false, false -> 1

    printfn "Number of attacks: %d" number_of_attacks

    combat_loop weapons_list number_of_attacks

    0
