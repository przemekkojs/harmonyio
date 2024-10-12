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
  }

  getSlotsTaken() {
    return max(
      this.upperStaff.getSlotsTaken(),
      this.lowerStaff.getSlotsTaken()
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
      console.log("other has same value");
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
      let barSlotsAvailable = this.parent.getSlotsAvailable();
      let emptyVerticals = this.parent.countEmptyVerticals();
      let slotsPerVertical = barSlotsAvailable / emptyVerticals;

      return slotsPerVertical * slotWidth;
    }

    return this.getSlotsTaken() * slotWidth;
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
      x < this.getX() + this.upperStaff.width &&
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
  }

  drawArea() {
    push();
    fill(255, 153, 153, 50);
    noStroke();
    rect(this.getX(), this.getY(), slotWidth, this.getHeight());
    pop();
  }
}
