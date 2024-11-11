class Metre {
  constructor(count, value) {
    this.count = count;
    this.value = value;

    this.isRendered = false;
  }

  slotsPerBar() {
    return Note.baseValueToSlots(this.value) * this.count;
  }

  clearGraphics() {
    if (this.metreGraphic) {
      this.metreGraphic.remove();
    }
  }

  #renderGraphic(letterSize) {
    textSize(letterSize);

    const graphicH = 4 * spaceBetweenStaffLines;
    const graphicW = Math.max(this.getMetreWidth());
    let metreGraphic = createGraphics(graphicW, graphicH);
    metreGraphic.textSize(letterSize);
    metreGraphic.textAlign(CENTER, BASELINE);

    metreGraphic.text(this.count, graphicW / 2, graphicH / 2 - 3);
    metreGraphic.text(this.value, graphicW / 2, graphicH - 3);

    this.metreGraphic = metreGraphic;
    this.isRendered = true;
  }

  draw(x, y) {
    if (!this.isRendered) {
      const letterSize = this.calculateTextSize();
      this.#renderGraphic(letterSize);
    }
    imageMode(CORNER);
    image(this.metreGraphic, x, y);
  }

  calculateTextSize() {
    return 2.4 * spaceBetweenStaffLines;
  }

  getMetreWidth() {
    return this.calculateTextSize() * 1.4;
  }
}
