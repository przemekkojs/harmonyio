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

  #renderTextGraphics(text, fontSize) {
    textSize(fontSize);
    let bbox = sketchFont.textBounds(text, 0, 0);
    let graphic = createGraphics(bbox.w, bbox.h + 2);

    graphic.textAlign(CENTER, BASELINE);
    graphic.textFont(sketchFont);
    graphic.textSize(fontSize);
    graphic.text(text, bbox.w / 2, bbox.h);

    return { graphic, bbox };
  }

  #renderFunctionLetterGraphics(letterSize) {
    const { graphic, bbox } = this.#renderTextGraphics(
      this.functionLetter,
      letterSize
    );

    this.functionLetterW = bbox.w;
    this.functionLetterH = bbox.h;
    this.functionLetterGraphic = graphic;
  }

  #renderRomanLetterGraphics(letterSize) {
    if (this.functionLetterRomanNumber === "") {
      this.romanLetterWidth = 0;
      return;
    }

    const { graphic, bbox } = this.#renderTextGraphics(
      this.functionLetterRomanNumber,
      letterSize
    );

    this.romanLetterWidth = bbox.w;
    this.romanLetterHeight = bbox.h;
    this.romanLetterGraphic = graphic;
  }

  #renderNumberAboveAndBelowGraphics(letterSize) {
    this.numberAboveGraphic = this.#renderTextGraphics(
      this.numberAbove,
      letterSize
    ).graphic;

    this.numberBelowGraphic = this.#renderTextGraphics(
      this.numberBelow,
      letterSize
    ).graphic;
  }

  #renderNumbersArrayGraphics(numbersArray, letterSize) {
    let maxWidth = 0;
    textSize(letterSize);
    // Calculate total height for the numbers graphics
    let totalHeight = numbersArray.length * (letterSize * 0.66 + 1) + 3;

    // Create a graphics buffer for the numbers
    let numbersGraphic = createGraphics(textWidth("7>"), totalHeight);
    numbersGraphic.textAlign(LEFT, BASELINE);
    numbersGraphic.textFont(sketchFont);
    numbersGraphic.textSize(letterSize);

    let currentY = totalHeight - 3;

    for (let i = numbersArray.length - 1; i >= 0; i--) {
      numbersGraphic.text(numbersArray[i], 0, currentY);
      maxWidth = Math.max(textWidth(numbersArray[i]), maxWidth);
      currentY -= letterSize * 0.66 + 1;
    }

    return { numbersGraphic, maxWidth, totalHeight };
  }

  #renderNumbersRightGraphics(letterSize) {
    // Render top right numbers
    const {
      numbersGraphic: topRightNumbersGraphic,
      maxWidth: topRightMaxWidth,
      totalHeight: topRightHeight,
    } = this.#renderNumbersArrayGraphics(this.numbersTopRight, letterSize);
    this.topRightNumbersGraphic = topRightNumbersGraphic;
    this.maxTopRightWidth = topRightMaxWidth;
    this.topRightHeight = topRightHeight;

    // Render bottom right numbers
    const {
      numbersGraphic: bottomRightNumbersGraphic,
      maxWidth: bottomRightMaxWidth,
      totalHeight: bottomRightHeight,
    } = this.#renderNumbersArrayGraphics(this.numbersBottomRight, letterSize);
    this.bottomRightNumbersGraphic = bottomRightNumbersGraphic;
    this.maxBottomRightWidth = bottomRightMaxWidth;
    this.bottomRightHeight = bottomRightHeight;

    // Render center right numbers
    const {
      numbersGraphic: rightNumbersGraphic,
      maxWidth: rightMaxWidth,
      totalHeight: rigthHeight,
    } = this.#renderNumbersArrayGraphics(this.numbersCenterRight, letterSize);
    this.rightNumbersGraphic = rightNumbersGraphic;
    this.rightMaxWidth = rightMaxWidth;
    this.rigthHeight = rigthHeight;
  }

  #renderGraphics(letterSize) {
    this.#renderFunctionLetterGraphics(letterSize);
    this.#renderRomanLetterGraphics(letterSize * 0.3);
    this.#renderNumberAboveAndBelowGraphics(letterSize * 0.4);
    this.#renderNumbersRightGraphics(letterSize * 0.3);

    this.isRendered = true;
  }

  draw(x, y) {
    push();
    const letterSize = getVerticalWidth() * 0.5;

    if (!this.isRendered) {
      this.#renderGraphics(letterSize);
    }

    // draw symbol
    imageMode(CENTER);
    image(this.functionLetterGraphic, x, y);

    const romanLetterX =
      this.functionLetter === "T" ? x + 3 : x + this.functionLetterW / 2 + 2;

    // draw roman letter
    if (this.functionLetterRomanNumber !== "") {
      imageMode(CORNER);
      image(
        this.romanLetterGraphic,
        romanLetterX,
        y + this.functionLetterH / 2 - this.romanLetterHeight
      );
    }

    const rommanLetterEndX =
      this.romanLetterWidth === 0
        ? x + this.functionLetterW / 2
        : romanLetterX + this.romanLetterWidth;

    // draw circle if present
    if (this.hasCircle) {
      const circleD = letterSize * 0.2;
      strokeWeight(2);
      noFill();
      circle(
        x - this.functionLetterW / 2 - circleD / 2 - 3,
        y - this.functionLetterH / 2,
        circleD
      );
    }

    fill(0);
    strokeWeight(0);

    // draw number above and below
    imageMode(CENTER);
    image(this.numberAboveGraphic, x, y - this.functionLetterH * 0.9);
    image(this.numberBelowGraphic, x, y + this.functionLetterH * 0.9);

    const cornerNumbersX = rommanLetterEndX + 2;
    // draw top right numbers
    imageMode(CORNER);
    image(
      this.topRightNumbersGraphic,
      cornerNumbersX,
      y - this.functionLetterH / 6 - this.topRightHeight
    );
    // draw bottom right numbers
    image(
      this.bottomRightNumbersGraphic,
      cornerNumbersX,
      y + this.functionLetterH / 6
    );

    const centerRightX =
      cornerNumbersX +
      Math.max(this.maxTopRightWidth, this.maxBottomRightWidth) +
      2;
    const centerRightY = y - this.rigthHeight / 2;

    // draw center right numbers
    image(this.rightNumbersGraphic, centerRightX, centerRightY);
    pop();
  }
}
