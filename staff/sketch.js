const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];

let canvas;
let canvasWidth;
let canvasHeight;

let grandStaff;
let menu;
let curClicked;
let isMouseClickedHelper;

function preload() {
  preloadSymbols();
}

function setup() {
  canvasWidth = windowWidth * 0.85;
  canvasHeight = 800;
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.position((windowWidth - canvasWidth) / 2, 100);

  resizeSymbols();

  const menuWidth = 150;
  grandStaff = new GrandStaff(verticalsPerBarList, canvasWidth - menuWidth);
  menu = new Menu(canvasWidth - menuWidth, menuWidth);

  curClicked = null;
  isMouseClickedHelper = false;
}

function draw() {
  background(240);
  grandStaff.draw();
  menu.draw();

  handleMouseInteraction(grandStaff);
}

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
  vertical.drawArea();

  if (vertical.canClear()) {
    circle(mouseX, mouseY, 10);

    if (mouseIsPressed) {
      vertical.clear();
    }
  } else {
    rect(mouseX, mouseY, 10, 10);
  }
}

function handleNoteInteraction(elementsUnderMouse, note) {
  if ("twoNotes" in elementsUnderMouse) {
    const twoNotes = elementsUnderMouse.twoNotes;
    if (twoNotes.canAddNote(note)) {
      const snappingPoint = twoNotes.getClosestSnappingPoint(mouseX, mouseY);
      note.draw(snappingPoint.x, snappingPoint.y);

      if (mouseIsPressed) {
        if(!isMouseClickedHelper) {
          twoNotes.addNote(note, snappingPoint.lineNumber);
        }
        isMouseClickedHelper = true;
      } else {
        isMouseClickedHelper = false;
      }
    } else {
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
