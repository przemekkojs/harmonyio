// Odzwierciedlenie createComponentCreator.js
export class Component {
    constructor(id) {
        this.id = id;
        this.component = document.createElement('div');        
        this.component.className = "task-box";

        // Popup opóźnienia
        this.suspensionPopup = document.createElement('div');
        this.suspensionPopup.className = "suspension-popup popup-base";

        // Popup opóźnienia - tytuł
        const suspensionPopupTitle = document.createElement('span');
        suspensionPopupTitle.className = "popup-title";
        suspensionPopupTitle.innerHTML = "Dodaj opóźnienie";

        // Popup opóźnienia - przycisk potwierdź
        this.confirmAddSuspension = document.createElement('input');
        this.confirmAddSuspension.type = 'button';
        this.confirmAddSuspension.name = 'confirm-button';        
        this.confirmAddSuspension.className = `confirm-button`;
        this.confirmAddSuspension.value = "Dodaj opóźnienie";

        // Popup opóźnienia - przycisk anuluj
        this.cancelAddSuspension = document.createElement('input');
        this.cancelAddSuspension.type = 'button';
        this.cancelAddSuspension.name = 'cancel-button';        
        this.cancelAddSuspension.className = `cancel-button`;
        this.cancelAddSuspension.value = "Anuluj opóźnienie";

        // Dodanie do popupu opóźnienia
        [suspensionPopupTitle, this.confirmAddSuspension, this.cancelAddSuspension].forEach(e => {
            this.suspensionPopup.appendChild(e);
        });

        // Popup alteracji
        this.alterationPopup = document.createElement('div');
        this.alterationPopup.className = "alteration-popup popup-base";        

        // Popup alteracji - tytuł
        const alterationPopupTitle = document.createElement('span');
        alterationPopupTitle.className = "popup-title";
        alterationPopupTitle.innerText = "Dodaj alterację";

        // Popup alteracji - składniki
        this.addAlterationFormComponents = document.createElement('div');
        this.addAlterationFormComponents.className = "component-choice";
                
        this.alterRoot = document.createElement('input');
        this.alterRoot.type = "radio";
        this.alterRoot.name = "component";
        this.alterRoot.value = "root";        
        
        this.alterThird = document.createElement('input');
        this.alterThird.type = "radio";
        this.alterThird.name = "component";
        this.alterThird.value = "third";                
                
        this.alterFifth = document.createElement('input');
        this.alterFifth.type = "radio";
        this.alterFifth.name = "component";
        this.alterFifth.value = "fifth";                

        this.alterRootLabel = document.createElement('label');
        this.alterRootLabel.innerText = "1";        

        this.alterThirdLabel = document.createElement('label');
        this.alterThirdLabel.innerText = "3";        

        this.alterFifthLabel = document.createElement('label');
        this.alterFifthLabel.innerText = "5";        

        // Dodanie do składników
        [this.alterRoot, this.alterRootLabel, this.alterThird, this.alterThirdLabel, this.alterFifth, this.alterFifthLabel].forEach(e => {
            this.addAlterationFormComponents.appendChild(e);
        });

        // Popup alteracji - opcje składników
        this.addAlterationFormOptions = document.createElement('div');
        
        this.alterUp = document.createElement('input');
        this.alterUp.type = "radio";
        this.alterUp.name = "type";
        this.alterUp.value = "up";
        
        this.alterDown = document.createElement('input');
        this.alterDown.type = "radio";
        this.alterDown.name = "type";
        this.alterDown.value = "down";        

        this.alterUpLabel = document.createElement('label');
        this.alterUpLabel.innerText = "&uarr;";        

        this.alterDownLabel = document.createElement('label');
        this.alterDownLabel.innerText = "&darr;";        

        // Dodanie do opcji składników
        [this.alterDown, this.alterDownLabel, this.alterUp, this.alterUpLabel].forEach(e => {
            this.addAlterationFormOptions.appendChild(e);
        });

        // Popup alteracji - przycisk potwierdź
        this.confirmAddAlteration = document.createElement('input');
        this.confirmAddAlteration.type = 'button';
        this.confirmAddAlteration.name = 'confirm-button';        
        this.confirmAddAlteration.className = `confirm-button`;
        this.confirmAddAlteration.value = "Dodaj";

        // Popup alteracji - przycisk anuluj
        this.cancelAddAlteration = document.createElement('input');
        this.cancelAddAlteration.type = 'button';
        this.cancelAddAlteration.name = 'confirm-button';        
        this.cancelAddAlteration.className = `cancel-button`;
        this.cancelAddAlteration.value = "Anuluj";

        // Dodanie do popupu opóźnienia
        [alterationPopupTitle, this.addAlterationFormComponents, this.addAlterationFormOptions, this.confirmAddAlteration, this.cancelAddAlteration].forEach(e => {
            this.alterationPopup.appendChild(e);
        })

        // Popup składników dodanych
        this.addedPopup = document.createElement('div');
        this.addedPopup.className = "added-popup popup-base";

        // Popup składników dodanych - tytuł
        const addedPopupTitle = document.createElement('span');
        addedPopupTitle.innerText = "Dodaj składnik dysonujący";
        addedPopupTitle.className = "popup-title";

        // Popup składników dodanych - składniki
        this.addAddedFormComponents = document.createElement('div');
        
        this.addSixth = document.createElement('input');
        this.addSixth.name = "component";
        this.addSixth.type = "radio";
        this.addSixth.value = "sixth";
        
        this.addSeventh = document.createElement('input');
        this.addSeventh.name = "component";
        this.addSeventh.type = "radio";
        this.addSeventh.value = "seventh";      
        
        this.addNinth = document.createElement('input');
        this.addNinth.name = "component";
        this.addNinth.type = "radio";
        this.addNinth.value = "ninth";        

        this.addSixthLabel = document.createElement('label');        
        this.addSixthLabel.innerText = "6";        

        this.addSeventhLabel = document.createElement('label');
        this.addSeventhLabel.innerText = "7";        

        this.addNinthLabel = document.createElement('label');
        this.addNinthLabel.innerText = "9";        

        [this.addSixth, this.addSixthLabel, this.addSeventh, this.addSeventhLabel, this.addNinth, this.addNinthLabel].forEach(e => {
            this.addAddedFormComponents.appendChild(e);
        });

        // Popup składników dodanych - opcje
        this.addAddedFormOptions = document.createElement('div');        
                
        this.addedMinor = document.createElement('input');
        this.addedMinor.type = "radio";
        this.addedMinor.name = 'type';
        this.addedMinor.value = 'major';        
                
        this.addedNeutral = document.createElement('input');
        this.addedNeutral.type = "radio";
        this.addedNeutral.name = 'type';
        this.addedNeutral.value = 'neutral';        
        
        this.addedMajor = document.createElement('input');
        this.addedMajor.type = "radio";
        this.addedMajor.name = 'type';
        this.addedMajor.value = 'minor';        

        this.addedMinorLabel = document.createElement('label');
        this.addedMinorLabel.innerText = "&lt;";        

        this.addedNeutralLabel = document.createElement('label');
        this.addedNeutralLabel.innerText = "-";        

        this.addedMajorLabel = document.createElement('label');
        this.addedMajorLabel.innerText = "&gt;";        

        [this.addedMajor, this.addedMajorLabel, this.addedNeutral, this.addedNeutralLabel, this.addedMinor, this.addedMinorLabel].forEach(e => {
            this.addAddedFormOptions.appendChild(e);
        });

        // Popup składników dodanych - przycisk potwierdź
        this.confirmAddAdded = document.createElement('input');
        this.confirmAddAdded.type = 'button';
        this.confirmAddAdded.name = 'confirm-button';        
        this.confirmAddAdded.className = `confirm-button`;
        this.confirmAddAdded.value = "Dodaj";

        // Popup składników dodanych - przycisk anuluj
        this.cancelAddAdded = document.createElement('input');
        this.cancelAddAdded.type = 'button';
        this.cancelAddAdded.name = 'confirm-button';        
        this.cancelAddAdded.className = `cancel-button`;
        this.cancelAddAdded.value = "Anuluj";

        [addedPopupTitle, this.addAddedFormComponents, this.addAddedFormOptions, this.confirmAddAdded, this.cancelAddAdded].forEach(e => {
            this.addedPopup.appendChild(e);
        });        
        
        // Kreator funkcji
        this.functionCreator = document.createElement('div');
        this.functionCreator.className = "function-creator";

        // Kreator funkcji - siatka
        this.gridContainer = document.createElement('div');
        this.gridContainer.className = "grid-container";

        // Kreator funkcji - siatka - lewy nawias
        this.leftBraceContainer = document.createElement('div');
        this.leftBraceContainer.className = "left-brace-container";        

        this.leftBrace = document.createElement('select');
        this.leftBrace.innerHTML = `
            <option value=" " selected> </option>    
            <option value="(">(</option>
            <option value="[">[</option>`;
        this.leftBrace.name = "left-brace";
        this.leftBrace.title = "Wybierz typ wtrącenia - początek";        
        this.leftBraceContainer.appendChild(this.leftBrace);

        // Kreator funkcji - siatka - moll?
        this.minorContainer = document.createElement('div');
        this.minorContainer.className = "minor-container";        

        const label = document.createElement('label');
        label.className = "custom-checkbox";

        this.minor = document.createElement('input');
        this.minor.type = "checkbox";
        this.minor.name = "minor";        

        this.minorSpan = document.createElement('span');
        this.minorSpan.className = "checkmark checkmark-border";
        this.minorSpan.title = "Moll?";        

        [this.minor, this.minorSpan].forEach(e => {
            label.appendChild(e);
        });

        this.minorContainer.appendChild(label);

        // Kreator funkcji - siatka - symbol
        this.symbolContainer = document.createElement('div');
        this.symbolContainer.className = "symbol-container";        

        this.symbol = document.createElement('select');        
        this.symbol.title = "Wybierz symbol";
        this.symbol.name = "symbol";
        this.symbol.style = "font-size: 20px;";

        this.symbolContainer.appendChild(this.symbol);

        // Kreator funkcji - siatka - pozycja
        this.positionContainer = document.createElement('div');
        this.positionContainer.className = "position-container";        

        this.position = document.createElement('select');        
        this.position.title = "Wybierz pozycję";
        this.position.name = "position";

        this.positionContainer.appendChild(this.position);

        // Kreator funkcji - siatka - oparcie
        this.rootContainer = document.createElement('div');
        this.rootContainer.className = "root-container";        

        this.root = document.createElement('select');        
        this.root.title = "Wybierz oparcie";
        this.root.name = "root";

        this.rootContainer.appendChild(this.root);

        // Kreator funkcji - siatka - dodane
        this.addedContainer = document.createElement('div');
        this.addedContainer.className = "added-container";        

        this.added = document.createElement('input');
        this.added.type = "button";
        this.added.name = "add-added";        
        this.added.title = "Dodaj składnik dysonujący";
        this.added.value = " ";

        this.addedContainer.appendChild(this.added);

        // Kreator funkcji - siatka - opóźnienie
        this.suspensionContainer = document.createElement('div');
        this.suspensionContainer.className = "suspension-container";        

        this.suspension = document.createElement('input');
        this.suspension.type = "button";
        this.suspension.name = "add-suspension";        
        this.suspension.title = "Dodaj opóźnienie";
        this.suspension.value = " ";

        this.suspensionContainer.appendChild(this.suspension);

        // Kreator funkcji - siatka - alteracja
        this.alterationContainer = document.createElement('div');
        this.alterationContainer.className = "alteration-container";        

        this.alteration = document.createElement('input');
        this.alteration.type = "button";
        this.alteration.name = "add-alteration";        
        this.alteration.title = "Dodaj alterację";
        this.alteration.value = " ";

        this.alterationContainer.appendChild(this.alteration);

        // Kreator funkcji - siatka - usunięty
        this.removedContainer = document.createElement('div');
        this.removedContainer.className = "removed-container";        

        this.removed = document.createElement('select');        
        this.removed.title = "Wybierz składnik dodany";
        this.removed.name = "removed";

        this.removedContainer.appendChild(this.removed);

        // Kreator funkcji - siatka - prawy nawias
        this.rightBraceContainer = document.createElement('div');
        this.rightBraceContainer.className = "right-brace-container";        

        this.rightBrace = document.createElement('select');
        this.rightBrace.name = 'right-brace';
        this.rightBrace.title = "Wybierz typ wtrącenia - koniec";        
        this.rightBrace.innerHTML = `
            <option value=" " selected> </option>
            <option value=")">)</option>
            <option value="]">]</option>`;

        this.rightBraceContainer.appendChild(this.rightBrace);

        [this.leftBraceContainer, this.minorContainer, this.symbolContainer, this.positionContainer, this.rootContainer,
         this.addedContainer, this.suspensionContainer, this.alterationContainer, this.removedContainer,
         this.rightBraceContainer].forEach(e => {
             this.gridContainer.appendChild(e);
        });

        // Formularz zatwierdzenia
        this.formButtons = document.createElement('div');        
        this.formButtons.class = "function-form-buttons";

        // Formularz zatwierdzenia - przycisk anuluj
        this.cancelCreator = document.createElement('input');
        this.cancelCreator.type = "button";
        this.cancelCreator.name = "cancel-creator";        
        this.cancelCreator.title = "Usuń funkcję";
        this.cancelCreator.className = "custom-button trash-button button-small";
        this.cancelCreator.value = " ";

        // Formularz zatwierdzenia - przycisk resetuj
        this.resetCreator = document.createElement('input');
        this.resetCreator.type = "button";
        this.resetCreator.name = "reset-creator";        
        this.resetCreator.title = "Resetuj funkcję";
        this.resetCreator.className = "custom-button round-arrow-button button-small";
        this.resetCreator.value = " ";

        // Formularz zatwierdzenia - przycisk potwierdź
        this.submitCreator = document.createElement('input');
        this.submitCreator.type = "button";
        this.submitCreator.name = "submit-creator";        
        this.submitCreator.title = "Zatwierdź funkcję";
        this.submitCreator.className = "custom-button tick-button button-small";
        this.submitCreator.value = " ";

        [this.cancelCreator, this.resetCreator, this.submitCreator].forEach(e => {
            this.formButtons.appendChild(e);
        });

        [this.gridContainer, this.formButtons].forEach(e => {
            this.functionCreator.appendChild(e);
        });

        [this.suspensionPopup,
         this.alterationPopup,
         this.addedPopup,
         this.functionCreator].forEach(e => {
            this.component.appendChild(e);
        });

        // Ustawienie ID'ków wszystkiego co trzeba
        this.setIds(this.id);
    }

