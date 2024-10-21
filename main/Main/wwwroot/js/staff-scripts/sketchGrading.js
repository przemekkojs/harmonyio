const verticalsPerBarList = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];


let canvas;
let canvasWidth;
let canvasHeight;

let grandStaff;

function resizeCanvasHorizontally() {
  canvasWidth = grandStaff.width + 5;
  resizeCanvas(canvasWidth, canvasHeight);
}

function resizeCanvasVertically() {
  canvasHeight = grandStaff.numberOfStaffs * doubleGrandStaffHeight + 50;
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

  grandStaff = new GrandStaff(verticalsPerBarList, canvasWidth - 5);
  grandStaff.init();
  noLoop();
}

function draw() {
  background(240);
  grandStaff.draw();
}


