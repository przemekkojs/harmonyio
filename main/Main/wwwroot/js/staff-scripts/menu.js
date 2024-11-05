class Button {
  constructor(
    parent,
    xOffset,
    yOffset,
    width,
    height,
    symbol,
    shortcut,
    onClick,
    checkIfIsActive
  ) {
    this.parent = parent;
    this.xOffset = xOffset;
    this.yOffset = yOffset;

    this.width = width;
    this.height = height;
    this.symbol = symbol;
    this.setDrawBehavior();
    this.shortcut = shortcut;
    this.onClick = onClick;
    this.checkIfIsActive = checkIfIsActive;

    this.isActive = false;
  }

  setDrawBehavior() {
    if (this.symbol instanceof Note) {
      this.symbolDrawBehavior = this.#noteDrawBehavior;
    } else if (this.symbol instanceof Accidental) {
      this.symbolDrawBehavior = this.#accidentalDrawBehavior;
    } else if (this.symbol instanceof p5.Image) {
      this.symbolDrawBehavior = this.#p5ImageDrawBehavior;
    } else if (this.symbol === "dot") {
      this.symbolDrawBehavior = this.#dotDrawBehavior;
    }
  }

  isMouseOver() {
    return (
      mouseX > this.parent.x + this.xOffset &&
      mouseX < this.parent.x + this.xOffset + this.width &&
      mouseY > this.parent.y + this.yOffset &&
      mouseY < this.parent.y + this.yOffset + this.height
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

  draw() {
    const x = this.parent.x + this.xOffset;
    const y = this.parent.y + this.yOffset;
    this.#drawBorder(x, y);
    this.#drawSymbol(x, y);
    this.#drawShortcut(x, y);
  }

  #drawSymbol(x, y) {
    this.symbolDrawBehavior(x, y);
  }

  #drawBorder(x, y) {
    // make border wider if active
    if (this.isActive) {
      strokeWeight(5);
    } else {
      strokeWeight(2);
    }

    // make background darker if mouseOver
    if (this.isMouseOver()) {
      fill(166, 166, 166, 50);
    } else {
      noFill();
    }

    rect(x, y, this.width, this.height, 10, 10, 10, 10);
  }

  #drawShortcut(x, y) {
    const letterSize = 13;
    if (!this.shortcutGraphic) {
      const w = (this.shortcut.length + 3) * letterSize;
      this.shortcutGraphic = createGraphics(w, letterSize * 2);
      this.shortcutGraphic.textAlign(CENTER, CENTER);
      this.shortcutGraphic.textFont(sketchFont);
      this.shortcutGraphic.textSize(letterSize);
      this.shortcutGraphic.text("(" + this.shortcut + ")", w / 2, letterSize);
    }
    imageMode(CENTER);
    image(
      this.shortcutGraphic,
      x + this.width * 0.5,
      y + this.height - letterSize
    );
  }

  #noteDrawBehavior(x, y) {
    const widthScaler = this.symbol.getBaseValue() === 1 ? 0.5 : 0.45;

    this.symbol.drawNote(x + this.width * widthScaler, y + this.height * 0.6);
  }

  #accidentalDrawBehavior(x, y) {
    const symbolName = this.symbol.getName();
    let symbolYOffset = 0;
    if (
      symbolName === Accidental.accidentals.bemol ||
      symbolName === Accidental.accidentals.doubleBemol
    ) {
      symbolYOffset = -bemolOffset;
    }

    this.symbol.draw(
      x + this.width * 0.5,
      y + this.height * 0.4 + symbolYOffset
    );
  }

  #p5ImageDrawBehavior(x, y) {
    imageMode(CORNER);
    image(this.symbol, x + 5, y + 5, this.width - 10, this.height - 25);
  }

  #dotDrawBehavior(x, y) {
    fill(0);
    circle(x + this.width * 0.5, y + this.height * 0.5, dotDiameter);
  }
}

class Menu {
  static actions = {
    none: "none",
    note: "note",
    thrash: "thrash",
  };

  constructor(x, y, width) {
    this.x = x;
    this.y = y;
    this.width = width;

    this.curAction = Menu.actions.none;
    this.note = new Note();

    this.#createButtons();
    this.updateActiveButtons();
  }

  getHeight() {
    return (
      this.buttons[this.buttons.length - 1].yOffset +
      this.buttons[this.buttons.length - 1].height
    );
  }

