class KeySignature {
  static sharpOffsets = [0, 1.5, -0.5, 1, 2.5, 0.5, 2];
  static bemolOffsets = [2, 0.5, 2.5, 1, 3, 1.5, 3.5];

  constructor(
    count = 0,
    accidental = new Accidental(Accidental.accidentals.sharp)
  ) {
    this.count = count;
    this.accidental = accidental;

    this.offsets =
      this.accidental.getName() === Accidental.accidentals.sharp
        ? KeySignature.sharpOffsets
        : KeySignature.bemolOffsets;
  }

  getWidth() {
    return this.count * this.accidental.getWidth();
  }

  draw(x, y, isUpperStaff) {
    const accidentalWidth = this.accidental.getWidth();
    const additionalOffset = isUpperStaff ? 0 : 1;

    let accidentalX = x + accidentalWidth / 2;
    for (let i = 0; i < this.count; i++) {
      const offset = this.offsets[i] + additionalOffset;
      this.accidental.draw(accidentalX, y + GrandStaff.getLineOffset(offset));
      accidentalX += accidentalWidth;
    }
  }
}
