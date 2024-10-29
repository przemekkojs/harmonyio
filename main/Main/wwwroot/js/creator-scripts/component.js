// Odzwierciedlenie createComponentCreator.js
export class Component {
    constructor(id) {
        this.id = id;
        this.component = document.createElement('div');

        // TODO: Uzupełnić forma
        this.suspensionPopup = document.createElement('div');
        this.confirmAddSuspension = document.createElement('input');
        this.cancelAddSuspension = document.createElement('input');

        // TODO: Uzupełnić forma
        this.alterationPopup = document.createElement('div');
        this.addAlterationFormComponents = document.createElement('div');
        this.addAlterationFormOptions = document.createElement('div');
        this.confirmAddAlteration = document.createElement('input');
        this.cancelAddAlteration = document.createElement('input');
        this.alterRoot = document.createElement('input');
        this.alterThird = document.createElement('input');
        this.alterFifth = document.createElement('input');
        this.alterUp = document.createElement('input');
        this.alterDown = document.createElement('input');

        this.addedPopup = document.createElement('div');
        this.addAddedFormComponents = document.createElement('div');
        this.addAddedFormOptions = document.createElement('div');
        this.confirmAddAdded = document.createElement('input');
        this.cancelAddAdded = document.createElement('input');
        this.addSixth = document.createElement('input');
        this.addSeventh = document.createElement('input');
        this.addNinth = document.createElement('input');
        this.addedMinor = document.createElement('input');
        this.addedNeutral = document.createElement('input');
        this.addedMajor = document.createElement('input');

        this.functionCreator = document.createElement('div');

        this.gridContainer = document.createElement('div');

        this.leftBraceContainer = document.createElement('div');
        this.minorContainer = document.createElement('div');
        this.symbolContainer = document.createElement('div');
        this.positionContainer = document.createElement('div');
        this.rootContainer = document.createElement('div');
        this.addedContainer = document.createElement('div');
        this.suspensionContainer = document.createElement('div');
        this.alterationContainer = document.createElement('div');
        this.removedContainer = document.createElement('div');
        this.rightBraceContainer = document.createElement('div');

        this.formButtons = document.createElement('div');
        this.cancelCreator = document.createElement('input');
        this.resetCreator = document.createElement('input');
        this.submitCreator = document.createElement('input');
    }

    setIds(newId) {

    }

    getElement() {
        return this.component;
    }
}