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
  }

  draw(x, y) {
    push();
    strokeWeight(0);

    // draw function letter
    const letterSize = getVerticalWidth() * 0.5;
    textAlign(CENTER, BASELINE);
    textSize(letterSize);

    // this calculates bounds only once since it may be costly
    if (!this.functionLetterW) {
      let functionLetterBbox = sketchFont.textBounds(this.functionLetter, x, y);
      this.functionLetterW = functionLetterBbox.w;
      this.functionLetterH = functionLetterBbox.h;
    }
    text(this.functionLetter, x, y + this.functionLetterH / 2);

    // draw roman letter
    textSize(letterSize * 0.3);
    textAlign(LEFT, BASELINE);
    const romanLetterX =
      this.functionLetter === "T" ? x + 3 : x + this.functionLetterW / 2 + 2;
    text(
      this.functionLetterRomanNumber,
      romanLetterX,
      y + this.functionLetterH / 2
    );
    const rommanLetterWidth = textWidth(this.functionLetterRomanNumber);
    const rommanLetterEndX =
      rommanLetterWidth === 0
        ? x + this.functionLetterW / 2
        : romanLetterX + rommanLetterWidth;

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
    const numberAboveBelowSize = letterSize * 0.4;
    textAlign(CENTER, BOTTOM);
    textSize(numberAboveBelowSize);
    text(this.numberAbove, x, y - this.functionLetterH / 2);
    textAlign(CENTER, TOP);
    text(this.numberBelow, x, y + this.functionLetterH / 2);

    // draw top right numbers
    textAlign(LEFT, BASELINE);
    const rightNumbersSize = letterSize * 0.3;
    textSize(rightNumbersSize);
    let maxTopRightWidth = 0;
    let numY = y - this.functionLetterH / 4;
    for (let i = this.numbersTopRight.length - 1; i >= 0; i--) {
      text(this.numbersTopRight[i], rommanLetterEndX, numY);
      maxTopRightWidth = Math.max(
        textWidth(this.numbersTopRight[i]),
        maxTopRightWidth
      );
      numY -= rightNumbersSize * 0.66 + 1;
    }

    // draw bottom right numbers
    let maxBottomRightWidth = 0;
    textAlign(LEFT, TOP);
    numY = y + this.functionLetterH / 4;
    for (let i = this.numbersBottomRight.length - 1; i >= 0; i--) {
      text(this.numbersBottomRight[i], rommanLetterEndX, numY);
      maxBottomRightWidth = Math.max(
        textWidth(this.numbersBottomRight[i]),
        maxBottomRightWidth
      );
      numY += rightNumbersSize * 0.66 + 1;
    }

    const centerRightX =
      rommanLetterEndX + Math.max(maxTopRightWidth, maxBottomRightWidth) + 2;

    // draw center right numbers
    textAlign(LEFT, CENTER);
    const centerY = y;
    const centerSpacing = rightNumbersSize * 0.66 + 1;

    if (this.numbersCenterRight.length > 0) {
      // Calculate the starting Y position based on even or odd count
      let startY =
        centerY - ((this.numbersCenterRight.length - 1) * centerSpacing) / 2;

      for (let i = 0; i < this.numbersCenterRight.length; i++) {
        text(
          this.numbersCenterRight[i],
          centerRightX,
          startY + i * centerSpacing
        );
      }
    }
    pop();
  }
}
