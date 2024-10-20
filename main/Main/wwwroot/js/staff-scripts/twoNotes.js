class TwoNotes {
  constructor(parent, isUpperStaff, note1 = null, note2 = null) {
    this.note1 = note1;
    this.note2 = note2;
    this.isUpperStaff = isUpperStaff;

    this.parent = parent;
    this.height = isUpperStaff ? upperStaffHeight : lowerStaffHeight;
    this.availableLinesAbove = isUpperStaff ? 2 : 3;
    this.availableLinesBelow = isUpperStaff ? 2.5 : 2;
  }

  toJson() {
    let jsonResult = [];

    let note1Json;
    if (this.note1 !== null) {
      note1Json = this.note1.toJson();
      note1Json.voice = this.#determineVoice(this.note1);
      jsonResult.push(note1Json);
    }

    let note2Json;
    if (this.note2 !== null) {
      note2Json = this.note2.toJson();
      note2Json.voice = this.#determineVoice(this.note2);
      jsonResult.push(note2Json);
    } else {
    }

    return jsonResult;
  }

  loadNote(note) {
    if (this.note1 === null) {
      this.note1 = note;
    } else {
      this.note2 = note;
    }
  }

  #determineVoice(note) {
    if (note.isFacingUp && this.isUpperStaff) {
      return Note.noteVoices.soprano;
    } else if (!note.isFacingUp && this.isUpperStaff) {
      return Note.noteVoices.alto;
    } else if (note.isFacingUp && !this.isUpperStaff) {
      return Note.noteVoices.tenore;
    } else {
      return Note.noteVoices.bass;
    }
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
      return this.parent.canAddNote(note, this.isUpperStaff);
    }

    //cant add if there are already two notes on the staff
    if (!this.hasEmptyNote()) {
      return false;
    }

    // cant add if notes has different value
    if (!this.note1.hasSameValue(note)) {
      return false;
    }

    // notes have same direction, if base value is 1, allow adding but change direction
    // TODO fix bug, this adds instantly two whole notes, need to check for that
    if (
      !this.note1.hasOppositeDirection(note) &&
      this.note1.getBaseValue() !== 1
    ) {
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
      this.parent.parent.updateVerticalPositions();
      return true;
    }

    // handle adding whole note, need to have opposite direction
    if (note.getBaseValue() === 1 && !this.note1.hasOppositeDirection(note)) {
      this.note2 = new Note(
        note.getBaseValue(),
        note.hasDot,
        !note.isFacingUp,
        note.getAccidentalName()
      );
      this.note2.setLine(line);

      return true;
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
  }

  isNoteFacingUpAboveNoteFacingDown() {
    if (this.hasEmptyNote()) {
      return true;
    }

    if (this.note1.isFacingUp) {
      return this.note1.line < this.note2.line;
    } else {
      return this.note2.line < this.note1.line;
    }
  }

  getWidth() {
    return this.parent.verticalWidth;
  }

  // works like a charm
  getClosestSnappingPoint(x, y) {
    const center = this.parent.getNotesCenterLineX();

    // above highest line
    const highestAvailableSnappingPointY = this.getLineY(
      -this.availableLinesAbove
    );
    this.getTopLineY() - this.availableLinesAbove * spaceBetweenStaffLines;
    if (y < highestAvailableSnappingPointY) {
      return {
        x: center,
        y: highestAvailableSnappingPointY,
        lineNumber: -this.availableLinesAbove,
      };
    }

    // below lowest line
    const lowestAvailableSnappingPointY = this.getLineY(
      4 + this.availableLinesBelow
    );
    if (y > lowestAvailableSnappingPointY) {
      return {
        x: center,
        y: lowestAvailableSnappingPointY,
        lineNumber: 4 + this.availableLinesBelow,
      };
    }

    const firstSnappingPointTop =
      this.getTopLineY() - spaceBetweenStaffLines * 0.25;
    const diff = y - firstSnappingPointTop;
    const lineNumber = Math.floor(diff / (spaceBetweenStaffLines * 0.5)) * 0.5;
    return {
      x: center,
      y: this.getLineY(lineNumber),
      lineNumber: lineNumber,
    };
  }

  getTopLineY() {
    return this.isUpperStaff
      ? this.y + upperStaffUpperMargin
      : this.y + lowerStaffUpperMargin;
  }

  getLineY(lineNumber) {
    return this.getTopLineY() + GrandStaff.getLineOffset(lineNumber);
  }

  isOver(x, y) {
    const isOverCheck =
      x > this.x &&
      x < this.x + this.getWidth() &&
      y > this.y &&
      y < this.y + this.height;
    if (!isOverCheck) {
      return {};
    }

    const note1Distance = this.#getDistance(this.note1, y);
    const note2Distance = this.#getDistance(this.note2, y);

    if (
      this.note2 &&
      note2Distance < note1Distance &&
      note2Distance <= spaceBetweenStaffLines / 2
    ) {
      return { twoNotes: this, note: this.note2 };
    }

    if (this.note1 && note1Distance <= spaceBetweenStaffLines / 2) {
      return { twoNotes: this, note: this.note1 };
    }

    return { twoNotes: this };
  }

  #getDistance(note, y) {
    if (!note) return Infinity;

    const noteY = this.getLineY(note.line);
    return Math.abs(y - noteY);
  }

  draw() {
    this.#drawBoundingBox();

    // no note to draw
    if (this.isEmpty()) {
      return;
    }

    const notesCenterLineX = this.parent.getNotesCenterLineX();
    const note1Y = this.getLineY(this.note1.line);

    // note1 to draw, normal draw, no overlapping
    if (this.hasEmptyNote()) {
      this.note1.draw(notesCenterLineX, note1Y);
      this.#drawAdditionalLines(
        notesCenterLineX,
        this.note1.getNoteWidth(false) * 2
      );
      return;
    }

    // two notes to draw
    const note2Y = this.getLineY(this.note2.line);
    const lineDistBetweenNotes = Math.abs(this.note1.line - this.note2.line);
    const noteHeadWidth = this.note1.getNoteWidth(false);

    const notesXOffset = this.#drawTwoNotes(
      notesCenterLineX,
      lineDistBetweenNotes,
      note1Y,
      note2Y,
      noteHeadWidth
    );

    const additionalLineWidth =
      notesXOffset === 0 ? noteHeadWidth * 2 : noteHeadWidth * 3;
    this.#drawAdditionalLines(notesCenterLineX, additionalLineWidth);

    const accidentalsXOffset =
      notesCenterLineX -
      notesXOffset -
      noteHeadWidth / 2 -
      spaceBetweenNoteAndAccidental;
    this.#drawAccidentals(accidentalsXOffset, note1Y, note2Y);

    this.#drawDots(notesCenterLineX + notesXOffset);
  }

  drawAdditionalLinesOnAdding(lineNumber, noteWidth) {
    const lineX = this.parent.getNotesCenterLineX();
    const lineX0 = lineX - noteWidth;
    const lineX1 = lineX + noteWidth;

    const note1Line = this.note1 ? this.note1.line : 0;

    for (let i = Math.min(note1Line - 1, -1); i >= lineNumber; i--) {
      const lineY = this.getLineY(i);
      line(lineX0, lineY, lineX1, lineY);
    }

    for (let i = Math.max(note1Line + 1, 5); i <= lineNumber; i++) {
      const lineY = this.getLineY(i);
      line(lineX0, lineY, lineX1, lineY);
    }
  }

  #drawAdditionalLines(additionalLineCenterX, additionalLineWidth) {
    let minLine = 0;
    let maxLine = 4;
    if (this.note1) {
      minLine = Math.min(this.note1.line, minLine);
      maxLine = Math.max(this.note1.line, maxLine);
    }
    if (this.note2) {
      minLine = Math.min(this.note2.line, minLine);
      maxLine = Math.max(this.note2.line, maxLine);
    }

    const additionalLineX0 = additionalLineCenterX - additionalLineWidth / 2;
    const additionalLineX1 = additionalLineCenterX + additionalLineWidth / 2;

    for (let i = -1; i >= minLine; i--) {
      const lineY = this.getLineY(i);
      line(additionalLineX0, lineY, additionalLineX1, lineY);
    }

    for (let i = 5; i <= maxLine; i++) {
      const lineY = this.getLineY(i);
      line(additionalLineX0, lineY, additionalLineX1, lineY);
    }
  }

  #drawDots(notesXOffset) {
    // notes dont have dots, no need to draw
    if (!this.note1.hasDot) {
      return;
    }

    const note1Line = this.note1.line;
    const note2Line = this.note2.line;

    // if both notes are on the same line, draw just one dot
    // dot is moved between lines
    if (note1Line === note2Line) {
      const note1Width = this.note1.getNoteWidth(false);
      const dotX = notesXOffset + note1Width / 2 + dotDiameter + 2;
      const dotY = this.getLineY(Math.floor(note1Line - 0.5) + 0.5);
      this.note1.drawDot(dotX, dotY);
      return;
    }

    // notes arent on the same line, draw two notes
    // upper dot is moved between lines upwards, lower downwards
    if (note1Line < note2Line) {
      const note2Width = this.note2.getNoteWidth(true);
      const dotX = notesXOffset + note2Width / 2 + dotDiameter + 2;
      const dot1Y = this.getLineY(Math.floor(note1Line - 0.5) + 0.5);
      const dot2Y = this.getLineY(Math.ceil(note2Line + 0.5) - 0.5);

      this.note1.drawDot(dotX, dot1Y);
      this.note2.drawDot(dotX, dot2Y);
    } else {
      const note1Width = this.note1.getNoteWidth(true);
      const dotX = notesXOffset + note1Width / 2 + dotDiameter + 2;
      const dot1Y = this.getLineY(Math.ceil(note1Line + 0.5) - 0.5);
      const dot2Y = this.getLineY(Math.floor(note2Line - 0.5) + 0.5);

      this.note1.drawDot(dotX, dot1Y);
      this.note2.drawDot(dotX, dot2Y);
    }
  }

  #drawTwoNotes(
    notesCenterLineX,
    lineDistBetweenNotes,
    note1Y,
    note2Y,
    noteHeadWidth
  ) {
    let notesXOffset = 0;

    // overlap need to be separated
    if (
      lineDistBetweenNotes === 0.5 ||
      (lineDistBetweenNotes > 0 &&
        lineDistBetweenNotes < 3 &&
        !this.isNoteFacingUpAboveNoteFacingDown())
    ) {
      notesXOffset += noteHeadWidth / 2;

      if (note1Y < note2Y) {
        this.note1.drawNote(notesCenterLineX - notesXOffset, note1Y);
        this.note2.drawNote(notesCenterLineX + notesXOffset, note2Y);
      } else {
        this.note1.drawNote(notesCenterLineX + notesXOffset, note1Y);
        this.note2.drawNote(notesCenterLineX - notesXOffset, note2Y);
      }
    } else {
      this.note1.drawNote(notesCenterLineX, note1Y);
      this.note2.drawNote(notesCenterLineX, note2Y);
    }

    return notesXOffset;
  }

  #drawAccidentals(accidentalsXOffset, accidental1Y, accidental2Y) {
    const note1HasAccidental = this.note1.hasAccidental();
    const note2HasAccidental = this.note2.hasAccidental();
    // no accidental to draw
    if (!note1HasAccidental && !note2HasAccidental) {
      return;
    }

    // if both notes are on the same line and have same accidentals, draw just one
    if (
      accidental1Y === accidental2Y &&
      this.note1.getAccidentalName() === this.note2.getAccidentalName()
    ) {
      const accidentalWidth = this.note1.accidental.getWidth();
      this.note1.drawAccidental(
        accidentalsXOffset - accidentalWidth / 2,
        accidental1Y
      );
      return;
    }

    const accidental1Height = this.note1.accidental.getHeight();
    const accidental2Height = this.note2.accidental.getHeight();
    const checkedAccidentalHeight =
      Math.max(accidental1Height, accidental2Height) + 2; // horizontal margin between accidentals

    const verticalMarginBetweenAccidentals = 3;
    if (accidental1Y < accidental2Y) {
      const accidental1Width = this.note1.accidental.getWidth();
      const offset =
        accidental2Y - checkedAccidentalHeight < accidental1Y
          ? accidental1Width + verticalMarginBetweenAccidentals
          : 0;

      // upper note without aditional offset
      this.note1.drawAccidental(
        accidentalsXOffset - accidental1Width / 2,
        accidental1Y
      );
      // lower note with potential aditional offset
      this.note2.drawAccidental(
        accidentalsXOffset - offset - accidental1Width / 2,
        accidental2Y
      );
    } else {
      const accidental2Width = this.note2.accidental.getWidth();
      const offset =
        accidental1Y - checkedAccidentalHeight < accidental2Y
          ? accidental2Width + verticalMarginBetweenAccidentals
          : 0;

      // upper note without aditional offset
      this.note2.drawAccidental(
        accidentalsXOffset - accidental2Width / 2,
        accidental2Y
      );
      // lower note with potential aditional offset
      this.note1.drawAccidental(
        accidentalsXOffset - offset - accidental2Width / 2,
        accidental1Y
      );
    }
  }

  #drawBoundingBox() {
    push();
    noFill();
    rect(this.x, this.y, this.getWidth(), this.height);
    pop();
  }
}
