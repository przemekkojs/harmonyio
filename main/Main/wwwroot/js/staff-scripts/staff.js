class GrandStaff {
  constructor(
    // numberOfVerticalsPerBarList,
    minWidth
    // metre = new Metre(4, 4),
    // keySignature = new KeySignature(7)
  ) {
    // this.keySignature = keySignature;
    // this.metre = metre;

    // const slotsPerBar = this.metre.slotsPerBar();
    // this.bars = [];
    // for (let i = 0; i < numberOfVerticalsPerBarList.length; i++) {
    //   this.bars.push(
    //     new Bar(this, numberOfVerticalsPerBarList[i], slotsPerBar)
    //   );
    // }

    this.keySignatureOffset =
      keyOffset + symbols.violinKey.width + 0.4 * spaceBetweenStaffLines;
    // this.metreOffset =
    //   this.keySignatureOffset +
    //   this.keySignature.getWidth() +
    //   0.2 * spaceBetweenStaffLines;
    // this.dynamicElementsOffset = this.metreOffset + this.metre.getMetreWidth();

    this.minWidth = minWidth;
    this.isLoaded = false;
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

  #loadTaskFromJson(taskJsonString) {
    // task mock, for now only how many verticals per bar, metre and key signature
    const taskJson = {
      meterCount: 3,
      meterValue: 4,
      sharpsCount: 0,
      flatsCount: 7,
      verticalsPerBar: verticalsPerBarList,
    };

    this.keySignature =
      taskJson.sharpsCount > 0
        ? new KeySignature(
            taskJson.sharpsCount,
            new Accidental(Accidental.accidentals.sharp)
          )
        : new KeySignature(
            taskJson.flatsCount,
            new Accidental(Accidental.accidentals.bemol)
          );

    this.metre = new Metre(taskJson.meterCount, taskJson.meterValue);

    const slotsPerBar = this.metre.slotsPerBar();
    this.bars = [];
    for (let i = 0; i < taskJson.verticalsPerBar.length; i++) {
      this.bars.push(new Bar(this, taskJson.verticalsPerBar[i], slotsPerBar));
    }

    this.metreOffset =
      this.keySignatureOffset +
      this.keySignature.getWidth() +
      0.2 * spaceBetweenStaffLines;
    this.dynamicElementsOffset = this.metreOffset + this.metre.getMetreWidth();
  }

  #loadNotesFromJson(jsonString) {
    // json string is empty, empty staff
    if (jsonString === "") {
      return;
    }

    const json = JSON.parse(jsonString);
    const jsonNotes = json.notes;
    for (let i = 0; i < jsonNotes.length; i++) {
      const curNote = jsonNotes[i];
      const barIndex = curNote.barIndex;
      this.bars[barIndex].loadNoteFromJson(curNote);
    }
  }

  loadFromJson(taskJsonString, notesJsonString) {
    this.isLoaded = false;
    this.#loadTaskFromJson(taskJsonString);
    this.#loadNotesFromJson(notesJsonString);
    this.init();
    this.isLoaded = true;
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
    const windowHeight = $(window).height();
    const clientRectTop = canvas.elt.getBoundingClientRect().top;

    this.drawStaticElements(windowHeight, clientRectTop);
    this.drawDynamicElements(windowHeight, clientRectTop);
  }

  drawDynamicElements(windowHeight, clientRectTop) {
    const barsLength = this.bars.length;
    for (let i = 0; i < barsLength; i++) {
      // draw only visible bars
      const barYTop = clientRectTop + this.bars[i].y;
      const barYBottom = barYTop + this.bars[i].height;
      if (barYBottom < 0) {
        continue;
      }
      if (barYTop > windowHeight) {
        break;
      }

      this.bars[i].draw(i === barsLength - 1);
    }
  }

  drawStaticElements(windowHeight, clientRectTop) {
    push();
    const linesWidth = this.width - braceWidth;
    // draw needed staffs (vertically)
    for (let i = 0; i < this.numberOfStaffs; i++) {
      const y = i * doubleGrandStaffHeight;

      // draw only visible staffs
      const staffYTop = clientRectTop + y;
      const staffYBottom = staffYTop + doubleGrandStaffHeight;
      if (staffYBottom < 0) {
        continue;
      }
      if (staffYTop > windowHeight) {
        break;
      }

      const upperStaffFirstLineY = y + upperStaffUpperMargin;

      this.#drawBrace(
        braceWidth / 2,
        upperStaffFirstLineY + braceHeight / 2,
        braceHeight,
        braceWidth
      );

      this.#drawSeparationLines(
        braceWidth,
        y,
        linesWidth,
        upperStaffHeight,
        lowerStaffHeight
      );

      this.#drawBarLine(braceWidth, upperStaffFirstLineY, braceHeight);

      //upper staff
      this.#drawSingleStaffLines(braceWidth, upperStaffFirstLineY, linesWidth);
      this.#drawKey(
        symbols.violinKey.width / 2 + keyOffset,
        upperStaffFirstLineY + 2.2 * spaceBetweenStaffLines,
        symbols.violinKey,
        true
      );
      this.keySignature.draw(
        this.keySignatureOffset,
        upperStaffFirstLineY,
        true
      );
      this.metre.draw(this.metreOffset, upperStaffFirstLineY);

      // lower staff
      const lowerStaffY = y + upperStaffHeight + lowerStaffUpperMargin;
      this.#drawSingleStaffLines(braceWidth, lowerStaffY, linesWidth);
      this.#drawKey(keyOffset, lowerStaffY, symbols.bassKey, false);
      this.keySignature.draw(this.keySignatureOffset, lowerStaffY, false);
      this.metre.draw(this.metreOffset, lowerStaffY);
    }
    pop();
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
    stroke(0);
    strokeWeight(2);
    line(x, y, x, y + height);
  }

  #drawSeparationLines(x, y, width, upperStaffHeight, lowerStaffHeight) {
    stroke(150);
    strokeWeight(1);
    line(x, y, width, y); // above
    const betweenY = y + upperStaffHeight;
    line(x, betweenY, x + width, betweenY); // between
    const belowY = betweenY + lowerStaffHeight;
    line(x, belowY, x + width, belowY); // below
  }

  #drawSingleStaffLines(x, y, width) {
    stroke(0);
    strokeWeight(2);
    for (let i = 0; i < 5; i++) {
      const lineOffset = GrandStaff.getLineOffset(i);
      line(x, y + lineOffset, width, y + lineOffset);
    }
  }

  #drawKey(x, y, keySymbol, centerSymbol = false) {
    if (centerSymbol) {
      imageMode(CENTER);
    } else {
      imageMode(CORNER);
    }
    image(keySymbol, x, y);
  }
}
