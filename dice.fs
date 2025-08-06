module dice

let rnd = System.Random()

let roll dice =
    let dice_roll = rnd.Next(1, dice + 1)
    dice_roll

let d4 = roll 4
let d6 = roll 6
let d8 = roll 8
let d10 = roll 10
let d12 = roll 12
let d20 = roll 20
let d100 = roll 100
