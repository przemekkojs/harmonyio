class GrandStaff {
    constructor(keySignature = new KeySignature(3), metre = new Metre(3,4)) {
        this.keySignature = keySignature;
        this.metre = metre;
        
        this.bars = [];
    }
}