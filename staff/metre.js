class Metre {
    constructor(count, value) {
      this.count = count;
      this.value = value;
    }

    slotsPerBar() {
        return Note.valueToSlots(this.value) * this.count;
    }
}