    // TODO: Wszystko z "ID SET" powinno wywędrować tutaj
    setIds(newId) {
        this.id = newId;
        this.component.id = newId;

        this.suspensionPopup.id = `suspension-popup-${this.id}`; // ID SET
        this.confirmAddSuspension.id = `confirm-add-suspension-${this.id}`; // ID SET
        this.cancelAddSuspension.id = `cancel-add-suspension-${this.id}`; // ID SET
        this.alterationPopup.id = `alteration-popup-${this.id}`; // ID SET

        const alterRootId = `alter-root-${this.id}`; // ID SET
        this.alterRoot.id = alterRootId; // ID SET
        this.alterRootLabel.for = alterRootId; // ID SET

        const alterThirdId = `alter-third-${this.id}`; // ID SET
        this.alterThird.id = alterThirdId; // ID SET
        this.alterThirdLabel.for = alterThirdId; // ID SET

        const alterFifthId = `alter-fifth-${this.id}`; // ID SET
        this.alterFifth.id = alterFifthId; // ID SET
        this.alterFifthLabel.for = alterFifthId; // ID SET

        const alterUpId = `up-${this.id}`; // ID SET
        this.alterUp.id = alterUpId; // ID SET
        this.alterUpLabel.for = alterUpId; // ID SET

        const alterDownId = `down-${this.id}`; // ID SET
        this.alterDown.id = alterDownId; // ID SET
        this.alterDownLabel.for = alterDownId; // ID SET

        this.confirmAddAlteration.id = `confirm-add-alteration-${this.id}`; // ID SET
        this.cancelAddAlteration.id = `cancel-add-alteration-${this.id}`; // ID SET
        this.addedPopup.id = `added-popup-${this.id}`; // ID SET
        this.addAddedFormComponents.id = `add-added-form-${this.id}-components`; // ID SET

        this.addAddedFormOptions.id = `add-added-form-${this.id}-options`; // ID SET

        const addSixthId = `add-sixth-${this.id}`; // ID SET
        this.addSixth.id = addSixthId; // ID SET
        this.addSixthLabel.for = addSixthId; // ID SET

        const addSeventhId = `add-seventh-${this.id}`; // ID SET
        this.addSeventh.id = addSeventhId; // ID SET
        this.addSeventhLabel.for = addSeventhId; // ID SET

        const addNinthId = `add-ninth-${this.id}`; // ID SET
        this.addNinth.id = addNinthId; // ID SET
        this.addNinthLabel.for = addNinthId; // ID SET

        const addedMinorId = `added-minor-${this.id}`; // ID SET
        this.addedMinor.id = addedMinorId; // ID SET
        this.addedMinorLabel.for = addedMinorId; // ID SET

        const addedNeutralId = `added-neutral-${this.id}`; // ID SET
        this.addedNeutral.id = addedNeutralId; // ID SET
        this.addedNeutralLabel.for = addedNeutralId; // ID SET

        const addedMajorId = `added-major-${this.id}`; // ID SET
        this.addedMajor.id = addedMajorId; // ID SET
        this.addedMajorLabel.for = addedMajorId; // ID SET

        this.confirmAddAdded.id = `confirm-add-added-${this.id}`; // ID SET
        this.cancelAddAdded.id = `cancel-add-added-${this.id}`; // ID SET

        this.leftBraceContainer.id = `left-brace-container-${this.id}`; // ID SET
        this.leftBrace.id = `left-brace-${this.id}`; // ID SET

        this.minorContainer.id = `minor-container-${this.id}`; // ID SET
        this.minor.id = `minor-${this.id}`; // ID SET

        this.minorSpan.id = `minor-span-${this.id}`; // ID SET

        this.symbolContainer.id = `symbol-container-${this.id}`; // ID SET
        this.symbol.id = `symbol-${this.id}`; // ID SET

        this.positionContainer.id = `position-container-${this.id}`; // ID SET
        this.position.id = `position-${this.id}`; // ID SET

        this.rootContainer.id = `root-container-${this.id}`; // ID SET
        this.root.id = `root-${this.id}`; // ID SET

        this.addedContainer.id = `added-container-${this.id}`; // ID SET
        this.added.id = `add-added-${this.id}`; // ID SET

        this.suspensionContainer.id = `suspension-container-${this.id}`; // ID SET
        this.suspension.id = `add-suspension-${this.id}`; // ID SET

        this.alterationContainer.id = `alteration-container-${this.id}`; // ID SET
        this.alteration.id = `add-alteration-${this.id}`; // ID SET

        this.removedContainer.id = `removed-container-${this.id}`; // ID SET
        this.removed.id = `removed-${this.id}`; // ID SET

        this.rightBraceContainer.id = `right-brace-container-${this.id}`; // ID SET
        this.rightBrace.id = `right-brace-${this.id}`; // ID SET

        this.formButtons.id = `form-buttons-${this.id}`; // ID SET

        this.cancelCreator.id = `cancel-creator-${this.id}`; // ID SET
        this.resetCreator.id = `reset-creator-${this.id}`; // ID SET
        this.submitCreator.id = `submit-creator-${this.id}`; // ID SET
    }

    getElement() {
        return this.component;
    }
}