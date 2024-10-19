const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];

let canvas;
let canvasWidth;
let canvasHeight;

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
}

function setup() {
  canvasWidth = 1240;
  canvasHeight = 800;
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.parent("#music-staff-div");

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
