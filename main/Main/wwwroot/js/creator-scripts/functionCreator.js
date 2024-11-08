import { ParsedFunction } from "./parsedFunction.js";

export class Function {
    constructor(functionContainer, functionIndex) {
        //console.log("Function");
        this.functionContainer = functionContainer;        
        
        this.taskIndex = functionContainer.taskIndex;
        this.functionIndex = functionIndex;
        this.barIndex = functionContainer.barIndex;

        this.functionCreator = new FunctionCreator(this.taskIndex, this.barIndex, this.functionIndex);

        this.remover = this.functionCreator.cancelCreator;
        this.resetter = this.functionCreator.resetCreator;

        this.handleRemove = this.functionContainer.removeFunction.bind(this.functionContainer, this.functionIndex);
        this.handleReset = this.functionCreator.resetAll.bind(this.functionContainer);

        this.addListeners();
    }

    setId(taskIndex, barIndex, functionIndex) {
        this.removeListeners();

        this.taskIndex = taskIndex;
        this.barIndex = barIndex;
        this.functionIndex = functionIndex;

        this.functionCreator.setId(taskIndex, barIndex, functionIndex);

        this.handleRemove = this.functionContainer.removeFunction.bind(this.functionContainer, this.functionIndex);
        this.handleReset = this.functionCreator.resetAll.bind(this.functionContainer);
        this.addListeners();
    }

    removeListeners() {
        this.remover.removeEventListener('click', this.handleRemove);
        this.resetter.removeEventListener('click', this.handleReset);
    }

    addListeners() {
        this.remover.addEventListener('click', this.handleRemove);
        this.resetter.addEventListener('click', this.handleReset);
    }
}

class FunctionCreator {
    constructor(taskIndex, barIndex, functionIndex) {
        //console.log("FunctionCreator");
        this.id = `x-${taskIndex}-${barIndex}-${functionIndex}`;

        this.taskIndex = taskIndex;
        this.barIndex = barIndex;
        this.functionIndex = functionIndex;

        this.container = document.createElement('div');        
        this.container.className = "task-box";

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
        this.alterUpLabel.innerHTML = "&uarr;";        

        this.alterDownLabel = document.createElement('label');
        this.alterDownLabel.innerHTML = "&darr;";        

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
        this.addedMinorLabel.innerHTML = "&lt;";        

        this.addedNeutralLabel = document.createElement('label');
        this.addedNeutralLabel.innerText = "-";        

        this.addedMajorLabel = document.createElement('label');
        this.addedMajorLabel.innerHTML = "&gt;";        

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

        [this.cancelCreator, this.resetCreator].forEach(e => {
            this.formButtons.appendChild(e);
        });

        [this.gridContainer, this.formButtons].forEach(e => {
            this.functionCreator.appendChild(e);
        });

        [this.suspensionPopup,
         this.alterationPopup,
         this.addedPopup,
         this.functionCreator].forEach(e => {
            this.container.appendChild(e);
        });

        // Ustawienie ID'ków wszystkiego co trzeba
        this.setId(this.taskIndex, this.barIndex, this.functionIndex);

        this.avaiableAlterations = ['up', 'down'];
        this.alterationList = [];
        this.addedList = [];

        this.allComponents = [this.minor, this.position, this.root,
        this.symbol, this.removed, this.added, this.alteration,
        this.suspension, this.leftBrace, this.rightBrace];

        this.controlButtons = [this.resetCreator, this.cancelCreator];

        this.allPopups = [this.addedPopup, this.suspensionPopup, this.alterationPopup];

        this.populateDropdowns();
        this.addOnClickEvents();
    }

