class Note {
  static availableBaseValues = [1, 2, 4, 8, 16];
  static noteVoices = {
    soprano: "SOPRANO",
    alto: "ALTO",
    tenore: "TENORE",
    bass: "BASS",
  };

  constructor(
    baseValue = 1,
    hasDot = false,
    isFacingUp = true,
    accidentalName = Accidental.accidentals.none
  ) {
    this.headSymbolMapping = {
      1: symbols.fullNote, // Full note image
      2: symbols.noteHeadOpened, // Half note
      4: symbols.noteHeadClosed, // Quarter note
      8: symbols.noteHeadClosed, // Eighth note (same as quarter note head)
      16: symbols.noteHeadClosed, // Sixteenth note (same as quarter note head)
    };

    this.hasDot = hasDot;
    this.setBaseValue(baseValue);
    this.isFacingUp = isFacingUp;
    this.accidental = new Accidental(accidentalName);
  }

  toJson() {
    return {
      value: this.getSlotsTaken(),
      accidentalName: this.accidental.getChar(),
      line: this.line,
    };
  }

  static fromJson(noteJson) {
    const slots = noteJson.value;
    const { baseValue, hasDot } = Note.slotsToBaseValueAndDot(slots);

    const accidentalChar = noteJson.accidentalName;
    const accidentalName = Accidental.charToAccidentalName(accidentalChar);

    const voice = noteJson.voice;
    const isFacingUp =
      voice === Note.noteVoices.soprano || voice === Note.noteVoices.tenore;

    const line = noteJson.line;

    let note = new Note(baseValue, hasDot, isFacingUp, accidentalName);
    note.setLine(line);
    return note;
  }

  hasSameValue(other) {
    return this.getValue() === other.getValue();
  }

  hasOppositeDirection(other) {
    return this.isFacingUp !== other.isFacingUp;
  }

  hasAccidental() {
    return this.accidental.isSet();
  }

  getAccidentalName() {
    return this.accidental.getName();
  }

  setLine(line) {
    this.line = line;
  }

  setBaseValue(baseValue) {
    if (Note.availableBaseValues.includes(baseValue)) {
      this.baseValue = baseValue;

      // when setting last available note value, disable dot
      const lastAvailableBaseValueIndex = Note.availableBaseValues.length - 1;
      const lastAvailableBaseValue =
        Note.availableBaseValues[lastAvailableBaseValueIndex];
      if (baseValue === lastAvailableBaseValue) {
        this.hasDot = false;
      }
    } else {
      this.baseValue = 4; // if baseValue is not correct, default to 4
    }
  }

  setAccidental(accidentalName) {
    this.accidental.setName(accidentalName);
  }

  getBaseValue() {
    return this.baseValue;
  }

  getValue() {
    if (this.hasDot) {
      return this.baseValue + 2 * this.baseValue;
    } else {
      return this.baseValue;
    }
  }

  getSlotsTaken() {
    if (this.hasDot) {
      return (
        Note.baseValueToSlots(this.baseValue) +
        Note.baseValueToSlots(this.baseValue * 2)
      );
    } else {
      return Note.baseValueToSlots(this.baseValue);
    }
  }

  static baseValueToSlots(baseValue) {
    // value -> slots
    // 1 -> 16
    // 2 -> 8
    // 4 -> 4
    // 8 -> 2
    // 16 -> 1
    return 16 / baseValue;
  }

  static slotsToBaseValueAndDot(slots) {
    for (let i = 0; i < Note.availableBaseValues.length; i++) {
      const calculatedSlots = Note.baseValueToSlots(
        Note.availableBaseValues[i]
      );
      if (calculatedSlots === slots) {
        const baseValue = Note.availableBaseValues[i];
        const hasDot = false;
        return { baseValue, hasDot };
      } else if (calculatedSlots < slots) {
        const baseValue = Note.availableBaseValues[i];
        const hasDot = true;
        return { baseValue, hasDot };
      }
    }

    const baseValue = 16;
    const hasDot = false;
    return { baseValue, hasDot };
  }

  toggleIsFacingUp() {
    this.isFacingUp = !this.isFacingUp;
  }

  // 16-th note cant have dot
  toggleHasDot() {
    // dont allow to have dot while being last possible note (16)
    const lastAvailableBaseValueIndex = Note.availableBaseValues.length - 1;
    const lastAvailableBaseValue =
      Note.availableBaseValues[lastAvailableBaseValueIndex];
    if (this.baseValue !== lastAvailableBaseValue) {
      this.hasDot = !this.hasDot;
    }
  }

  getNoteWidth(addFlagWidth = true) {
    if (addFlagWidth && this.isFacingUp && this.#howManyFlags() > 0) {
      return this.#getHeadSymbol().width + symbols.noteFlag.width;
    } else {
      return this.#getHeadSymbol().width;
    }
  }

  draw(x, y) {
    this.drawNote(x, y);

    if (this.hasDot) {
      const dotX = x + this.#getHeadSymbol().width / 2 + dotDiameter + 2;
      if (this.line && Math.floor(this.line) === this.line) {
        const dotY = this.isFacingUp
          ? y - spaceBetweenStaffLines / 2
          : y + spaceBetweenStaffLines / 2;
        this.drawDot(dotX, dotY);
      } else {
        this.drawDot(dotX, y);
      }
    }

    if (this.hasAccidental()) {
      const accidentalOffset =
        this.#getHeadSymbol().width / 2 +
        this.accidental.getWidth() / 2 +
        spaceBetweenNoteAndAccidental;
      this.drawAccidental(x - accidentalOffset, y);
    }
  }

  drawNote(x, y) {
    const headSymbol = this.#getHeadSymbol();
    push();

    imageMode(CENTER);
    image(headSymbol, x, y);

    if (this.#hasStem()) {
      stroke(0);
      strokeWeight(2);

      let stemX, stemY1, stemY2;
      if (this.isFacingUp) {
        stemX = x - 1 + headSymbol.width / 2;
        stemY1 = y - spaceBetweenStaffLines / 4;
        stemY2 = y - stemHeight;
      } else {
        stemX = x + 1 - headSymbol.width / 2;
        stemY1 = y + spaceBetweenStaffLines / 4;
        stemY2 = y + stemHeight;
      }

      line(stemX, stemY1, stemX, stemY2);

      this.#drawFlags(stemX, stemY2);
    }

    pop();
  }

  drawDot(x, y) {
    push();
    fill(0);
    circle(x, y, dotDiameter);
    pop();
  }

  drawAccidental(x, y) {
    this.accidental.draw(x, y);
  }

  #drawFlags(stemX, stemY) {
    const numOfFlags = this.#howManyFlags();
    if (numOfFlags === 0) return;

    for (let i = 0; i < numOfFlags; i++) {
      push();
      imageMode(CORNER);
      translate(
        stemX,
        stemY + ((i * stemHeight) / 3) * (this.isFacingUp ? 1 : -1)
      );
      if (!this.isFacingUp) scale(1, -1);

      image(symbols.noteFlag, 0, 0);
      pop();
    }
  }

  #getHeadSymbol() {
    return this.headSymbolMapping[this.baseValue];
  }

  #hasStem() {
    return this.baseValue !== 1;
  }

  #howManyFlags() {
    if (this.baseValue === 8) {
      return 1;
    } else if (this.baseValue === 16) {
      return 2;
    } else {
      return 0;
    }
  }
}
