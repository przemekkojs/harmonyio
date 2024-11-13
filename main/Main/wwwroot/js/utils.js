export function parseTonationToAccidentalsCount(tonationString) {
    switch (tonationString) {
        case "Cesdur":
            return [0, 7, 1];
        case "Cdur":
            return [0, 0, 1];
        case "Cmoll":
            return [0, 3, 0];
        case "Cisdur":
            return [7, 0, 1];
        case "Cismoll":
            return [4, 0, 0];
        case "Desdur":
            return [0, 5, 1];
        case "Ddur":
            return [2, 0, 1];
        case "Dmoll":
            return [0, 1, 0];
        case "Dismoll":
            return [6, 0, 0];
        case "Esdur":
            return [0, 3, 1];
        case "Esmoll":
            return [0, 6, 0];
        case "Edur":
            return [4, 0, 1];
        case "Emoll":
            return [1, 0, 0];
        case "Fdur":
            return [0, 1, 1];
        case "Fmoll":
            return [0, 4, 0];
        case "Fisdur":
            return [6, 0, 1];
        case "Fismoll":
            return [3, 0, 0];
        case "Gesdur":
            return [0, 6, 1];
        case "Gdur":
            return [1, 0, 1];
        case "Gmoll":
            return [0, 2, 0];
        case "Gismoll":
            return [5, 0, 0];
        case "Asdur":
            return [0, 4, 1];
        case "Asmoll":
            return [0, 7, 0];
        case "Adur":
            return [3, 0, 1];
        case "Amoll":
            return [0, 0, 0];
        case "Aismoll":
            return [7, 0, 0];
        case "Bdur":
            return [0, 2, 1];
        case "Bmoll":
            return [0, 5, 0];
        case "Hdur":
            return [5, 0, 1];
        case "Hmoll":
            return [2, 0, 0];
        default:
            return [-1, -1];
    }
}

export function parseAccidentalsCountToTonationInfo(accidentalsCount) {
    if (accidentalsCount.length != 3)
        return ["C", "dur"];

    const minor = (accidentalsCount[2] === 0);
    const sharpsCount = accidentalsCount[0];
    const flatsCount = accidentalsCount[1];

    if (sharpsCount != 0 && flatsCount != 0)
        return ["C", "dur"];

    if (sharpsCount == 0) {
        if (minor) {
            switch (flatsCount) {
                case 0: return ["A", "moll"];
                case 1: return ["D", "moll"];
                case 2: return ["G", "moll"];
                case 3: return ["C", "moll"];
                case 4: return ["F", "moll"];
                case 5: return ["B", "moll"];
                case 6: return ["Es", "moll"];
                case 7: return ["As", "moll"];
                default : return ["C", "dur"];
            }
        }
        else {
            switch (flatsCount) {
                case 1: return ["F", "dur"];
                case 2: return ["B", "dur"];
                case 3: return ["Es", "dur"];
                case 4: return ["As", "dur"];
                case 5: return ["Des", "dur"];
                case 6: return ["Ges", "dur"];
                case 7: return ["Ces", "dur"];
                default: return ["C", "dur"];
            }
        }
    }
    else {
        if (minor) {
            switch (sharpsCount) {
                case 0: return ["A", "moll"];
                case 1: return ["E", "moll"];
                case 2: return ["H", "moll"];
                case 3: return ["Fis", "moll"];
                case 4: return ["Cis", "moll"];
                case 5: return ["Gis", "moll"];
                case 6: return ["Dis", "moll"];
                case 7: return ["Ais", "moll"];
                default: return ["C", "dur"];
            }
        }
        else {
            switch (sharpsCount) {
                case 1: return ["G", "dur"];
                case 2: return ["D", "dur"];
                case 3: return ["A", "dur"];
                case 4: return ["E", "dur"];
                case 5: return ["H", "dur"];
                case 6: return ["Fis", "dur"];
                case 7: return ["Cis", "dur"];
                default: return ["C", "dur"];
            }
        }
    }

    return ["C", "dur"];
}

export function parseMetreValuesToMetre(values) {
    if (values.length != 2)
        return "2/4";

    return `${values[1]}/${values[0]}`;
}