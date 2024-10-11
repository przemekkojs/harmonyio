class TwoNotes {
    constructor(isUpperStaff, note1 = null, note2 = null){
        this.note1 = note1;
        this.note2 = note2;
        this.isUpperStaff = isUpperStaff;
    }

    isEmpty() {
        // always note1 will be taken first
        return this.note1 === null;
    }

    isFull() {
        return this.note2 !== null;
    }

    hasEmptySpaceForNote() {
        return !this.isFull();
    }

    getSlotsTaken() {
        return this.note1 === null ? 0 : this.note1.getSlotsTaken()
    }
}