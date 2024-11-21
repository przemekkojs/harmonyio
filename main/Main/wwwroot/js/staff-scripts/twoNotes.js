class TwoNotes {
  constructor(parent, isUpperStaff, note1 = null, note2 = null) {
    this.note1 = note1;
    this.note2 = note2;
    this.isUpperStaff = isUpperStaff;

    this.parent = parent;
    this.height = isUpperStaff ? upperStaffHeight : lowerStaffHeight;
    this.availableLinesAbove = isUpperStaff ? 2 : 3;
    this.availableLinesBelow = isUpperStaff ? 2.5 : 2;

    this.prevNote1Line = -20;
    this.prevNote2Line = -20;
  }

  toJson() {
    let jsonResult = [];

    let commonAccidentalName = "";
    // check if notes are on the same line and one of them has accidental and other doesnt
    if (
      this.note1 !== null &&
      this.note2 !== null &&
      this.note1.line == this.note2.line
    ) {
      if (this.note1.hasAccidental() && !this.note2.hasAccidental()) {
        commonAccidentalName = this.note1.accidental.getChar();
      } else if (!this.note1.hasAccidental() && this.note2.hasAccidental()) {
        commonAccidentalName = this.note2.accidental.getChar();
      }
    }

    let note1Json;
    if (this.note1 !== null) {
      note1Json = this.note1.toJson();
      if (commonAccidentalName !== "") {
        note1Json.accidentalName = commonAccidentalName;
      }
      note1Json.voice = this.#determineVoice(this.note1, true);
      jsonResult.push(note1Json);
    }

    let note2Json;
    if (this.note2 !== null) {
      note2Json = this.note2.toJson();
      if (commonAccidentalName !== "") {
        note2Json.accidentalName = commonAccidentalName;
      }
      note2Json.voice = this.#determineVoice(this.note2, false);
      jsonResult.push(note2Json);
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

  #determineVoice(note, determiningNote1) {
    // both notes are present and they are full note
    if (
      this.note1 !== null &&
      this.note2 !== null &&
      this.note1.getBaseValue() === 1
    ) {
      const curNoteLine = note.line;
      const otherNoteLine = determiningNote1
        ? this.note2.line
        : this.note1.line;

      // full notes are on the same line, note1 will be facing up and note2 will be facing down
      if (curNoteLine === otherNoteLine) {
        if (determiningNote1) {
          if (this.isUpperStaff) {
            return Note.noteVoices.soprano;
          } else {
            return Note.noteVoices.tenore;
          }
        } else {
          if (this.isUpperStaff) {
            return Note.noteVoices.alto;
          } else {
            return Note.noteVoices.bass;
          }
        }
      } else {
        if (curNoteLine < otherNoteLine && this.isUpperStaff) {
          return Note.noteVoices.soprano;
        } else if (curNoteLine > otherNoteLine && this.isUpperStaff) {
          return Note.noteVoices.alto;
        } else if (curNoteLine < otherNoteLine && !this.isUpperStaff) {
          return Note.noteVoices.tenore;
        } else {
          return Note.noteVoices.bass;
        }
      }
    }

    // notes arent full note so normal check
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

  #calculateDotPositions(notesCenterLineX) {
    // no need to calculate dot positions, since there are no dots to draw
    if (!this.note1.hasDot) {
      return;
    }

    // there is only one note to draw, so dot is always next to it, moved between lines in the direction the note is facing
    if (this.hasEmptyNote()) {
      const note1Line = this.note1.line;
      const note1Width = this.note1.getNoteWidth(false);
      this.dotX = this.note1X + note1Width / 2 + dotDiameter + 2;

      // note on line
      if (Math.floor(note1Line) === note1Line) {
        this.dot1Y = this.note1.isFacingUp
          ? this.note1Y - spaceBetweenStaffLines / 2
          : this.note1Y + spaceBetweenStaffLines / 2;
      } else {
        // note between lines
        this.dot1Y = this.note1Y;
      }
      return;
    }

    const note1Line = this.note1.line;
    const note2Line = this.note2.line;

    // if both notes are on the same line, draw just one dot
    // dot is moved between lines upwards
    if (note1Line === note2Line) {
      const note1Width = this.note1.getNoteWidth(false);
      this.dotX = this.note1X + note1Width / 2 + dotDiameter + 2;
      this.dot1Y = this.getLineY(Math.floor(note1Line - 0.5) + 0.5);
      this.dot2Y = this.dot1Y;
      return;
    }

    // notes arent on the same line, draw two notes
    // upper dot is moved between lines upwards, lower downwards
    if (note1Line < note2Line) {
      const note2Width = this.note2.getNoteWidth(true);
      this.dotX =
        notesCenterLineX + this.notesXOffset + note2Width / 2 + dotDiameter + 2;
      this.dot1Y = this.getLineY(Math.floor(note1Line - 0.5) + 0.5);
      this.dot2Y = this.getLineY(Math.ceil(note2Line + 0.5) - 0.5);
    } else {
      const note1Width = this.note1.getNoteWidth(true);
      this.dotX =
        notesCenterLineX + this.notesXOffset + note1Width / 2 + dotDiameter + 2;
      this.dot1Y = this.getLineY(Math.ceil(note1Line + 0.5) - 0.5);
      this.dot2Y = this.getLineY(Math.floor(note2Line - 0.5) + 0.5);
    }
  }

  #calculateAccidentalPositions(notesCenterLineX) {
    if (this.hasEmptyNote()) {
      this.accidental1X =
        this.note1X -
        this.note1.getNoteWidth() / 2 -
        this.note1.accidental.getWidth() / 2 -
        spaceBetweenNoteAndAccidental;
      this.accidental1Y = this.note1Y;
      return;
    }

    const note1HasAccidental = this.note1.hasAccidental();
    const note2HasAccidental = this.note2.hasAccidental();
    // no accidental to draw
    if (!note1HasAccidental && !note2HasAccidental) {
      return;
    }

    this.accidental1Y = this.note1Y;
    this.accidental2Y = this.note2Y;

    const accidentalsXOffset =
      notesCenterLineX -
      this.notesXOffset -
      this.note1.getNoteWidth(false) / 2 -
      spaceBetweenNoteAndAccidental;

    // if both notes are on the same line and have same accidentals, draw just one
    if (
      this.accidental1Y === this.accidental2Y &&
      this.note1.getAccidentalName() === this.note2.getAccidentalName()
    ) {
      const accidentalWidth = this.note1.accidental.getWidth();
      this.accidental1X = accidentalsXOffset - accidentalWidth / 2;
      return;
    }

    const accidental1Height = this.note1.accidental.getHeight();
    const accidental2Height = this.note2.accidental.getHeight();

    const horizontalMarginBetweenAccidentals = 3;
    const accidental1Width = this.note1.accidental.getWidth();
    const accidental2Width = this.note2.accidental.getWidth();
    if (this.accidental1Y <= this.accidental2Y) {
      const accidental1BottomY = this.accidental1Y + accidental1Height / 2;
      const accidental2TopY = this.accidental2Y - accidental2Height / 2;

      const offset =
        accidental1BottomY >= accidental2TopY
          ? accidental1Width + horizontalMarginBetweenAccidentals
          : 0;

      this.accidental1X = accidentalsXOffset - accidental1Width / 2;
      this.accidental2X = accidentalsXOffset - offset - accidental2Width / 2;
    } else {
      const accidental2BottomY = this.accidental2Y + accidental2Height / 2;
      const accidental1TopY = this.accidental1Y - accidental1Height / 2;
      const offset =
        accidental2BottomY >= accidental1TopY
          ? accidental2Width + horizontalMarginBetweenAccidentals
          : 0;

      this.accidental2X = accidentalsXOffset - accidental2Width / 2;
      this.accidental1X = accidentalsXOffset - offset - accidental1Width / 2;
    }
  }

  #calculateNotePositions(notesCenterLineX) {
    if (this.hasEmptyNote()) {
      this.notesXOffset = 0;
      this.note1X = notesCenterLineX;
      this.note1Y = this.getLineY(this.note1.line);
      return;
    }

    this.note1X = notesCenterLineX;
    this.note2X = notesCenterLineX;
    this.note1Y = this.getLineY(this.note1.line);
    this.note2Y = this.getLineY(this.note2.line);

    this.notesXOffset = 0;

    const lineDistBetweenNotes = Math.abs(this.note1.line - this.note2.line);
    const noteHeadWidth = this.note1.getNoteWidth(false);

    if (this.note1.getBaseValue() == 1) {
      if (lineDistBetweenNotes == 0.5) {
        this.notesXOffset += noteHeadWidth / 2;
        if (this.note1Y < this.note2Y) {
          this.note1X -= this.notesXOffset;
          this.note2X += this.notesXOffset;
        } else {
          this.note1X += this.notesXOffset;
          this.note2X -= this.notesXOffset;
        }
      }

      return;
    }

    // overlap need to be separated
    if (
      lineDistBetweenNotes === 0.5 ||
      (lineDistBetweenNotes > 0 &&
        lineDistBetweenNotes < 3 &&
        !this.isNoteFacingUpAboveNoteFacingDown())
    ) {
      this.notesXOffset += noteHeadWidth / 2;

      if (this.note1Y < this.note2Y) {
        this.note1X -= this.notesXOffset;
        this.note2X += this.notesXOffset;
      } else {
        this.note1X += this.notesXOffset;
        this.note2X -= this.notesXOffset;
      }
    }
  }

  #calculateAdditionalLinesPositions() {
    const noteHeadWidth = this.note1.getNoteWidth(false);
    this.additionalLineWidth =
      this.notesXOffset === 0 ? noteHeadWidth * 2 : noteHeadWidth * 3;

    this.minLine = 0;
    this.maxLine = 4;
    if (this.note1) {
      this.minLine = Math.min(this.note1.line, this.minLine);
      this.maxLine = Math.max(this.note1.line, this.maxLine);
    }
    if (this.note2) {
      this.minLine = Math.min(this.note2.line, this.minLine);
      this.maxLine = Math.max(this.note2.line, this.maxLine);
    }
  }

  #recalculateNoteElementsPositions(notesCenterLineX) {
    this.#calculateNotePositions(notesCenterLineX);
    this.#calculateDotPositions(notesCenterLineX);
    this.#calculateAccidentalPositions(notesCenterLineX);
    this.#calculateAdditionalLinesPositions();
  }

  draw() {
    this.#drawHintArea();

    // no note to draw
    if (this.isEmpty()) {
      this.prevNote1Line = -20;
      this.prevNote2Line = -20;
      return;
    }

    const notesCenterLineX = this.parent.getNotesCenterLineX();

    const note1Line = this.note1.line;
    const note2Line = this.note2 === null ? -20 : this.note2.line;
    if (note1Line !== this.prevNote1Line || note2Line !== this.prevNote2Line) {
      this.#recalculateNoteElementsPositions(notesCenterLineX);
    }
    this.prevNote1Line = note1Line;
    this.prevNote2Line = note2Line;

    this.#drawAdditionalLines(notesCenterLineX);
    this.#drawNotes();
    this.#drawAccidentals();
    this.#drawDots();
  }

  drawAdditionalLinesOnAdding(lineNumber, noteWidth) {
    console.log(lineNumber);
    const lineX = this.parent.getNotesCenterLineX();
    const lineX0 = lineX - noteWidth;
    const lineX1 = lineX + noteWidth;

    const note1Line = this.note1 ? this.note1.line : 2;

    strokeWeight(2);
    stroke(0);
    for (let i = Math.min(Math.floor(note1Line), -1); i >= lineNumber; i--) {
      const lineY = this.getLineY(i);
      line(lineX0, lineY, lineX1, lineY);
    }

    for (let i = Math.max(Math.ceil(note1Line), 5); i <= lineNumber; i++) {
      const lineY = this.getLineY(i);
      line(lineX0, lineY, lineX1, lineY);
    }
  }

  #drawAdditionalLines(additionalLineCenterX) {
    const additionalLineX0 =
      additionalLineCenterX - this.additionalLineWidth / 2;
    const additionalLineX1 =
      additionalLineCenterX + this.additionalLineWidth / 2;

    strokeWeight(2);
    stroke(0);
    for (let i = -1; i >= this.minLine; i--) {
      const lineY = this.getLineY(i);
      line(additionalLineX0, lineY, additionalLineX1, lineY);
    }

    for (let i = 5; i <= this.maxLine; i++) {
      const lineY = this.getLineY(i);
      line(additionalLineX0, lineY, additionalLineX1, lineY);
    }
  }

  #drawDots() {
    // notes dont have dots, no need to draw
    if (!this.note1.hasDot) {
      return;
    }

    if (this.hasEmptyNote() || this.note1.line === this.note2.line) {
      this.note1.drawDot(this.dotX, this.dot1Y);
      return;
    }

    this.note1.drawDot(this.dotX, this.dot1Y);
    this.note2.drawDot(this.dotX, this.dot2Y);
  }

  #drawNotes() {
    if (this.hasEmptyNote()) {
      this.note1.drawNote(this.note1X, this.note1Y);
      return;
    }

    this.note1.drawNote(this.note1X, this.note1Y);
    this.note2.drawNote(this.note2X, this.note2Y);
  }

  #drawAccidentals() {
    if (this.hasEmptyNote()) {
      this.note1.drawAccidental(this.accidental1X, this.accidental1Y);
      return;
    }

    const note1HasAccidental = this.note1.hasAccidental();
    const note2HasAccidental = this.note2.hasAccidental();
    // no accidental to draw
    if (!note1HasAccidental && !note2HasAccidental) {
      return;
    }

    // if both notes are on the same line and have same accidentals, draw just one
    if (
      this.accidental1Y === this.accidental2Y &&
      this.note1.getAccidentalName() === this.note2.getAccidentalName()
    ) {
      this.note1.drawAccidental(this.accidental1X, this.accidental1Y);
      return;
    }

    this.note1.drawAccidental(this.accidental1X, this.accidental1Y);
    this.note2.drawAccidental(this.accidental2X, this.accidental2Y);
  }

  #drawHintArea() {
    if (!drawAddNoteHint) {
      return;
    }
    if (this.note1 !== null && this.note2 !== null) {
      return;
    }
    if (
      (this.note1 !== null && this.note2 === null) ||
      this.parent.shouldDrawHintArea(this.isUpperStaff)
    ) {
      push();
      fill(81, 125, 213, 20);
      noStroke();
      const thirdW = this.getWidth() / 3;
      rect(
        this.parent.getNotesCenterLineX() - thirdW,
        this.y,
        thirdW * 2,
        this.height
      );
      pop();
    }
  }
}
