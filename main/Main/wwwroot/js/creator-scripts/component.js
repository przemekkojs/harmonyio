// Odzwierciedlenie createComponentCreator.js
export class Component {
    constructor(id) {
        this.id = id;
        this.component = document.createElement('div');

        // Popup opóźnienia
        this.suspensionPopup = document.createElement('div');

        // Popup opóźnienia - przycisk potwierdź
        this.confirmAddSuspension = document.createElement('input');
        this.confirmAddSuspension.type = 'button';
        this.confirmAddSuspension.name = 'confirm-button';
        this.confirmAddSuspension.id = `confirm-add-suspension-${this.id}`; // ID SET
        this.confirmAddSuspension.className = `confirm-button`;
        this.confirmAddSuspension.value = "Dodaj opóźnienie";

        // Popup opóźnienia - przycisk anuluj
        this.cancelAddSuspension = document.createElement('input');
        this.cancelAddSuspension.type = 'button';
        this.cancelAddSuspension.name = 'cancel-button';
        this.cancelAddSuspension.id = `cancel-add-suspension-${this.id}`; // ID SET
        this.cancelAddSuspension.className = `cancel-button`;
        this.cancelAddSuspension.value = "Anuluj opóźnienie";

        //Dodanie do popupu opóźnienia
        [this.confirmAddSuspension, this.cancelAddSuspension].forEach(e => {
            this.suspensionPopup.appendChild(e);
        });

        // Popup alteracji
        this.alterationPopup = document.createElement('div');

        // Popup alteracji - składniki
        this.addAlterationFormComponents = document.createElement('div');
        this.alterRoot = document.createElement('input');
        this.alterThird = document.createElement('input');
        this.alterFifth = document.createElement('input');

        // Popup alteracji - opcje składników
        this.addAlterationFormOptions = document.createElement('div');
        
        this.alterUp = document.createElement('input');
        this.alterDown = document.createElement('input');

        // Popup alteracji - przycisk potwierdź
        this.confirmAddAlteration = document.createElement('input');
        this.confirmAddAlteration.type = 'button';
        this.confirmAddAlteration.name = 'confirm-button';
        this.confirmAddAlteration.id = `confirm-add-alteration-${this.id}`; // ID SET
        this.confirmAddAlteration.className = `confirm-button`;
        this.confirmAddAlteration.value = "Dodaj alterację";

        // Popup alteracji - przycisk anuluj
        this.cancelAddAlteration = document.createElement('input');
        this.cancelAddAlteration.type = 'button';
        this.cancelAddAlteration.name = 'confirm-button';
        this.cancelAddAlteration.id = `cancel-add-alteration-${this.id}`; // ID SET
        this.cancelAddAlteration.className = `cancel-button`;
        this.cancelAddAlteration.value = "Anuluj alterację";

        // Popup składników dodanych
        this.addedPopup = document.createElement('div');

        // Popup składników dodanych - składniki
        this.addAddedFormComponents = document.createElement('div');
        this.addSixth = document.createElement('input');
        this.addSeventh = document.createElement('input');
        this.addNinth = document.createElement('input');

        // Popup składników dodanych - opcje
        this.addAddedFormOptions = document.createElement('div');
        this.addedMinor = document.createElement('input');
        this.addedNeutral = document.createElement('input');
        this.addedMajor = document.createElement('input');

        // Popup składników dodanych - przycisk potwierdź
        this.confirmAddAdded = document.createElement('input');

        // Popup składników dodanych - przycisk anuluj
        this.cancelAddAdded = document.createElement('input');
        
        // Kreator funkcji
        this.functionCreator = document.createElement('div');

        // Kreator funkcji - siatka
        this.gridContainer = document.createElement('div');

        // Kreator funkcji - siatka - lewy nawias
        this.leftBraceContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.minorContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.symbolContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.positionContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.rootContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.addedContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.suspensionContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.alterationContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.removedContainer = document.createElement('div');

        // Kreator funkcji - siatka - 
        this.rightBraceContainer = document.createElement('div');

        // Formularz zatwierdzenia
        this.formButtons = document.createElement('div');

        // Formularz zatwierdzenia - przycisk anuluj
        this.cancelCreator = document.createElement('input');

        // Formularz zatwierdzenia - przycisk resetuj
        this.resetCreator = document.createElement('input');

        // Formularz zatwierdzenia - przycisk potwierdź
        this.submitCreator = document.createElement('input');

        // Ustawienie ID'ków wszystkiego co trzeba
        this.setIds(this.id);
    }

    // TODO: Wszystko z "ID SET" powinno wywędrować tutaj
    setIds(newId) {
        this.id = newId;
    }

    getElement() {
        return this.component;
    }
}