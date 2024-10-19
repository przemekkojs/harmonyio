class GrandStaff {
  constructor(
    numberOfVerticalsPerBarList,
    minWidth,
    metre = new Metre(4, 4),
    keySignature = new KeySignature(7)
  ) {
    this.keySignature = keySignature;
    this.metre = metre;

    const slotsPerBar = this.metre.slotsPerBar();
    this.bars = [];
    for (let i = 0; i < numberOfVerticalsPerBarList.length; i++) {
      this.bars.push(
        new Bar(this, numberOfVerticalsPerBarList[i], slotsPerBar)
      );
    }

    this.keySignatureOffset =
      keyOffset + symbols.violinKey.width + 0.4 * spaceBetweenStaffLines;
    this.metreOffset =
      this.keySignatureOffset +
      this.keySignature.getWidth() +
      0.2 * spaceBetweenStaffLines;
    this.dynamicElementsOffset = this.metreOffset + this.metre.getMetreWidth();

    this.minWidth = minWidth;
  }

  init() {
    this.setWidth(this.minWidth);
    this.calculateBarPositions();
  }

  toJson() {
    let notesList = [];

    this.bars.forEach((bar, i) => {
      const barJson = bar.toJson();

      barJson.forEach((note) => {
        note.barIndex = i;
      });

      notesList.push(...barJson);
    });

    const signatureCount = this.keySignature.count;
    const signatureSymbol = this.keySignature.accidental.getName();
    const sharpsCount =
      signatureSymbol === Accidental.accidentals.sharp ? signatureCount : 0;
    const flatsCount =
      signatureSymbol === Accidental.accidentals.bemol ? signatureCount : 0;

    return JSON.stringify({
      notes: notesList,
      sharpsCount: sharpsCount,
      flatsCount: flatsCount,
      meterCount: this.metre.count,
      meterValue: this.metre.value,
    });
  }

  setWidth(width) {
    if (width < this.minWidth) {
      this.width = this.minWidth;
    } else {
      this.width = width;
    }

    this.staffWorkspaceWidth = this.width - this.dynamicElementsOffset;
    resizeCanvasHorizontally();
  }

  calculateBarPositions() {
    let barX;
    let barY;
    let curXOffset = 0;

    const barHeight = doubleGrandStaffHeight;

    let curStaffVertically = 0;
    let curBarHorizontally = 0;

    const barWidths = this.bars.map((bar) => bar.calculateBarWidth());
    const maxBarWidth = Math.max(...barWidths);

    if (this.staffWorkspaceWidth !== maxBarWidth) {
      this.setWidth(maxBarWidth + this.dynamicElementsOffset);
    }

    for (let i = 0; i < this.bars.length; i++) {
      const barWidth = barWidths[i];

      if (this.staffWorkspaceWidth - (curXOffset + barWidth) < 0) {
        curStaffVertically += 1;
        curBarHorizontally = 0;
        curXOffset = 0;
      }

      barX = this.dynamicElementsOffset + curXOffset;
      barY = curStaffVertically * barHeight;
      this.bars[i].updatePosition(barX, barY, barWidth, barHeight);

      curBarHorizontally += 1;
      curXOffset += barWidth;
    }

    if (this.numberOfStaffs !== curStaffVertically + 1) {
      this.numberOfStaffs = curStaffVertically + 1;

      resizeCanvasVertically();
    }
  }

  static getLineOffset(lineNumber) {
    return lineNumber * spaceBetweenStaffLines;
  }

  // this will need refactor for sure(? maybe not) but works for now
  isOver(x, y) {
    for (let i = 0; i < this.bars.length; i++) {
      const bar = this.bars[i].isOver(x, y);
      if ("bar" in bar) {
        return bar;
      }
    }
    return {};
  }

  // DRAWING SECTION
  draw() {
    this.drawStaticElements();
    this.drawDynamicElements();
  }

  drawDynamicElements() {
    const barsLength = this.bars.length;
    for (let i = 0; i < barsLength; i++) {
      this.bars[i].draw(i === barsLength - 1);
    }
  }

  drawStaticElements() {
    const linesWidth = this.width - braceWidth;

    // draw needed staffs (vertically)
    for (let i = 0; i < this.numberOfStaffs; i++) {
      push();
      translate(0, i * doubleGrandStaffHeight);

      this.#drawBrace(
        braceWidth / 2,
        upperStaffUpperMargin + braceHeight / 2,
        braceHeight,
        braceWidth
      );

      this.#drawSeparationLines(
        braceWidth,
        0,
        linesWidth,
        upperStaffHeight,
        lowerStaffHeight
      );

      translate(0, upperStaffUpperMargin);
      this.#drawBarLine(braceWidth, 0, braceHeight);

      //upper staff
      this.#drawSingleStaffLines(braceWidth, 0, linesWidth);
      this.#drawKey(
        symbols.violinKey.width / 2 + keyOffset,
        2.2 * spaceBetweenStaffLines,
        symbols.violinKey,
        true
      );
      this.keySignature.draw(this.keySignatureOffset, 0, true);
      this.metre.draw(this.metreOffset, 0);

      translate(
        0,
        upperStaffHeight + lowerStaffUpperMargin - upperStaffUpperMargin
      );

      // lower staff
      this.#drawSingleStaffLines(braceWidth, 0, linesWidth);
      this.#drawKey(keyOffset, 0, symbols.bassKey, false);
      this.keySignature.draw(this.keySignatureOffset, 0, false);
      this.metre.draw(this.metreOffset, 0);

      pop();
    }
  }

  #drawBrace(x, y, height, width) {
    push();
    translate(x, y);
    rotate(-PI / 2);
    imageMode(CENTER);
    image(symbols.curlyBrace, 0, 0, height, width);
    pop();
  }

  #drawBarLine(x, y, height) {
    push();
    strokeWeight(2);
    translate(x, y);
    line(0, 0, 0, height);
    pop();
  }

  #drawSeparationLines(x, y, width, upperStaffHeight, lowerStaffHeight) {
    push();
    translate(x, y);

    stroke(150);
    strokeWeight(1);

    line(0, 0, width, 0); // above
    translate(0, upperStaffHeight);
    line(0, 0, width, 0); // between
    translate(0, lowerStaffHeight);
    line(0, 0, width, 0); // below

    pop();
  }

  #drawSingleStaffLines(x, y, width) {
    push();
    translate(x, y);
    for (let i = 0; i < 5; i++) {
      const lineOffset = GrandStaff.getLineOffset(i);
      line(0, 0 + lineOffset, width, 0 + lineOffset);
    }
    pop();
  }

  #drawKey(x, y, keySymbol, centerSymbol = false) {
    push();
    translate(x, y);
    if (centerSymbol) {
      imageMode(CENTER);
    } else {
      imageMode(CORNER);
    }
    image(keySymbol, 0, 0);
    pop();
  }
}
