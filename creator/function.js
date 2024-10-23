export class Function {
    constructor(minor, symbol, position, root, removed, alterations, added) {
        this.minor = minor;
        this.symbol = symbol;
        this.position = position;
        this.root = root;
        this.removed = removed;
        this.alterations = alterations;
        this.added = added;
    }
}

// functionLetter = symbol,
// hasCircle = minor,
// numberAbove = position,
// numberBelow = root,
// numbersTopRight = added,
// numbersBottomRight = removed,
// numbersCenterRight = alterations