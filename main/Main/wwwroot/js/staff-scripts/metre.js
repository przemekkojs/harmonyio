class Metre {
  constructor(count, value) {
    this.count = count;
    this.value = value;
  }

  slotsPerBar() {
    return Note.baseValueToSlots(this.value) * this.count;
  }

  draw(x, y) {
    const size = this.calculateTextSize();
    textSize(size);
    textAlign(CENTER);
    stroke(0);
    strokeWeight(0);

    text(this.count, x + size / 2, y + 0.75 * size); // upper number of metre
    text(this.value, x + size / 2, y + 0.75 * size + 2 * spaceBetweenStaffLines); //lower number of metre
  }

  calculateTextSize() {
    return 2.4 * spaceBetweenStaffLines;
  }

  getMetreWidth() {
    return this.calculateTextSize() * 1.4;
  }
}
