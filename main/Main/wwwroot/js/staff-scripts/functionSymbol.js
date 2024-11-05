class FunctionSymbol {
  constructor(
    parent,
    functionLetter,
    hasCircle,
    numberAbove,
    numberBelow,
    numbersTopRight,
    numbersBottomRight,
    numbersCenterRight
  ) {
    this.parent = parent;

    if (functionLetter.length > 1) {
      this.functionLetter = functionLetter[0];
      this.functionLetterRomanNumber = functionLetter.slice(1).toUpperCase();
    } else {
      this.functionLetter = functionLetter;
      this.functionLetterRomanNumber = "";
    }

    this.hasCircle = hasCircle;
    this.numberAbove = numberAbove;
    this.numberBelow = numberBelow;

    this.numbersTopRight = numbersTopRight;
    this.numbersBottomRight = numbersBottomRight;
    this.numbersCenterRight = numbersCenterRight;

    this.isRendered = false;
  }

  static fromJson(parent, task) {
    return new FunctionSymbol(
      parent,
      task.symbol,
      task.minor,
      task.position,
      task.root,
      task.added.reverse(),
      [task.removed],
      task.alterations
    );
  }

  clearGraphics() {
    if (this.centerGraphic) {
      this.centerGraphic.remove();
    }
    if (this.numbersRightGraphic) {
      this.numbersRightGraphic.remove();
    }
  }

  #renderCenterGraphic(letterSize) {
    const romanLetterSize = letterSize * 0.3;
    const numberAboveBelowSize = letterSize * 0.4;

    textSize(letterSize);
    const functionLetterBbox = sketchFont.textBounds(this.functionLetter, 0, 0);

    textSize(romanLetterSize);
    const romanLetterWidth = textWidth(this.functionLetterRomanNumber);

    const romanLetterXOffset =
      this.functionLetter === "T" ? -functionLetterBbox.w / 2 + 3 : 3;

    const graphicW =
      this.functionLetterRomanNumber === ""
        ? functionLetterBbox.w + 2
        : functionLetterBbox.w + romanLetterXOffset + romanLetterWidth + 2;

    const graphicH = functionLetterBbox.h + 2 * numberAboveBelowSize;

    let graphic = createGraphics(graphicW, graphicH);
    // function letter
    graphic.textAlign(CENTER, BASELINE);
    graphic.textFont(sketchFont);
    graphic.textSize(letterSize);
    graphic.text(
      this.functionLetter,
      functionLetterBbox.w / 2,
      graphicH / 2 + functionLetterBbox.h / 2
    );

    // numbers above and below
    graphic.textSize(numberAboveBelowSize);
    graphic.text(
      this.numberAbove,
      functionLetterBbox.w / 2,
      graphicH / 2 - functionLetterBbox.h / 2 - 3
    );
    graphic.text(this.numberBelow, functionLetterBbox.w / 2, graphicH - 2);

    // roman letter
    graphic.textAlign(LEFT, BASELINE);
    graphic.textSize(romanLetterSize);
    graphic.text(
      this.functionLetterRomanNumber,
      functionLetterBbox.w + romanLetterXOffset,
      graphicH / 2 + functionLetterBbox.h / 2
    );

    this.centerGraphic = graphic;
    this.functionLetterH = functionLetterBbox.h;
    this.centerGraphicYOffset = graphicH / 2;
    this.centerGraphicXOffset = functionLetterBbox.w / 2;
    this.centerGraphicW = graphicW;
  }

  #renderNumbersRightGraphic(letterSize) {
    textSize(letterSize);

    const yDiff = letterSize * 0.66 + 1;
    const graphicH = 8 * yDiff + this.functionLetterH / 3 + 3;
    const graphicW = textWidth("7>") * 2 + 2;

    let numbersRightGraphic = createGraphics(graphicW, graphicH);
    numbersRightGraphic.textFont(sketchFont);
    numbersRightGraphic.textSize(letterSize);
    numbersRightGraphic.textAlign(LEFT, BASELINE);

    let maxWidth = 0;

    const topRightOffset =
      this.numbersTopRight.length === 4
        ? this.functionLetterH / 8
        : this.functionLetterH / 2;
    let currentY = graphicH / 2 - topRightOffset;
    for (let i = this.numbersTopRight.length - 1; i >= 0; i--) {
      numbersRightGraphic.text(this.numbersTopRight[i], 0, currentY);
      maxWidth = Math.max(textWidth(this.numbersTopRight[i]), maxWidth);
      currentY -= yDiff;
    }

    currentY = graphicH / 2 + this.functionLetterH / 2 + yDiff - 1;
    for (let i = 0; i < this.numbersBottomRight.length; i++) {
      numbersRightGraphic.text(this.numbersBottomRight[i], 0, currentY);
      maxWidth = Math.max(textWidth(this.numbersBottomRight[i]), maxWidth);
      currentY += yDiff;
    }

    currentY =
      graphicH / 2 - (this.numbersCenterRight.length / 2) * yDiff + yDiff;
    for (let i = 0; i < this.numbersCenterRight.length; i++) {
      numbersRightGraphic.text(this.numbersCenterRight[i], maxWidth, currentY);
      currentY += yDiff;
    }

    this.numbersRightGraphic = numbersRightGraphic;
    this.numbersRightYOffset = graphicH / 2;
  }

  #renderGraphics(letterSize) {
    this.#renderCenterGraphic(letterSize);
    this.#renderNumbersRightGraphic(letterSize * 0.3);
    this.isRendered = true;
  }

  draw(x, y) {
    const letterSize = getVerticalWidth() * 0.7;

    if (!this.isRendered) {
      this.#renderGraphics(letterSize);
    }

    imageMode(CORNER);

    y = y + taskHeight * 0.05;
    // function symbol, numbers above and below, roman number
    image(
      this.centerGraphic,
      x - this.centerGraphicXOffset,
      y - this.centerGraphicYOffset
    );

    const centerGraphicsEndX =
      x - this.centerGraphicXOffset + this.centerGraphicW;
    // numbers in top/bottom/center right
    image(
      this.numbersRightGraphic,
      centerGraphicsEndX,
      y - this.numbersRightYOffset
    );

    // draw circle if present
    if (this.hasCircle) {
      const circleD = letterSize * 0.2;
      strokeWeight(2);
      noFill();
      circle(
        x - this.centerGraphicXOffset - circleD / 2 - 3,
        y - this.functionLetterH / 2,
        circleD
      );
    }
  }
}