  #createNoteButton(xOffset, yOffset, width, height, noteValue, shortcut) {
    return new Button(
      this,
      xOffset,
      yOffset,
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

  #createAccidentalButton(
    xOffset,
    yOffset,
    width,
    height,
    accidental,
    shortcut
  ) {
    return new Button(
      this,
      xOffset,
      yOffset,
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
        this,
        20,
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
        this,
        85,
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
    const noteButtonsYOffset = 80;
    const noteButtonConfig = [
      { x: 20, y: 15, noteValue: 1, label: "1" },
      { x: 85, y: 15, noteValue: 2, label: "2" },
      { x: 20, y: 110, noteValue: 4, label: "3" },
      { x: 85, y: 110, noteValue: 8, label: "4" },
      { x: 20, y: 205, noteValue: 16, label: "5" },
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
        this,
        20,
        385,
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
        this,
        85,
        385,
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
    const accidentalButtonsYOffset = 455;
    const accidentalButtonConfig = [
      {
        x: 20,
        y: 15,
        accidental: Accidental.accidentals.sharp,
        label: "q",
      },
      {
        x: 85,
        y: 15,
        accidental: Accidental.accidentals.doubleSharp,
        label: "w",
      },
      {
        x: 20,
        y: 100,
        accidental: Accidental.accidentals.bemol,
        label: "e",
      },
      {
        x: 85,
        y: 100,
        accidental: Accidental.accidentals.doubleBemol,
        label: "r",
      },
      {
        x: 20,
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
    const y = clientRect.y;
    const topOffset = 10;

    if (y > topOffset) {
      return 0;
    } else {
      const canvasHeight = clientRect.height;

      if (y + canvasHeight < this.getHeight() + 20) {
        return canvasHeight - (this.getHeight() + 20) + topOffset;
      } else {
        return -y + topOffset;
      }
    }
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

  setX(x) {
    this.x = x;
  }

  draw() {
    const yOffset = this.calculateYOffset();
    this.y = yOffset;

    for (let i = 0; i < this.buttons.length; i++) {
      this.buttons[i].draw();
    }
  }
}

let curClicked = null;
let isMouseClickedHelper = false;

function handleMouseInteraction(grandStaff) {
  const elementsUnderMouse = grandStaff.isOver(mouseX, mouseY);
  if (menu.curAction === Menu.actions.thrash) {
    handleThrashInteraction(elementsUnderMouse);
  } else if (menu.curAction === Menu.actions.note) {
    handleNoteInteraction(elementsUnderMouse, menu.note);
  } else if (menu.curAction === Menu.actions.none) {
    handleDragNoteInteraction(elementsUnderMouse);
  }
}

function handleDragNoteInteraction(elementsUnderMouse) {
  if (!mouseIsPressed) {
    curClicked = null;
    return;
  }

  if (curClicked !== null) {
    // has already chosen some note
    const snappingPoint = curClicked.twoNotes.getClosestSnappingPoint(
      mouseX,
      mouseY
    );

    curClicked.note.setLine(snappingPoint.lineNumber);
  } else if ("note" in elementsUnderMouse) {
    curClicked = {
      note: elementsUnderMouse.note,
      twoNotes: elementsUnderMouse.twoNotes,
    };
  }
}

function handleThrashInteraction(elementsUnderMouse) {
  if (!("vertical" in elementsUnderMouse)) {
    return;
  }

  const vertical = elementsUnderMouse.vertical;

  if (vertical.canClear()) {
    vertical.drawArea();

    if (mouseIsPressed) {
      vertical.clear();
    }
  } else {
    imageMode(CENTER);
    image(symbols.thrashCanCrossed, mouseX + 20, mouseY - 20, 30, 30);
  }
}

function handleNoteInteraction(elementsUnderMouse, note) {
  if ("twoNotes" in elementsUnderMouse) {
    const twoNotes = elementsUnderMouse.twoNotes;
    if (twoNotes.canAddNote(note)) {
      const snappingPoint = twoNotes.getClosestSnappingPoint(mouseX, mouseY);
      twoNotes.drawAdditionalLinesOnAdding(
        snappingPoint.lineNumber,
        note.getNoteWidth(false)
      );
      note.draw(snappingPoint.x, snappingPoint.y);

      if (mouseIsPressed) {
        if (!isMouseClickedHelper) {
          twoNotes.addNote(note, snappingPoint.lineNumber);
        }
        isMouseClickedHelper = true;
      } else {
        isMouseClickedHelper = false;
      }
    } else {
      imageMode(CENTER);
      image(symbols.forbiddenSymbol, mouseX + 25, mouseY - 20, 20, 20)
      note.draw(mouseX, mouseY);
    }
  } else {
    note.draw(mouseX, mouseY);
  }
}

function keyPressed() {
  menu.keyPressed();
}

function mouseClicked() {
  menu.mouseClicked();
}
