class Vertical {
  constructor(
    parent,
    index,
    upperStaff = new TwoNotes(this, true),
    lowerStaff = new TwoNotes(this, false)
  ) {
    this.parent = parent; // bar in which the vertical is
    this.index = index; // vertical index in bar

    this.upperStaff = upperStaff;
    this.lowerStaff = lowerStaff;
    this.verticalWidth = getVerticalWidth();
  }

  setFunctionSymbol(task) {
    this.functionSymbol = FunctionSymbol.fromJson(this, task);
  }

  toJson() {
    const upperJson = this.upperStaff.toJson();
    const lowerJson = this.lowerStaff.toJson();

    const concatenated = upperJson.concat(lowerJson);

    for (let i = 0; i < concatenated.length; i++) {
      concatenated[i].verticalIndex = this.index;
    }

    return concatenated;
  }

  loadNoteFromJson(noteJson) {
    const voice = noteJson.voice;
    if (voice === Note.noteVoices.soprano || voice === Note.noteVoices.alto) {
      this.upperStaff.loadNote(Note.fromJson(noteJson));
    } else {
      this.lowerStaff.loadNote(Note.fromJson(noteJson));
    }
  }

  getSlotsTaken() {
    return max(
      this.upperStaff.getSlotsTaken(),
      this.lowerStaff.getSlotsTaken()
    );
  }

  getNotesCenterLineX() {
    return (
      this.getX() + this.verticalWidth * 0.75 - spaceBetweenNoteAndAccidental
    );
  }

  isEmpty() {
    return this.upperStaff.isEmpty() && this.lowerStaff.isEmpty();
  }

  // needs to be called from twoNotes.canAddNote
  canAddNote(note, isUpperStaff) {
    let otherStaffHasSameValue;

    // chceck if other staff has same value
    if (isUpperStaff) {
      otherStaffHasSameValue = this.lowerStaff.hasSameValue(note);
    } else {
      otherStaffHasSameValue = this.upperStaff.hasSameValue(note);
    }
    if (otherStaffHasSameValue) {
      return true;
    }

    // at this point both staffs are empty for this vertical, need to check if can add this note to bar
    return this.parent.canAddNote(note, this.index);
  }

  canClear() {
    // can clear vertical if its not empty and is last not empty vertical
    return !this.isEmpty() && this.parent.canClearVertical(this.index);
  }

  clear() {
    if (this.canClear()) {
      this.upperStaff = new TwoNotes(this, true);
      this.lowerStaff = new TwoNotes(this, false);

      this.parent.updateVerticalPositions();
    }
  }

  getX() {
    return this.upperStaff.x;
  }

  getY() {
    return this.upperStaff.y;
  }

  getWidth() {
    const slotsTaken = this.getSlotsTaken();
    if (slotsTaken === 0) {
      const barSlotsAvailableCount = this.parent.getSlotsAvailable();
      const emptyVerticalsCount = this.parent.countEmptyVerticals();

      const widthOfEmptyVerticals = emptyVerticalsCount * this.verticalWidth;
      const widthOfEmptySlots = barSlotsAvailableCount * slotWidth;

      if (widthOfEmptySlots - widthOfEmptyVerticals >= 0) {
        return widthOfEmptySlots / emptyVerticalsCount;
      } else {
        return this.verticalWidth;
      }
    }

    const slotsWidthNeeded = slotsTaken * slotWidth;
    if (slotsWidthNeeded - this.verticalWidth >= 0) {
      return slotsWidthNeeded;
    } else {
      const verticalWidthToSlotWidthRatio = this.verticalWidth / slotWidth;
      return (
        this.verticalWidth +
        max(slotsTaken - verticalWidthToSlotWidthRatio, 0) * slotWidth
      );
    }
  }

  getHeight() {
    return this.upperStaff.height + this.lowerStaff.height;
  }

  updatePosition(previousVertical) {
    let x;
    const y = this.parent.y;
    if (previousVertical) {
      x = previousVertical.getX() + previousVertical.getWidth();
    } else {
      x = this.parent.x + barMargin;
    }

    this.upperStaff.updatePosition(x, y);
    this.lowerStaff.updatePosition(x, y + this.upperStaff.height);
  }

  isOver(x, y) {
    const isOverCheck =
      x > this.getX() &&
      x < this.getX() + this.verticalWidth &&
      y > this.getY() &&
      y < this.getY() + this.upperStaff.height + this.lowerStaff.height;
    if (!isOverCheck) {
      return {};
    }

    // check if is over upper staff
    const upperStaffResult = this.upperStaff.isOver(x, y);
    if ("twoNotes" in upperStaffResult) {
      return {
        vertical: this,
        ...upperStaffResult,
      };
    }
    // check if is over lower staff
    const lowerStaffResult = this.lowerStaff.isOver(x, y);
    if ("twoNotes" in lowerStaffResult) {
      return {
        vertical: this,
        ...lowerStaffResult,
      };
    }
    // edge case when x,y is on line between staffs
    return {
      vertical: this,
    };
  }

  draw() {
    this.upperStaff.draw();
    this.lowerStaff.draw();

    const functionSymbolX = this.getX() + this.verticalWidth / 2;
    const functionSymbolY = this.getY() + this.getHeight() + taskHeight / 2;
    this.functionSymbol.draw(functionSymbolX, functionSymbolY);
  }

  drawArea() {
    push();
    fill(255, 153, 153, 50);
    noStroke();
    rect(this.getX(), this.getY(), this.verticalWidth, this.getHeight());
    pop();
  }
}
