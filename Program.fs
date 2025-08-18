// custom modules
open dice
open JsonStuff
open Setup
open Types

open System


let pretty_print (text: string) =
    let padding = 1
    let boxWidth = text.Length + padding * 2
    printf " ╭"; printf "%s" (String.replicate boxWidth "─"); printfn "╮"
    printfn " │%s%s%s│" (String.replicate padding " ") text (String.replicate padding " ")
    printf " ╰"; printf "%s" (String.replicate boxWidth "─"); printfn "╯"

let rec print_weapons weapons index : unit =
    match weapons with
    | [] -> ()
    | head :: tail ->
        printfn "%d: %s" index head.Name
        print_weapons tail (index + 1)

// deal_damage accounts is recursive to account for situations where the user can attack multiple times
let rec deal_damage (damage_dice: string) (attacks_amount: int) (damage_dealt: int) : int =

    // temp_damage accounts for situations like 2d6
    // where it then rolls two d6's
    let rec temp_damage (rolling_dice: int) (rolls_amount: int) (damage_dealt: int): int =
        let allowed_dice: int list = [4;6;8;10;12;20;100]

        let damage_roll: int = 
            match List.contains rolling_dice allowed_dice with
            | true -> roll rolling_dice None None
            | false -> 
                printfn "Invalid dice: d%d" rolling_dice
                0

        let current_damage: int = damage_roll + damage_dealt
        match rolls_amount with
        | 1 -> current_damage + damage_dealt
        | _ -> temp_damage rolling_dice (rolls_amount-1) current_damage

    let split_dice: string array = damage_dice.Split 'd'
    let rolling_dice: int = int split_dice[1]
    let dice_amount: int  = int split_dice[0]

    let pre_damage: int = 0
    let current_damage: int = temp_damage rolling_dice dice_amount pre_damage

    match attacks_amount with
    | 1 -> damage_dealt + current_damage
    | _ -> deal_damage damage_dice (attacks_amount-1) (damage_dealt + current_damage)

let rec attack (weapon: Weapon) (turn: int) (advantage: bool) (disadvantage: bool) =
    if turn = 0 then
        printfn""
    else
        let dice_roll = roll 20 (Some advantage) (Some disadvantage)

        let critical_hit =
            match dice_roll with
            | 20 -> true
            | _ -> false

        if critical_hit then
            printfn "NAT 20 BABY!!"

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

        // SOMETHINGS WRONG I CAN FEEL IT
        let damage (double_attack: bool) : int =

            let attacks_amount: int=
                match double_attack with
                | true -> 2
                | false -> 1

            let hit_damage =
                match hit with
                | true -> 
                    deal_damage weapon.Damage_Dice attacks_amount 0
                | false -> weapon.Damage_On_Miss

            hit_damage

        let total_damage: int = damage double_attack
        let damage = sprintf "you dealt %d damage" total_damage
        pretty_print damage

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

let args_help () : unit =
    let lines =
        [ "here are the valid arguments to pass to the program and their function:"
          "'surge'        - the ability Action Surge is being used (extra attack)"
          "'haste'        - the spell Haste has been cast on your character (extra attack)"
          "'advantage'    - your character has advantage on the roll to hit"
          "'disadvantage' - your character has disadvantage on the roll to hit"
          "'help'         - prints this help menu :)" ]

    lines |> List.iter (printfn "%s")

[<EntryPoint>]
let main args =

    let os_name: string = get_operating_system ()
    create_config_dir os_name
    clone_database ()
    let weapons_json_path = get_weapons_path os_name
    let weapons_list: Weapon List = load_weapons weapons_json_path

    let has_arg arg : bool = Array.contains arg args
    let has_surge: bool = has_arg "surge"
    let has_haste: bool = has_arg "haste"
    let advantage: bool = has_arg "advantage"
    let disadvantage: bool = has_arg "disadvantage"
    let help: bool = has_arg "help"

    if help then
        args_help ()
    else
        // Rules as written:
        let number_of_attacks: int =
            match has_surge, has_haste with
            | true, true -> 3
            | true, false -> 2
            | false, true -> 2
            | false, false -> 1

        printfn "Number of attacks: %d" number_of_attacks
        combat_loop weapons_list number_of_attacks advantage disadvantage

    0
