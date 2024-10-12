class TwoNotes {
  constructor(parent, isUpperStaff, note1 = null, note2 = null) {
    this.note1 = note1;
    this.note2 = note2;
    this.isUpperStaff = isUpperStaff;

    this.parent = parent;
    this.height = isUpperStaff ? upperStaffHeight : lowerStaffHeight;
    this.availableLinesAbove = isUpperStaff ? 2 : 3;
    this.availableLinesBelow = isUpperStaff ? 2.5 : 2;
    this.width = slotWidth;
  }

  getSlotsTaken() {
    if (this.note1 === null) {
      return 0;
    }

    return this.note1.getSlotsTaken();
  }

  isEmpty() {
    return this.note1 === null;
  }

  hasEmptyNote() {
    return this.note2 === null;
  }

  hasSameValue(note) {
    if (this.isEmpty()) {
      return false;
    }

    return this.note1.hasSameValue(note);
  }

  hasOppositeDirection(note) {
    if (this.isEmpty()) {
      return false;
    }

    return this.note1.hasOppositeDirection(note);
  }

  canAddNote(note) {
    // need to check vertical and/or whole bar to determine if can add if twoNote is empty
    if (this.isEmpty()) {
      console.log("need to check vertical");
      return this.parent.canAddNote(note, this.isUpperStaff);
    }

    //cant add if there are already two notes on the staff
    if (!this.hasEmptyNote()) {
      console.log("no empty note");
      return false;
    }

    // cant add if notes has different value
    if (!this.note1.hasSameValue(note)) {
      console.log("different value");
      return false;
    }

    // notes have same direction, if base value is 1, allow adding but change direction
    // TODO fix bug, this adds instantly two whole notes, need to check for that
    if (
      !this.note1.hasOppositeDirection(note) &&
      this.note1.getBaseValue() !== 1
    ) {
      console.log("same direction");
      return false;
    }

    return true;
  }

  addNote(note, line) {
    if (!this.canAddNote(note)) {
      return false;
    }

    if (this.note1 === null) {
      this.note1 = new Note(
        note.getBaseValue(),
        note.hasDot,
        note.isFacingUp,
        note.getAccidentalName()
      );
      this.note1.setLine(line);

      return true;
    }

    // handle adding whole note
    if (note.getBaseValue() === 1) {
      // cant add two notes on same line (TODO check if true)
      if (line === this.note1.line) {
        console.log("whole note already there");
        return false;
      }

      // need to have opposite direction
      if (!this.note1.hasOppositeDirection(note)) {
        this.note2 = new Note(
          note.getBaseValue(),
          note.hasDot,
          !note.isFacingUp,
          note.getAccidentalName()
        );
        this.note2.setLine(line);

        return true;
      }
    }

    this.note2 = new Note(
      note.getBaseValue(),
      note.hasDot,
      note.isFacingUp,
      note.getAccidentalName()
    );
    this.note2.setLine(line);

    return true;
  }

  updatePosition(x, y) {
    this.x = x;
    this.y = y;

    this.topLineY = this.isUpperStaff
      ? this.y + upperStaffUpperMargin
      : this.y + lowerStaffUpperMargin;
  }

  // works like a charm
  getClosestSnappingPoint(x, y) {
    const center = this.x + this.width / 2;

    // above highest line
    const highestAvailableSnappingPointY =
      this.topLineY - this.availableLinesAbove * spaceBetweenStaffLines;
    if (y < highestAvailableSnappingPointY) {
      return {
        x: center,
        y: highestAvailableSnappingPointY,
        lineNumber: -this.availableLinesAbove,
      };
    }

    // below lowest line
    const lowestAvailableSnappingPointY =
      this.topLineY +
      4 * spaceBetweenStaffLines +
      this.availableLinesBelow * spaceBetweenStaffLines;
    if (y > lowestAvailableSnappingPointY) {
      return {
        x: center,
        y: lowestAvailableSnappingPointY,
        lineNumber: 4 + this.availableLinesBelow,
      };
    }

    const firstSnappingPointTop = this.topLineY - spaceBetweenStaffLines * 0.25;
    const diff = y - firstSnappingPointTop;
    const lineNumber = Math.floor(diff / (spaceBetweenStaffLines * 0.5)) * 0.5;
    return {
      x: center,
      y: this.topLineY + lineNumber * spaceBetweenStaffLines,
      lineNumber: lineNumber,
    };
  }

  isOver(x, y) {
    const isOverCheck =
      x > this.x &&
      x < this.x + this.width &&
      y > this.y &&
      y < this.y + this.height;
    if (!isOverCheck) {
      return {};
    }

    return { twoNotes: this };
  }

  draw() {
    this.#drawBoundingBox();

    if (this.note1 !== null) {
      this.note1.drawSimple(
        this.x + this.width / 2,
        this.topLineY + spaceBetweenStaffLines * this.note1.line
      );
    }

    if (this.note2 !== null) {
      this.note2.drawSimple(
        this.x + this.width / 2,
        this.topLineY + spaceBetweenStaffLines * this.note2.line
      );
    }
  }

  #drawBoundingBox() {
    push();
    noFill();
    rect(this.x, this.y + 4, this.width, this.height - 8);
    pop();
  }
}
