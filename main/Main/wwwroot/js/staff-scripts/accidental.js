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
    none: "",
  };

  constructor(name = Accidental.accidentals.none) {
    this.setName(name);
  }

  isSet() {
    return this.name !== Accidental.accidentals.none;
  }

  setName(name) {
    if (!Accidental.accidentals.hasOwnProperty(name) || name === Accidental.accidentals.none) {
      this.name = Accidental.accidentals.none;
      this.img = null;
      this.width = 0;
      this.height = 0;
      this.imgOffset = 0;
    } else {
      this.name = name;
      this.img = symbols[name];
      this.width = this.img.width;
      this.height = this.img.height;
      
      this.imgOffset = this.name === Accidental.accidentals.bemol || this.name === Accidental.accidentals.doubleBemol ? bemolOffset : 0;
    } 
  }

  getChar() {
    return Accidental.accidentalChar[this.name];
  }

  static charToAccidentalName(char) {
    if (char === Accidental.accidentalChar.sharp) {
      return Accidental.accidentals.sharp;
    } else if (char === Accidental.accidentalChar.doubleSharp) {
      return Accidental.accidentals.doubleSharp;
    } else if (char === Accidental.accidentalChar.bemol) {
      return Accidental.accidentals.bemol;
    } else if (char === Accidental.accidentalChar.doubleBemol) {
      return Accidental.accidentals.doubleBemol;
    } else if (char === Accidental.accidentalChar.natural) {
      return Accidental.accidentals.natural;
    } else {
      return Accidental.accidentals.none;
    }
  }

  getName() {
    return this.name;
  }

  getAccidentalImage() {
    return this.img;
  }

  getHeight() {
    return this.height
  }

  getWidth() {
    return this.width;
  }

  draw(x, y) {
    if (this.img === null) {
      return;
    }

    imageMode(CENTER);
    image(this.img, x, y + this.imgOffset);
  }
}
