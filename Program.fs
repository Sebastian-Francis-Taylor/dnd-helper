open dice
open JsonStuff

type Weapon =
    { Name: string
      Damage_Dice: string
      Damage_Bonus: int
      Damage_On_Miss: int
      Hitbonus: int }

let deal_damage damage_dice =
    match damage_dice with
    | "d4" -> d4
    | "d6" -> d6
    | "d8" -> d8
    | "d10" -> d10
    | "d12" -> d12
    | "d20" -> d20 false false
    | "d100" -> d100
    | _ -> 0

let rec attack (weapon: Weapon) (turn: int) (advantage: bool) (disadvantage: bool) =
    if turn = 0 then
        printfn "Attack is over"

    let dice_roll = d20 advantage disadvantage    

    let critical_hit = 
        match dice_roll with
        | 20 -> true
        | _ -> false
    let hitbonus = weapon.Hitbonus
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
            attack weapon turn advantage disadvantage
            // Code won't reach here, dummy value
            false

    let damage =
        match hit with
        | true -> deal_damage weapon.Damage_Dice
        | false -> weapon.Damage_On_Miss

    printfn "you dealt %d" damage

    match critical_hit with
    | true -> attack weapon (turn + 1) advantage disadvantage
    | false -> attack weapon (turn - 1) advantage disadvantage

let rec combat_loop weapons_list turn =

    if turn = 0 then
        printf "Combat ended"

    printf "Please select a weapon from the list "
    // Print weapons here

    let input = System.Console.ReadLine()

    (*
    add weapon selection
    pass weapon into attack
    find a way to avoid one massive patt
    *)

    let weapon = get_weapons_by_name input


    // Change to match with weapons_list name
    let defalt_attacks = 1

    match input with
    | "whack" -> attack weapon defalt_attacks
    | _ ->
        printfn "invalid input try 'whack' or 'range'"
        combat_loop weapons_list turn


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
