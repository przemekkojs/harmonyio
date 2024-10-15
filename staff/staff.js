class GrandStaff {
  constructor(
    numberOfVerticalsPerBarList,
    width,
    metre = new Metre(4, 4),
    keySignature = new KeySignature(7)
  ) {
    this.keySignature = keySignature;
    this.metre = metre;

    const slotsPerBar = this.metre.slotsPerBar();
    this.bars = [];
    for (let i = 0; i < numberOfVerticalsPerBarList.length; i++) {
      this.bars.push(new Bar(numberOfVerticalsPerBarList[i], slotsPerBar));
    }

    this.keySignatureOffset =
      keyOffset + symbols.violinKey.width + 0.4 * spaceBetweenStaffLines;
    this.metreOffset =
      this.keySignatureOffset +
      this.keySignature.getWidth() +
      0.2 * spaceBetweenStaffLines;
    this.dynamicElementsOffset = this.metreOffset + this.metre.getMetreWidth();

    this.setWidth(width);
  }

  toJson() {
    let json = [];
    for (let i = 0; i < this.bars.length; i++) {
      let barJson = this.bars[i].toJson();

      for (let j = 0; j < barJson.length; j++) {
        barJson[j].barIndex = i;
      }
      json.push(...barJson);
    }
    return JSON.stringify(json);
  }

  setWidth(width) {
    this.width = width;
    this.staffWorkspaceWidth = this.width - this.dynamicElementsOffset;
    this.calculateBarPositions();
  }

  calculateBarPositions() {
    let barX;
    let barY;
    const barWidth = Bar.calculateBarWidth(this.metre);
    const barHeight = doubleGrandStaffHeight;

    let curStaffVertically = 0;
    let curBarHorizontally = 0;
    for (let i = 0; i < this.bars.length; i++) {
      if (
        curStaffVertically === 0 &&
        curBarHorizontally === 0 &&
        this.staffWorkspaceWidth - (curBarHorizontally + 1) * barWidth < 0
      ) {
        //TODO canvas is too small to draw
        console.log("canvas too small!");
      }

      // need to go to next line
      if (this.staffWorkspaceWidth - (curBarHorizontally + 1) * barWidth < 0) {
        curStaffVertically += 1;
        curBarHorizontally = 0;
      }

      barX = this.dynamicElementsOffset + curBarHorizontally * barWidth;
      barY = curStaffVertically * barHeight;
      this.bars[i].updatePosition(barX, barY, barWidth, barHeight);

      curBarHorizontally += 1;
    }

    this.numberOfStaffs = curStaffVertically + 1;
    resizeCanvas(
      canvasWidth,
      this.numberOfStaffs * doubleGrandStaffHeight + 50
    );
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
    for (let i = 0; i < this.bars.length; i++) {
      this.bars[i].draw();
    }
  }

  drawStaticElements() {
    const linesWidth = this.width - braceWidth;

    // draw lines n times
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
