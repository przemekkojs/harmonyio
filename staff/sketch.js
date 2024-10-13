const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];

function preload() {
  preloadSymbols();
}

let canvas;
let canvasWidth;
let canvasHeight;

let grandStaff;
let menu;

function setup() {
  canvasWidth = windowWidth * 0.85;
  canvasHeight = 800;
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.position((windowWidth - canvasWidth) / 2, 100);

  resizeSymbols();

  const menuWidth = 150;
  grandStaff = new GrandStaff(verticalsPerBarList, canvasWidth - menuWidth);
  menu = new Menu(canvasWidth - menuWidth, menuWidth);
}

function draw() {
  background(240);
  grandStaff.drawStaticElements();
  menu.draw();

  elementsUnderMouse = grandStaff.isOver(mouseX, mouseY);
  handleMouseInteraction(elementsUnderMouse);

  grandStaff.drawDynamicElements();
}

function mouseClicked() {
  menu.mouseClicked();
}

function handleMouseInteraction(elementsUnderMouse) {
  if (
    menu.curAction === Menu.actions.thrash &&
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
  } else if (menu.curAction === Menu.actions.note) {
    if ("twoNotes" in elementsUnderMouse) {
      const twoNotes = elementsUnderMouse.twoNotes;
      if (twoNotes.canAddNote(menu.note)) {
        const snappingPoint = twoNotes.getClosestSnappingPoint(mouseX, mouseY);
        menu.note.draw(snappingPoint.x, snappingPoint.y);

        if (mouseIsPressed) {
          twoNotes.addNote(menu.note, snappingPoint.lineNumber);
          elementsUnderMouse.bar.updateVerticalPositions();
        }
      } else {
        menu.note.draw(mouseX, mouseY);
      }
    } else {
      menu.note.draw(mouseX, mouseY);
    }
  }
}

function keyPressed() {
  menu.keyPressed();
}
