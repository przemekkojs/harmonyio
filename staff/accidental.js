class Accidental {
  static accidentals = {
    sharp: "#",
    doubleSharp: "x",
    bemol: "b",
    doubleBemol: "bb",
    natural: "bq",
    none: "",
  };
  constructor(name = "none") {
    this.setName(name);
  }

  isSet() {
    return this.name !== "none";
  }

  setName(name) {
    if (!Accidental.accidentals.hasOwnProperty(name)) {
      this.name = "none";
    } else {
      this.name = name;
    }
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
    if (this.name === "bemol" || this.name === "doubleBemol") {
      translate(0, -0.5 * spaceBetweenStaffLines);
    }

    image(img, 0, 0);
    pop();
  }
}
