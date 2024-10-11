class Bar {
    constructor(slotsPerBar){
        this.verticals = [];
        this.slotsPerBar = slotsPerBar;
    }

    getSlotsTaken() {
        return this.verticals.reduce((sum, vertical) => sum + vertical.getSlotsTaken(), 0);
    }

    getSlotsAvailable() {
        return this.slotsPerBar - this.getSlotsTaken();
    }

    countEmptyVerticals() {
        return this.verticals.filter(vertical => vertical.getSlotsTaken() === 0).length;
    }
}