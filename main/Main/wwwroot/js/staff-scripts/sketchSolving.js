const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];

let canvas;
let canvasWidth = 1288;
let canvasHeight = 800;
let sketchFont;

let grandStaff;
let menu;

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
    "",
    document.querySelector(`input[name="Answers[0]"]`).value
  );
}

function draw() {
  background(240);
  if (grandStaff.isLoaded) {
    grandStaff.draw();
    menu.draw();
    handleMouseInteraction(grandStaff);
  }
}
