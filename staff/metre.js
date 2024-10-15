class Metre {
  constructor(count, value) {
    this.count = count;
    this.value = value;
  }

  slotsPerBar() {
    return Note.baseValueToSlots(this.value) * this.count;
  }

  draw(x, y) {
    push();

    let size = this.calculateTextSize();
    textSize(size);
    textAlign(CENTER);
    translate(size / 2, 0.75 * size);

    text(this.count, x, y); // upper number of metre
    text(this.value, x, y + 2 * spaceBetweenStaffLines); //lower number of metre

    pop();
  }

  calculateTextSize() {
    return 2.4 * spaceBetweenStaffLines;
  }

  getMetreWidth() {
    return this.calculateTextSize() * 1.4;
  }
}
