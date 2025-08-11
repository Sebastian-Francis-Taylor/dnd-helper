module dice

let rnd = System.Random()

let roll (dice: int) (advantage: bool option) (disadvantage: bool option): int =
    let advantage = defaultArg advantage false
    let disadvantage = defaultArg disadvantage false
    let dice_roll() = rnd.Next(1, dice + 1)
    
    match advantage, disadvantage with
    | true, true -> dice_roll()
    | true, false -> max (dice_roll()) (dice_roll())
    | false, true -> min (dice_roll()) (dice_roll())
    | false, false -> dice_roll()
