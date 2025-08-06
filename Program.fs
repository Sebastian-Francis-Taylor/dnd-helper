open dice
open JsonStuff

let rec attack weapon_name turn hitbonus =
    if turn not = 0 then
        let dice_roll = d20
        let ciritical_hit = if dice_roll = 20 then true else false
        let roll_to_hit = dice_roll + hitbonus

        printf "roll: %d hit? [y/n]? " roll_to_hit
        let input = System.Console.ReadLine()
        printf "\n"

        let hit =
            match input with
            | "y" -> true
            | "n" -> false
            | _ ->
                printfn "Invalid input"
                attack weapon_name turn hitbonus

        let damage =
            if hit then
                weapon_name.damage
            else
                weapon_name.damage_on_miss

        printfn "you dealt %d" damage

        // fix recursive call
        if ciritical_hit then attack turn + 1 else attack turn - 1

let rec combat_loop turn =

    // loads the weapon list on every turn (memory eater)
    let weapons_list = load_weapons "weapons.json"

    if turn > 0 then
        printf "Please select a weapon from the list "
        // Print weapons here

        let input = System.Console.ReadLine()

        (*
        add weapon selection
        pass weapon into attack
        find a way to avoid one massive patt
        *)

        // Change to match with weapons_list name
        let defalt_attacks = 1

        match input with
        | "whack" -> attack weapon_name defalt_attacks weapon_hitbonus
        | _ ->
            printfn "invalid input try 'whack' or 'range'"
            combat_loop turn


[<EntryPoint>]
let main args =
    let has_arg arg = Array.contains arg args
    let hasSurge = has_arg "surge"
    let hasHaste = has_arg "haste"

    let number_of_attacks =
        // change values so it actually alligns with the rules as written
        match hasSurge, hasHaste with
        | true, true -> 10
        | true, false -> 5
        | false, true -> 2
        | false, false -> 1

    printfn "Number of attacks: %d" number_of_attacks

    combat_loop number_of_attacks

    0