    setId(taskIndex, barIndex, functionIndex) {
        const newId = `x-${taskIndex}-${barIndex}-${functionIndex}`
        this.id = newId;
        this.container.id = newId;
        this.taskIndex = taskIndex;
        this.barIndex = barIndex;
        this.functionIndex = functionIndex;

        this.suspensionPopup.id = `suspension-popup-${this.id}`;
        this.confirmAddSuspension.id = `confirm-add-suspension-${this.id}`;
        this.cancelAddSuspension.id = `cancel-add-suspension-${this.id}`;
        this.alterationPopup.id = `alteration-popup-${this.id}`;

        const alterRootId = `alter-root-${this.id}`;
        this.alterRoot.id = alterRootId;
        this.alterRootLabel.for = alterRootId;

        const alterThirdId = `alter-third-${this.id}`;
        this.alterThird.id = alterThirdId;
        this.alterThirdLabel.for = alterThirdId;

        const alterFifthId = `alter-fifth-${this.id}`;
        this.alterFifth.id = alterFifthId;
        this.alterFifthLabel.for = alterFifthId;

        const alterUpId = `up-${this.id}`;
        this.alterUp.id = alterUpId;
        this.alterUpLabel.for = alterUpId;

        const alterDownId = `down-${this.id}`;
        this.alterDown.id = alterDownId;
        this.alterDownLabel.for = alterDownId;

        this.confirmAddAlteration.id = `confirm-add-alteration-${this.id}`;
        this.cancelAddAlteration.id = `cancel-add-alteration-${this.id}`;
        this.addedPopup.id = `added-popup-${this.id}`;
        this.addAddedFormComponents.id = `add-added-form-${this.id}-components`;

        this.addAddedFormOptions.id = `add-added-form-${this.id}-options`;

        const addSixthId = `add-sixth-${this.id}`;
        this.addSixth.id = addSixthId;
        this.addSixthLabel.for = addSixthId;

        const addSeventhId = `add-seventh-${this.id}`;
        this.addSeventh.id = addSeventhId;
        this.addSeventhLabel.for = addSeventhId;

        const addNinthId = `add-ninth-${this.id}`;
        this.addNinth.id = addNinthId;
        this.addNinthLabel.for = addNinthId;

        const addedMinorId = `added-minor-${this.id}`;
        this.addedMinor.id = addedMinorId;
        this.addedMinorLabel.for = addedMinorId;

        const addedNeutralId = `added-neutral-${this.id}`;
        this.addedNeutral.id = addedNeutralId;
        this.addedNeutralLabel.for = addedNeutralId;

        const addedMajorId = `added-major-${this.id}`;
        this.addedMajor.id = addedMajorId;
        this.addedMajorLabel.for = addedMajorId;

        this.confirmAddAdded.id = `confirm-add-added-${this.id}`;
        this.cancelAddAdded.id = `cancel-add-added-${this.id}`;

        this.leftBraceContainer.id = `left-brace-container-${this.id}`;
        this.leftBrace.id = `left-brace-${this.id}`;

        this.minorContainer.id = `minor-container-${this.id}`;
        this.minor.id = `minor-${this.id}`;

        this.minorSpan.id = `minor-span-${this.id}`;

        this.symbolContainer.id = `symbol-container-${this.id}`;
        this.symbol.id = `symbol-${this.id}`;

        this.positionContainer.id = `position-container-${this.id}`;
        this.position.id = `position-${this.id}`;

        this.rootContainer.id = `root-container-${this.id}`;
        this.root.id = `root-${this.id}`;

        this.addedContainer.id = `added-container-${this.id}`;
        this.added.id = `add-added-${this.id}`;

        this.suspensionContainer.id = `suspension-container-${this.id}`;
        this.suspension.id = `add-suspension-${this.id}`;

        this.alterationContainer.id = `alteration-container-${this.id}`;
        this.alteration.id = `add-alteration-${this.id}`;

        this.removedContainer.id = `removed-container-${this.id}`;
        this.removed.id = `removed-${this.id}`;

        this.rightBraceContainer.id = `right-brace-container-${this.id}`;
        this.rightBrace.id = `right-brace-${this.id}`;

        this.formButtons.id = `form-buttons-${this.id}`;

        this.cancelCreator.id = `cancel-creator-${this.id}`;
        this.resetCreator.id = `reset-creator-${this.id}`;
    }

    resetAll() {
        this.enableItems(this.allComponents);
        this.confirmed = false;

        [this.position, this.symbol, this.removed, this.root].forEach(component => {
            component.value = ' ';
        });

        this.allComponents.forEach(e => {
            e.classList.remove('hide-border');
        });

        this.addedList.length = 0;

        const container = document.getElementById(`added-container-${this.id}`);
        const items = container.querySelectorAll('.added-component');

        items.forEach(element => {
            container.removeChild(element);
        });

        const none = "none";
        this.addedPopup.style.display = none;
        this.suspensionPopup.style.display = none;
        this.alterationPopup.style.display = none;
    }

    // Ta funkcja tworzy i wysyła obiekt, który potem jest zapisywany do bazy
    // Typ: ParsedFunction
    confirm() {
        this.allPopups.forEach(p => {
            p.style.display = "none";
        });

        const minor = this.minor.checked;
        const symbol = this.symbol.value;
        const position = this.position.value;
        const root = this.root.value;
        const removed = this.removed.value;
        const alterations = this.alterationsList;
        const added = this.addedList;

        if (symbol === "") {
            alert('Nie można dodać pustej funkcji!'); // TODO: Dać jakieś ładniejsze coś XD
            return null;
        }

        // TODO: To chyba można pominąć ostatecznie...
        //this.allComponents.forEach(e => {
        //    e.classList.add('hide-border');
        //});
        // this.disableItems(this.allComponents);
        // this.confirmed = true;

        let functionResult = new ParsedFunction(
            this.barIndex, this.functionIndex,
            minor, symbol, position, root, removed,
            alterations, added
        );

        return functionResult;
    }

