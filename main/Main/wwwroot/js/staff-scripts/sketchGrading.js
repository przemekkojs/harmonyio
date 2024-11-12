let canvas;
let sketchFont;

let grandStaff;

const drawAddNoteHint = false;

function resizeCanvasHorizontally() {
  canvasWidth = grandStaff.width;
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
  grandStaff.loadFromJson(questions[0], getCurrentSolution());
  //noLoop();
}

function draw() {
  background(255);
  if (grandStaff.isLoaded) {
    grandStaff.draw();
  }
}
