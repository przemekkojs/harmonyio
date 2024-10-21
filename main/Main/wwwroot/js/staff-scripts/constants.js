const spaceBetweenStaffLines = 16; // everything is dependent on this, it scales the canvas (minimum=12, try svg graphics or using font for images later)
const slotWidth = spaceBetweenStaffLines * 3;
const barMargin = spaceBetweenStaffLines;
const stemHeight = spaceBetweenStaffLines * 2.5;
const upperStaffUpperMargin = 2 * spaceBetweenStaffLines + stemHeight + 10;
const upperStaffLowerMargin = 2.5 * spaceBetweenStaffLines + stemHeight + 10;
const lowerStaffUpperMargin = 3 * spaceBetweenStaffLines + stemHeight + 10;
const lowerStaffLowerMargin = 2 * spaceBetweenStaffLines + stemHeight + 10;
const upperStaffHeight =
  upperStaffUpperMargin + 4 * spaceBetweenStaffLines + upperStaffLowerMargin;
const lowerStaffHeight =
  lowerStaffUpperMargin + 4 * spaceBetweenStaffLines + lowerStaffLowerMargin;
const braceWidth = spaceBetweenStaffLines;
const braceHeight =
  4 * 2 * spaceBetweenStaffLines +
  upperStaffLowerMargin +
  lowerStaffUpperMargin;
const taskHeight = spaceBetweenStaffLines * 2.5;
const doubleGrandStaffHeight =
  upperStaffUpperMargin +
  upperStaffLowerMargin +
  lowerStaffLowerMargin +
  lowerStaffUpperMargin +
  2 * 4 * spaceBetweenStaffLines +
  taskHeight; //margins + both lines + task height
const dotDiameter = 0.5 * spaceBetweenStaffLines;
const keyOffset = braceWidth + 0.2 * spaceBetweenStaffLines;
const bemolOffset = -0.5 * spaceBetweenStaffLines;
const spaceBetweenNoteAndAccidental = 5;
const accidentalsHeight = spaceBetweenStaffLines * 2.5;
const menuWidth = 150;

function getVerticalWidth() {
  return (
    2 * symbols.fullNote.width +
    spaceBetweenNoteAndAccidental +
    2 * symbols.bemol.width
  );
}
