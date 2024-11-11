let canvas;
let canvasHeight = 800;
let sketchFont;

let grandStaff;
let menu;

const drawAddNoteHint = true;

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
  sketchFont = loadFont(fontUrl);
}

function setup() {
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.parent("#music-staff-div");
  textFont(sketchFont);
  resizeSymbols();

  menu = new Menu(canvasWidth - menuWidth, 0, menuWidth);
  grandStaff = new GrandStaff(canvasWidth - menuWidth);
  grandStaff.loadFromJson(
    questions[0],
    document.querySelector(`input[name="Answers[0]"]`).value
  );
}

function draw() {
  background(255);
  if (grandStaff.isLoaded) {
    grandStaff.draw();
    menu.draw();
    handleMouseInteraction(grandStaff);
  }
}
