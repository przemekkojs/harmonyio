class Button {
  constructor(x, y, width, height, symbol, shortcut, onClick, checkIfIsActive) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
    this.symbol = symbol;
    this.shortcut = shortcut;
    this.onClick = onClick;
    this.checkIfIsActive = checkIfIsActive;

    this.isActive = false;
  }

  isMouseOver() {
    return (
      mouseX > this.x &&
      mouseX < this.x + this.width &&
      mouseY > this.y &&
      mouseY < this.y + this.height
    );
  }

  isShortcutPressed(key) {
    return key === this.shortcut;
  }

  handleShortcutPressed(key) {
    if (this.isShortcutPressed(key)) {
      this.onClick();
    }
  }

  handleClick() {
    if (this.isMouseOver()) {
      this.onClick();
    }
  }

  updateIsActive() {
    this.isActive = this.checkIfIsActive();
  }

  draw(xOffset, yOffset) {
    push();
    translate(xOffset, yOffset);

    // make border wider if chosen
    if (this.isActive) {
      strokeWeight(5);
    }

    // make background darker if mouseOver
    if (this.isMouseOver()) {
      fill(166, 166, 166, 50);
    } else {
      noFill();
    }
    rect(this.x, this.y, this.width, this.height, 10, 10, 10, 10);
    pop();

    if (this.symbol instanceof Note) {
      if (this.symbol.getBaseValue() === 1) {
        this.symbol.drawNote(
          this.x + this.width * 0.5,
          this.y + this.height * 0.6
        );
      } else {
        this.symbol.drawNote(
          this.x + this.width * 0.45,
          this.y + this.height * 0.6
        );
      }
    } else if (this.symbol instanceof Accidental) {
      const symbolName = this.symbol.getName();
      let symbolYOffset = 0;
      if (
        symbolName === Accidental.accidentals.bemol ||
        symbolName === Accidental.accidentals.doubleBemol
      ) {
        symbolYOffset = -bemolOffset;
      }

      this.symbol.draw(
        this.x + this.width * 0.5,
        this.y + this.height * 0.4 + symbolYOffset
      );
    } else if (this.symbol instanceof p5.Image) {
      push();
      imageMode(CORNER);
      image(
        this.symbol,
        this.x + 5,
        this.y + 5,
        this.width - 10,
        this.height - 25
      );
      pop();
    } else if (this.symbol === "dot") {
      push();
      fill(0);
      circle(
        this.x + this.width * 0.5,
        this.y + this.height * 0.5,
        dotDiameter
      );
      pop();
    }

    push();
    textAlign(CENTER);
    textSize(13);
    text(
      "(" + this.shortcut + ")",
      this.x + this.width * 0.5,
      this.y + this.height - 7
    );
    pop();
  }
}

class Menu {
  static actions = {
    none: "none",
    note: "note",
    thrash: "thrash",
  };

  constructor(x, width) {
    this.curAction = Menu.actions.none;
    this.x = x;
    this.width = width;
    this.yOffset = this.calculateYOffset();

    this.note = new Note();

    this.#createButtons();
    this.updateActiveButtons();
  }

