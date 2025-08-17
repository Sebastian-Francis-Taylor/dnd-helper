// custom modules
open dice
open JsonStuff
open Setup
open Types

open System

let rec print_weapons weapons index: unit =
    match weapons with
    | [] -> ()
    | head :: tail ->
        printfn "%d: %s" index head.Name
        print_weapons tail (index + 1)

let rec deal_damage (damage_dice: string) (attacks_amount: int) (damage_dealt: int) : int =

    let split_dice = damage_dice.Split 'd'

    let temp_damage = 
        match split_dice[1] with
        | "4" -> roll 4 None None
        | "6" -> roll 6 None None
        | "8" -> roll 8 None None
        | "10" -> roll 10 None None
        | "12" -> roll 12 None None
        | "20" -> roll 20 None None
        | "100" -> roll 100 None None
        | _ -> 
            printfn "failed to parse damage_dice"
            0
    match attacks_amount with
    | 1 -> damage_dealt + temp_damage
    | _ -> deal_damage damage_dice (attacks_amount - 1) (damage_dealt + temp_damage)

let rec attack (weapon: Weapon) (turn: int) (advantage: bool) (disadvantage: bool) =
    if turn = 0 then
        printfn "Attack is over"
    else 
        let dice_roll = roll 20 (Some advantage) (Some disadvantage)

        let critical_hit =
            match dice_roll with
            | 20 -> true
            | _ -> false

        let hitbonus = weapon.Hitbonus
        let roll_to_hit = dice_roll + hitbonus
        let double_attack = List.contains "Double Attack" weapon.Properties

        printf "roll: %d hit? [y/n]? " roll_to_hit
        let input = Console.ReadLine()
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

        let rec damage (double_attack: bool) : int =
            let hit_damage =
                match hit with
                | true -> deal_damage weapon.Damage_Dice 1 0
                | false -> weapon.Damage_On_Miss

            match double_attack with
            | true -> 
                printfn "DAMAGE RECURSIVE CALL"
                printfn "round 1 hit dmg: %d" hit_damage
                hit_damage + damage false
            | false -> hit_damage

        let base_damage: int = damage double_attack
        let bonus_damage: int = weapon.Damage_Bonus * if double_attack then 1 else 1
        let dealt_damage: int = base_damage + bonus_damage

        printfn "you dealt %d damage" dealt_damage

        match critical_hit with
        | true -> attack weapon (turn + 1) advantage disadvantage
        | false -> attack weapon (turn - 1) advantage disadvantage

let rec combat_loop (weapons_list: Weapon list) (turn: int) (advantage: bool) (disadvantage: bool) : unit =

    if turn = 0 then
        printfn "Combat ended"
    else
        printfn "Please select a weapon from the list [1,2,..]"
        print_weapons weapons_list 1

        let input: string = Console.ReadLine()
        let parsed_input: int = Int32.Parse input

        let defalt_attacks: int = 1

        match parsed_input >= 1 && parsed_input <= weapons_list.Length with
        | true ->
            let weapon: Weapon = List.item (parsed_input - 1) weapons_list
            attack weapon defalt_attacks advantage disadvantage
            combat_loop weapons_list (turn - 1) advantage disadvantage
        | false ->
            printfn "invalid input - weapon number must be between 1 and %d" weapons_list.Length
            combat_loop weapons_list turn advantage disadvantage

[<EntryPoint>]
let main args =

    clone_database ()

    let os_name: string = get_operating_system ()
    printfn "DEBUG! OPERATING SYSTEM: %s" os_name
    let config_path: string = get_config_path os_name
    printfn "DEBUG! CONFIG PATH: %s" config_path
    create_config_dir os_name
    clone_database()
    let weapons_json_path = get_weapons_path os_name
    let weapons_list: Weapon List = load_weapons weapons_json_path

    let has_arg arg : bool = Array.contains arg args
    let hasSurge: bool = has_arg "surge"
    let hasHaste: bool = has_arg "haste"
    let advantage: bool = has_arg "advantage"
    let disadvantage: bool = has_arg "disadvantage"

    let number_of_attacks: int =
        // change values so it actually alligns with the rules as written
        match hasSurge, hasHaste with
        | true, true -> 3
        | true, false -> 2
        | false, true -> 2
        | false, false -> 1

    printfn "Number of attacks: %d" number_of_attacks

    combat_loop weapons_list number_of_attacks advantage disadvantage

    0
