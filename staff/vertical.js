class Vertical {
    constructor(
        upperStaff = new TwoNotes(true),
        lowerStaff = new TwoNotes(false)
    ) {
        this.upperStaff = upperStaff;
        this.lowerStaff = lowerStaff;
    }

    getSlotsTaken() {
        return Math.max(
            this.upperStaff.getSlotsTaken(),
            this.lowerStaff.getSlotsTaken()
        )
    }

    isEmpty() {
        return this.upperStaff.isEmpty() && this.lowerStaff.isEmpty();
    }
}