  #createNoteButton(x, y, width, height, noteValue, shortcut) {
    return new Button(
      x,
      y,
      width,
      height,
      new Note(noteValue),
      shortcut,
      () => {
        this.note.setBaseValue(noteValue);
        this.curAction = Menu.actions.note;
        this.updateActiveButtons();
      },
      () => {
        return (
          this.note.getBaseValue() === noteValue &&
          this.curAction === Menu.actions.note
        );
      }
    );
  }

  #createAccidentalButton(x, y, width, height, accidental, shortcut) {
    return new Button(
      x,
      y,
      width,
      height,
      new Accidental(accidental),
      shortcut,
      () => {
        if (this.note.getAccidentalName() === accidental) {
          this.note.setAccidental(Accidental.accidentals.none);
        } else {
          this.note.setAccidental(accidental);
        }
        this.curAction = Menu.actions.note;
        this.updateActiveButtons();
      },
      () => {
        return (
          this.note.getAccidentalName() === accidental &&
          this.curAction === Menu.actions.note
        );
      }
    );
  }

  #createButtons() {
    this.buttons = [];

    // MOUSE BUTTON
    this.buttons.push(
      new Button(
        this.x + 20,
        15,
        50,
        60,
        symbols.mouse,
        "esc",
        () => {
          this.curAction = Menu.actions.none;
          this.note.setAccidental(Accidental.accidentals.none);
          this.note.hasDot = false;
          this.updateActiveButtons();
        },
        () => {
          return this.curAction === Menu.actions.none;
        }
      )
    );

    // THRASH BUTTON
    this.buttons.push(
      new Button(
        this.x + 85,
        15,
        50,
        60,
        symbols.thrashCan,
        "d",
        () => {
          this.curAction = Menu.actions.thrash;
          this.note.setAccidental(Accidental.accidentals.none);
          this.note.hasDot = false;
          this.updateActiveButtons();
        },
        () => {
          return this.curAction === Menu.actions.thrash;
        }
      )
    );

    // NOTE BUTTONS
    const noteButtonsYOffset = 90;
    const noteButtonConfig = [
      { x: this.x + 20, y: 15, noteValue: 1, label: "1" },
      { x: this.x + 85, y: 15, noteValue: 2, label: "2" },
      { x: this.x + 20, y: 110, noteValue: 4, label: "3" },
      { x: this.x + 85, y: 110, noteValue: 8, label: "4" },
      { x: this.x + 20, y: 205, noteValue: 16, label: "5" },
    ];
    for (let config of noteButtonConfig) {
      this.buttons.push(
        this.#createNoteButton(
          config.x,
          config.y + noteButtonsYOffset,
          50,
          80,
          config.noteValue,
          config.label
        )
      );
    }

    // REVERSE NOTE BUTTON
    this.buttons.push(
      new Button(
        this.x + 20,
        405,
        50,
        60,
        symbols.noteReverse,
        "x",
        () => {
          this.note.toggleIsFacingUp();
          this.updateActiveButtons();
        },
        () => {
          return this.note.isFacingUp !== true;
        }
      )
    );

    // TOGGLE DOT BUTTON
    this.buttons.push(
      new Button(
        this.x + 85,
        405,
        50,
        60,
        "dot",
        ".",
        () => {
          this.curAction = Menu.actions.note;
          this.note.toggleHasDot();
          this.updateActiveButtons();
        },
        () => {
          return this.note.hasDot === true;
        }
      )
    );

    // ACCIDENTAL BUTTONS
    const accidentalButtonsYOffset = 485;
    const accidentalButtonConfig = [
      {
        x: this.x + 20,
        y: 15,
        accidental: Accidental.accidentals.sharp,
        label: "q",
      },
      {
        x: this.x + 85,
        y: 15,
        accidental: Accidental.accidentals.doubleSharp,
        label: "w",
      },
      {
        x: this.x + 20,
        y: 100,
        accidental: Accidental.accidentals.bemol,
        label: "e",
      },
      {
        x: this.x + 85,
        y: 100,
        accidental: Accidental.accidentals.doubleBemol,
        label: "r",
      },
      {
        x: this.x + 20,
        y: 185,
        accidental: Accidental.accidentals.natural,
        label: "t",
      },
    ];
    for (let config of accidentalButtonConfig) {
      this.buttons.push(
        this.#createAccidentalButton(
          config.x,
          config.y + accidentalButtonsYOffset,
          50,
          70,
          config.accidental,
          config.label
        )
      );
    }
  }

  updateActiveButtons() {
    for (let i = 0; i < this.buttons.length; i++) {
      this.buttons[i].updateIsActive();
    }
  }

  calculateYOffset() {
    const clientRect = canvas.elt.getBoundingClientRect();
    console.log(canvas.elt.getBoundingClientRect());
  }

  mouseClicked() {
    for (let i = 0; i < this.buttons.length; i++) {
      this.buttons[i].handleClick(false);
    }
  }

  keyPressed() {
    for (let i = 0; i < this.buttons.length; i++) {
      if (keyCode === ESCAPE) {
        this.buttons[i].handleShortcutPressed("esc");
      } else {
        this.buttons[i].handleShortcutPressed(key);
      }
    }
  }

  draw() {
    for (let i = 0; i < this.buttons.length; i++) {
      this.buttons[i].draw(0, 0);
    }
  }
}

// canvas.elt.getBoundingClientRect();
