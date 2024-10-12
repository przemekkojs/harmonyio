class KeySignature {
  static sharpOffsets = [0, 1.5, -0.5, 1, 2.5, 0.5, 2];
  static bemolOffsets = [2, 0.5, 2.5, 1, 3, 1.5, 3.5];

  constructor(count = 0, accidental = new Accidental("bemol")) {
    this.count = count;
    this.accidental = accidental;
  }

  getWidth() {
    return this.count * this.accidental.getWidth();
  }

  draw(x, y, isUpperStaff) {
    const accidentalName = this.accidental.getName();
    const accidentalWidth = this.accidental.getWidth();
    const additionalOffset = isUpperStaff ? 0 : 1;
    const offsets =
      accidentalName === "sharp"
        ? KeySignature.sharpOffsets
        : KeySignature.bemolOffsets;

    push();
    translate(x + accidentalWidth / 2, y);
    for (let i = 0; i < this.count; i++) {
      const offset = offsets[i] + additionalOffset;
      this.accidental.draw(0, GrandStaff.getLineOffset(offset));
      translate(accidentalWidth, 0);
    }
    pop();
  }
}