    populateDropdowns() {
        this.addEmpty();
        this.populatePosition();
        this.populateSymbols();
        this.populateRoot();
        this.populateRemoved();
    }

    populatePosition() { this.populateDropdown(this.position, ['1', '3', '5', '6', '7', '9']); }
    populateRoot() { this.populateDropdown(this.root, ['1', '3', '5', '7']); }
    populateSymbols() { this.populateDropdown(this.symbol, ['T', 'Sii', 'Tiii', 'Diii', 'S', 'D', 'Tvi', 'Svi', 'Dvii', 'Svii']); }
    populateRemoved() { this.populateDropdown(this.removed, ['1', '5']); }

    populateDropdown(dropdown, components) {
        components.forEach(element => {
            const opt = document.createElement('option');
            opt.value = element;
            opt.innerText = element;
            dropdown.appendChild(opt);
        });
    }

    addEmpty() {
        [this.position, this.root, this.symbol, this.removed].forEach(element => {
            element.appendChild(document.createElement('option'));
        });
    }

    addOnClickEvents() {
        this.added.addEventListener('click', (event) => { this.addAdded(event); });
        this.suspension.addEventListener('click', (event) => { this.addSuspension(event); });
        this.alteration.addEventListener('click', (event) => { this.addAlteration(event); });
        this.resetCreator.addEventListener('click', () => { this.resetAll(); });

        this.addAddedOnClickEvents();
    }

    addAddedOnClickEvents() {
        const components = this.addedPopup
            .querySelectorAll('div')[0]
            .querySelectorAll('input[type="radio"]');

        const options = this.addedPopup
            .querySelectorAll('div')[1]
            .querySelectorAll('input[type="radio"]');

        const confirmButton = this.confirmAddAdded;

        confirmButton.addEventListener('click', () => {
            let checkedComponent = "";
            let checkedOption = "";

            components.forEach(c => {
                if (c.checked) {
                    switch (c.value) {
                        case 'sixth':
                            checkedComponent = '6';
                            break;
                        case 'seventh':
                            checkedComponent = '7';
                            break;
                        case 'ninth':
                            checkedComponent = '9';
                            break;
                    }
                }
            });

            options.forEach(c => {
                if (c.checked) {
                    switch (c.value) {
                        case 'major':
                            checkedOption = "<";
                            break;
                        case 'neutral':
                            checkedOption = "-";
                            break;
                        case 'minor':
                            checkedOption = ">";
                            break;
                    }
                }
            });

            if (checkedComponent !== "" && checkedOption !== "") {
                if (document.getElementById(`added-container-${this.id}`).children.length < 5) {
                    this.createAddedElement(checkedComponent, checkedOption);
                }

                this.togglePopupOff(this.addedPopup);
            }
        });
    }    

    createAddedElement(component, option) {
        const container = document.getElementById(`added-container-${this.id}`);
        let element = document.createElement('input');

        if (option == "-")
            option = "";

        let val = `${component}${option}`;
        element.type = "button";
        element.value = val;
        element.className = "added-component";

        element.addEventListener('click', () => {
            if (container.contains(element)) {
                container.removeChild(element);
                this.addedList.pop(val);
            }
        });

        this.addedList.push(val);
        container.appendChild(element);
    }

    addAdded(event) {
        this.togglePopupOn(this.addedPopup, event);
        this.disableItems(this.allComponents);
    }

    addAlteration(event) {
        this.togglePopupOn(this.alterationPopup, event);
        this.disableItems(this.allComponents);
    }

    addSuspension(event) {
        this.togglePopupOn(this.suspensionPopup, event);
        this.disableItems(this.allComponents);
    }

    addComponent(popup) {
        this.togglePopupOff(popup);
    }

    togglePopupOn(popup, event) {
        if (popup.style.display === "block") {
            popup.style.display = "none";
        }
        else {
            popup.style.display = "block";
        }

        const button = event.currentTarget;
        const rect = button.getBoundingClientRect();

        popup.style.left = (rect.left - (popup.style.width / 2)) + "px";
        popup.style.top = (rect.top + button.offsetHeight) + "px";

        const cancelButton = popup.querySelectorAll('input[type="button"]')[1];
        cancelButton.addEventListener('click', () => { this.togglePopupOff(popup); });
    }

    togglePopupOff(popup) {
        popup.style.display = "none";
        this.enableItems(this.allComponents);
    }    

    disableItems(items) {
        items.forEach(item => {
            this.disableElement(item);
        });

        let minorSpan = document.getElementById(`minor-span-${this.id}`);
        minorSpan.classList.remove('checkmark-border');
    }

    enableItems(items) {
        items.forEach(item => {
            this.enableElement(item);
        });

        let minorSpan = document.getElementById(`minor-span-${this.id}`);
        minorSpan.classList.add('checkmark-border');
    }

    enableElement(button) { button.disabled = false; }
    disableElement(button) { button.disabled = true; }
}
