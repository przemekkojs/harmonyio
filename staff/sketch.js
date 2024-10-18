const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];

const menuWidth = 150;

let canvas;
let canvasWidth;
let canvasHeight;

let grandStaff;
let menu;
let curClicked;
let isMouseClickedHelper;

function resizeCanvasHorizontally() {
  if (menu) {
    canvasWidth = grandStaff.width + menu.width;
    const menuX = canvasWidth - menuWidth;
    menu.setX(menuX);
  } else {
    canvasWidth = grandStaff.width + 5;
  }
  resizeCanvas(canvasWidth, canvasHeight);
}

function resizeCanvasVertically() {
  if (menu && grandStaff.numberOfStaffs === 1) {
    canvasHeight = menu.getHeight() + 10;
  } else {
    canvasHeight = grandStaff.numberOfStaffs * doubleGrandStaffHeight + 50;
  }
  resizeCanvas(canvasWidth, canvasHeight);
}

function preload() {
  preloadSymbols();
}

function setup() {
  canvasWidth = windowWidth * 0.85;
  canvasHeight = 800;
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.position((windowWidth - canvasWidth) / 2, 100);

  resizeSymbols();

  menu = new Menu(canvasWidth - menuWidth, 0, menuWidth);

  grandStaff = new GrandStaff(verticalsPerBarList, canvasWidth - menuWidth);
  grandStaff.init();

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
  
  if (vertical.canClear()) {
    vertical.drawArea();

    if (mouseIsPressed) {
      vertical.clear();
    }
  } else {
    push();
    imageMode(CENTER);
    image(symbols.thrashCanCrossed, mouseX + 20, mouseY - 20, 30, 30);
    pop();
  }
}

function handleNoteInteraction(elementsUnderMouse, note) {
  if ("twoNotes" in elementsUnderMouse) {
    const twoNotes = elementsUnderMouse.twoNotes;
    if (twoNotes.canAddNote(note)) {
      const snappingPoint = twoNotes.getClosestSnappingPoint(mouseX, mouseY);
      twoNotes.drawAdditionalLinesOnAdding(snappingPoint.lineNumber, note.getNoteWidth(false));
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
