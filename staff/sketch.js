const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];

function preload() {
  preloadSymbols();
}

let canvas;
let canvasWidth;
let canvasHeight;

let grandStaff;
let mouseChoice;
let over;

function setup() {
  canvasWidth = windowWidth * 0.85;
  canvasHeight = 800;
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.position((windowWidth - canvasWidth) / 2, 100);

  resizeSymbols();

  const grandStaffWidth = canvasWidth;
  grandStaff = new GrandStaff(verticalsPerBarList, grandStaffWidth);
  mouseChoice = new MouseChoice();
}

function draw() {
  background(240);
  grandStaff.drawStaticElements();

  elementsUnderMouse = grandStaff.isOver(mouseX, mouseY);
  handleMouseInteraction(elementsUnderMouse);

  grandStaff.drawDynamicElements();
}

function handleMouseInteraction(elementsUnderMouse) {
  if (
    mouseChoice.getMouseChoice() === "thrash" &&
    "vertical" in elementsUnderMouse
  ) {
    const vertical = elementsUnderMouse.vertical;
    vertical.drawArea();

    if (vertical.canClear()) {
      circle(mouseX, mouseY, 10);
      if (mouseIsPressed) {
        vertical.clear();
        elementsUnderMouse.bar.updateVerticalPositions();
      }
    } else {
      rect(mouseX, mouseY, 10, 10);
    }
  } else if (mouseChoice.getMouseChoice() === "note") {
    if ("twoNotes" in elementsUnderMouse) {
      const twoNotes = elementsUnderMouse.twoNotes;
      if (twoNotes.canAddNote(mouseChoice.note)) {
        const snappingPoint = twoNotes.getClosestSnappingPoint(mouseX, mouseY);
        mouseChoice.note.drawSimple(snappingPoint.x, snappingPoint.y);

        if (mouseIsPressed) {
          twoNotes.addNote(mouseChoice.note, snappingPoint.lineNumber);
          elementsUnderMouse.bar.updateVerticalPositions();
        }
      } else {
        mouseChoice.note.drawSimple(mouseX, mouseY);
      }
    } else {
      mouseChoice.note.drawSimple(mouseX, mouseY);
    }
  }
}

class MouseChoice {
  static choices = ["mouse", "thrash", "note"];

  constructor() {
    this.resetChoice();
    this.note = new Note(1);
  }

  resetChoice() {
    this.currentChoice = "mouse";
    this.choiceSymbol = null;
  }

  setNote(noteValue) {
    this.currentChoice = "note";
    this.note.setBaseValue(noteValue);
  }
  flipNote() {
    this.note.toggleIsFacingUp();
  }
  addDot() {
    this.note.toggleHasDot();
  }
  setAccidental(accidentalName) {
    this.note.setAccidental(accidentalName);
  }

  setTrash() {
    this.currentChoice = "thrash";
  }

  getMouseChoice() {
    return this.currentChoice;
  }
}

function keyPressed() {
  if (keyCode === ESCAPE) {
    mouseChoice.resetChoice();
  }
  if (key >= "1" && key <= "5") {
    let keyInt = parseInt(key);
    mouseChoice.setNote(2 ** (keyInt - 1));
  }
  if (key === "q") {
    mouseChoice.setAccidental("none");
  }
  if (key === "w") {
    mouseChoice.setAccidental("sharp");
  }
  if (key === "e") {
    mouseChoice.setAccidental("doubleSharp");
  }
  if (key === "r") {
    mouseChoice.setAccidental("bemol");
  }
  if (key === "t") {
    mouseChoice.setAccidental("doubleBemol");
  }
  if (key === "y") {
    mouseChoice.setAccidental("natural");
  }
  if (key === "a") {
    mouseChoice.flipNote();
  }
  if (key === "s") {
    mouseChoice.addDot();
  }
  if (key === "d") {
    mouseChoice.setTrash();
  }
}
