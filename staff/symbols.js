let symbols = {};
const symbolPaths = {
    curlyBrace: "/assets/symbols/curly-brace.png",
    bassKey: "/assets/symbols/bass-key.png",
    violinKey: "/assets/symbols/violin-key.png",
    bemol: "/assets/symbols/bemol.png",
    doubleBemol: "/assets/symbols/double-bemol.png",
    sharp: "/assets/symbols/sharp.png",
    doubleSharp: "/assets/symbols/double-sharp.png",
    natural: "/assets/symbols/natural.png",
    fullNote: "/assets/symbols/full-note.png",
    noteHeadClosed: "/assets/symbols/note-head-closed.png",
    noteHeadOpened: "/assets/symbols/note-head-opened.png",
    noteFlag: "/assets/symbols/note-flag.png",
};

function preloadSymbols() {
    for (let key in symbolPaths) {
        symbols[key] = loadImage(symbolPaths[key]);
    }
}

function resizeSymbols() {
    symbols.bassKey.resize(
        2.3 * spaceBetweenStaffLines,
        3 * spaceBetweenStaffLines
    );
    symbols.violinKey.resize(symbols.bassKey.width, 0);

    symbols.bemol.resize(0, spaceBetweenStaffLines * 2.3);
    symbols.doubleBemol.resize(symbols.bemol.width, spaceBetweenStaffLines * 2.5);
    symbols.sharp.resize(symbols.bemol.width, spaceBetweenStaffLines * 2.5);
    symbols.doubleSharp.resize(symbols.bemol.width, 0);
    symbols.natural.resize(symbols.bemol.width, spaceBetweenStaffLines * 2.7);

    symbols.fullNote.resize(0, spaceBetweenStaffLines);
    symbols.noteHeadOpened.resize(0, spaceBetweenStaffLines);
    symbols.noteHeadClosed.resize(0, spaceBetweenStaffLines);
    symbols.noteFlag.resize(0, stemHeight * 0.66);
}