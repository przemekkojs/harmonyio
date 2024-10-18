class Bar {
  constructor(parent, numberOfVerticals, slotsPerBar) {
    this.parent = parent;
    this.verticals = [];
    this.slotsPerBar = slotsPerBar;

    for (let i = 0; i < numberOfVerticals; i++) {
      this.verticals.push(new Vertical(this, i));
    }
  }

  toJson() {
    return this.verticals.map((vertical) => vertical.toJson()).flat();
  }

  getSlotsTaken() {
    return this.verticals.reduce(
      (sum, vertical) => sum + vertical.getSlotsTaken(),
      0
    );
  }

  getSlotsAvailable() {
    return this.slotsPerBar - this.getSlotsTaken();
  }

  countEmptyVerticals() {
    return this.verticals.filter((vertical) => vertical.getSlotsTaken() === 0)
      .length;
  }

  // needs to be called from vertical.canAddNote
  canAddNote(note, verticalIndex) {
    // cant add if left vertical is empty
    if (
      verticalIndex > 0 &&
      this.verticals[verticalIndex - 1].getSlotsTaken() == 0
    ) {
      //console.log("left is empty");
      return false;
    }

    // check metre
    const slotsAvailable = this.getSlotsAvailable();
    const emptyVerticals = this.countEmptyVerticals();
    const noteSlots = note.getSlotsTaken();

    // cant add note to bar if there is less slots than note needs
    if (slotsAvailable < noteSlots) {
      //console.log("no slots left");
      return false;
    }

    // if adding note to last empty vertical, it has to have same slots as available
    if (emptyVerticals === 1 && noteSlots !== slotsAvailable) {
      return false;
    }

    // currently adding to one of empty vertical, so check if remainging empty verticals have at least one slot each to take
    const emptyVerticalsAfterAddingNote = emptyVerticals - 1;
    if (slotsAvailable - noteSlots < emptyVerticalsAfterAddingNote) {
      return false;
    }

    return true;
  }

  // needs to be called from vertical.canClear
  canClearVertical(verticalIndex) {
    // can always clear last vertical in bar or vertical that doesnt have non empty vertical on the right
    return (
      verticalIndex === this.verticals.length - 1 ||
      this.verticals[verticalIndex + 1].isEmpty()
    );
  }

  calculateBarWidth() {
    const verticalsWidth = this.verticals.reduce(
      (sum, vertical) => sum + vertical.getWidth(),
      0
    );

    return 2 * barMargin + verticalsWidth;
  }

  updatePosition(x, y, width, height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;

    this.updateVerticalPositions();
  }

  updateVerticalPositions() {
    for (let i = 0; i < this.verticals.length; i++) {
      const previousVertical = i > 0 ? this.verticals[i - 1] : null;
      this.verticals[i].updatePosition(previousVertical);
    }

    if (this.calculateBarWidth() !== this.width) {
      this.parent.calculateBarPositions();
    }
  }

  isOver(x, y) {
    const isInside =
      x > this.x &&
      x < this.x + this.width &&
      y > this.y &&
      y < this.y + this.height;
    if (!isInside) {
      return {};
    }

    for (let i = 0; i < this.verticals.length; i++) {
      const vertical = this.verticals[i].isOver(x, y);
      if ("vertical" in vertical) {
        return { bar: this, ...vertical };
      }
    }
    return { bar: this };
  }

  draw() {
    this.#drawBarLine();
    this.#drawBoundingBox();

    for (let i = 0; i < this.verticals.length; i++) {
      this.verticals[i].draw();
    }
  }

  #drawBarLine() {
    push();
    strokeWeight(2);
    translate(this.x + this.width, this.y + upperStaffUpperMargin);
    line(0, 0, 0, braceHeight);
    pop();
  }

  #drawBoundingBox() {
    push();
    noFill();
    rect(this.x, this.y, this.width, this.height);
    pop();
  }
}
