const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];

let canvas;
let canvasWidth;
let canvasHeight;
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
  sketchFont = loadFont("/assets/fonts/Inconsolata.otf");
}

function setup() {
  canvasWidth = windowWidth * 0.85;
  canvasHeight = 800;
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.position((windowWidth - canvasWidth) / 2, 100);
  textFont(sketchFont);
  resizeSymbols();

  menu = new Menu(canvasWidth - menuWidth, 0, menuWidth);

  grandStaff = new GrandStaff(verticalsPerBarList, canvasWidth - menuWidth);
  grandStaff.init();
}

function draw() {
  background(240);
  grandStaff.draw();
  menu.draw();

  handleMouseInteraction(grandStaff);
}
