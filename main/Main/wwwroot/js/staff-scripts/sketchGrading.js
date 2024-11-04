let canvas;
let canvasWidth = 978;
let canvasHeight = 800;
let sketchFont;

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
  sketchFont = loadFont(fontUrl);
}

function setup() {
  canvas = createCanvas(canvasWidth, canvasHeight);
  canvas.parent("#music-staff-div");

  textFont(sketchFont);
  resizeSymbols();

  grandStaff = new GrandStaff(canvasWidth);
  grandStaff.loadFromJson(questions[0], userSolutions[0][0]);
  //noLoop();
}

function draw() {
  background(240);
  if (grandStaff.isLoaded) {
    grandStaff.draw();
  }
}
