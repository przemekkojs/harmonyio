class Accidental {
  static accidentals = {
    sharp: "sharp",
    doubleSharp: "doubleSharp",
    bemol: "bemol",
    doubleBemol: "doubleBemol",
    natural: "natural",
    none: "none",
  };

  static accidentalChar = {
    sharp: "#",
    doubleSharp: "x",
    bemol: "b",
    doubleBemol: "bb",
    natural: "bq",
    none: ""
  }

  constructor(name = Accidental.accidentals.none) {
    this.setName(name);
  }

  isSet() {
    return this.name !== Accidental.accidentals.none;
  }

  setName(name) {
    if (!Accidental.accidentals.hasOwnProperty(name)) {
      this.name = Accidental.accidentals.none;
    } else {
      this.name = name;
    }
  }

  getChar(){
    return Accidental.accidentalChar[this.name];
  }

  getName() {
    return this.name;
  }

  getAccidentalImage() {
    if (!this.isSet()) {
      return null;
    }

    return symbols[this.name];
  }

  getHeight() {
    const img = this.getAccidentalImage();
    if (img === null) {
      return 0;
    }

    return img.height;
  }

  getWidth() {
    const img = this.getAccidentalImage();
    if (img === null) {
      return 0;
    }

    return img.width;
  }

  draw(x, y) {
    const img = this.getAccidentalImage();
    if (img === null) {
      return;
    }

    push();
    imageMode(CENTER);
    translate(x, y);
    if (this.name === Accidental.accidentals.bemol || this.name === Accidental.accidentals.doubleBemol) {
      translate(0, bemolOffset);
    }

    image(img, 0, 0);
    pop();
  }
}
