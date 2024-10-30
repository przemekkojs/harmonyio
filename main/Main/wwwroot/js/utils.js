export function parseTonationToAccidentalsCount(tonationString) {
    switch (tonationString) {
        case "Cbdur":
            return [0, 7];
        case "Cdur":
            return [0, 0];
        case "Cmoll":
            return [0, 3];
        case "C#dur":
            return [7, 0];
        case "C#moll":
            return [4, 0];
        case "Dbdur":
            return [0, 5];
        case "Ddur":
            return [2, 0];
        case "Dmoll":
            return [0, 1];
        case "D#moll":
            return [6, 0];
        case "Ebdur":
            return [0, 3];
        case "Ebmoll":
            return [0, 6];
        case "Edur":
            return [4, 0];
        case "Emoll":
            return [1, 0];
        case "Fdur":
            return [0, 1];
        case "Fmoll":
            return [0, 4];
        case "F#dur":
            return [6, 0];
        case "F#moll":
            return [3, 0];
        case "Gbdur":
            return [0, 6];
        case "Gdur":
            return [1, 0];
        case "Gmoll":
            return [0, 2];
        case "G#moll":
            return [5, 0];
        case "Abdur":
            return [0, 4];
        case "Abmoll":
            return [0, 7];
        case "Adur":
            return [3, 0];
        case "Amoll":
            return [0, 0];
        case "A#moll":
            return [7, 0];
        case "Bbdur":
            return [0, 2];
        case "Bbmoll":
            return [0, 5];
        case "Bdur":
            return [5, 0];
        case "Bmoll":
            return [2, 0];
        default:
            return [0, 0];
    }
}