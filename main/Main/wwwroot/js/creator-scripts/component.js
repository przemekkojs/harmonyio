export class Component {
    constructor(id) {
        this.id = id;

        this.component = document.createElement('div');
        this.component.id = `task-box-${id}`;
        this.component.className = 'task-box';

        this.suspensionPopup = document.createElement('div');
        this.confirmButton = document.createElement('input');
        this.cancelButton = document.createElement('input');
        this.alterationPopup = document.createElement('div');

        this.createSuspensionPopup();
        this.createAlterationPopup();
        this.createAddedPopup();
        this.createFunctionCreator();
    }

    createSuspensionPopup() {
        this.suspensionPopup.className = 'suspension-popup popup-base';
        this.suspensionPopup.id = `suspension-popup-${this.id}`;

        const title = document.createElement('span');
        title.className = 'popup-title';
        title.textContent = 'Dodaj opóźnienie';
        this.suspensionPopup.appendChild(title);
        
        this.confirmButton.type = 'button';
        this.confirmButton.name = 'confirm-button';
        this.confirmButton.id = `confirm-add-suspension-${this.id}`;
        this.confirmButton.value = 'Dodaj opóźnienie';
        this.confirmButton.className = 'confirm-button';
        this.suspensionPopup.appendChild(confirmButton);

        this.cancelButton.type = 'button';
        this.cancelButton.name = 'cancel-button';
        this.cancelButton.id = `cancel-add-suspension-${this.id}`;
        this.cancelButton.value = 'Anuluj opóźnienie';
        this.cancelButton.className = 'cancel-button';
        this.suspensionPopup.appendChild(cancelButton);

        this.component.appendChild(suspensionPopup);
    }

    createAlterationPopup() {        
        this.alterationPopup.className = 'alteration-popup popup-base';
        this.alterationPopup.id = `alteration-popup-${this.id}`;

        const title = document.createElement('span');
        title.className = 'popup-title';
        title.textContent = 'Dodaj alterację';
        this.alterationPopup.appendChild(title);

        const componentChoice = document.createElement('div');
        componentChoice.className = 'component-choice';

        ['root', 'third', 'fifth'].forEach((value, index) => {
            const input = document.createElement('input');
            input.type = 'radio';
            input.name = 'component';
            input.id = `alter-${value}-${this.id}`;
            input.value = value;
            componentChoice.appendChild(input);

            const label = document.createElement('label');
            label.setAttribute('for', input.id);
            label.textContent = index + 1;
            componentChoice.appendChild(label);
        });

        this.alterationPopup.appendChild(componentChoice);

        const radioButtons = document.createElement('div');
        radioButtons.className = 'radio-buttons';

        ['down', 'up'].forEach((value) => {
            const input = document.createElement('input');
            input.type = 'radio';
            input.name = 'type';
            input.id = `${value}-${this.id}`;
            input.value = value;
            radioButtons.appendChild(input);

            const label = document.createElement('label');
            label.setAttribute('for', input.id);
            label.innerHTML = value === 'down' ? '&darr;' : '&uarr;';
            radioButtons.appendChild(label);
        });

        alterationPopup.appendChild(radioButtons);

        const confirmButton = document.createElement('input');
        confirmButton.type = 'button';
        confirmButton.name = 'confirm-button';
        confirmButton.id = `confirm-add-alteration-${this.id}`;
        confirmButton.value = 'Dodaj alterację';
        confirmButton.className = 'confirm-button';
        alterationPopup.appendChild(confirmButton);

        const cancelButton = document.createElement('input');
        cancelButton.type = 'button';
        cancelButton.name = 'cancel-button';
        cancelButton.id = `cancel-add-alteration-${this.id}`;
        cancelButton.value = 'Anuluj alterację';
        cancelButton.className = 'cancel-button';
        alterationPopup.appendChild(cancelButton);

        this.component.appendChild(alterationPopup);
    }

    createAddedPopup() {
        const addedPopup = document.createElement('div');
        addedPopup.className = 'added-popup popup-base';
        addedPopup.id = `added-popup-${this.id}`;

        const title = document.createElement('span');
        title.className = 'popup-title';
        title.textContent = 'Dodaj składnik';
        addedPopup.appendChild(title);

        const componentsDiv = document.createElement('div');
        componentsDiv.id = `add-added-form-${this.id}-components`;

        ['sixth', 'seventh', 'ninth'].forEach((value, index) => {
            const input = document.createElement('input');
            input.type = 'radio';
            input.name = 'component';
            input.id = `add-${value}-${this.id}`;
            input.value = value;
            componentsDiv.appendChild(input);

            const label = document.createElement('label');
            label.setAttribute('for', input.id);
            label.textContent = index + 6;
            componentsDiv.appendChild(label);
        });

        addedPopup.appendChild(componentsDiv);

        const optionsDiv = document.createElement('div');
        optionsDiv.id = `add-added-form-${this.id}-options`;

        [['major', '&lt;'], ['neutral', '-'], ['minor', '&gt;']].forEach(([value, symbol]) => {
            const input = document.createElement('input');
            input.type = 'radio';
            input.name = 'type';
            input.id = `added-${value}-${this.id}`;
            input.value = value;
            if (value === 'neutral') input.checked = true;
            optionsDiv.appendChild(input);

            const label = document.createElement('label');
            label.setAttribute('for', input.id);
            label.innerHTML = symbol;
            optionsDiv.appendChild(label);
        });

        addedPopup.appendChild(optionsDiv);

        const confirmButton = document.createElement('input');
        confirmButton.type = 'button';
        confirmButton.name = 'confirm-button';
        confirmButton.id = `confirm-add-added-${this.id}`;
        confirmButton.value = 'Dodaj składnik';
        confirmButton.className = 'confirm-button';
        addedPopup.appendChild(confirmButton);

        const cancelButton = document.createElement('input');
        cancelButton.type = 'button';
        cancelButton.name = 'cancel-button';
        cancelButton.id = `cancel-add-added-${this.id}`;
        cancelButton.value = 'Anuluj składnik';
        cancelButton.className = 'cancel-button';
        addedPopup.appendChild(cancelButton);

        this.component.appendChild(addedPopup);
    }

    createFunctionCreator() {
        const functionCreator = document.createElement('div');
        functionCreator.className = 'function-creator';

        const container = document.createElement('div');
        container.className = 'grid-container';

        const leftBraceContainer = document.createElement("div");
        leftBraceContainer.classList.add("left-brace-container");
        leftBraceContainer.id = `left-brace-container-${newId}`;
        const leftBraceSelect = document.createElement("select");
        leftBraceSelect.name = "left-brace";
        leftBraceSelect.id = `left-brace-${newId}`;
        leftBraceSelect.title = "Wybierz typ wtrącenia";
        leftBraceSelect.innerHTML = `
        <option value=" " selected> </option>
        <option value="(">(</option>
        <option value="[">[</option>
    `;
        leftBraceContainer.appendChild(leftBraceSelect);

        const minorContainer = document.createElement("div");
        minorContainer.classList.add("minor-container");
        minorContainer.id = `minor-container-${newId}`;

        const minorLabel = document.createElement("label");
        minorLabel.classList.add("custom-checkbox");

        const minorCheckbox = document.createElement("input");
        minorCheckbox.type = "checkbox";
        minorCheckbox.name = "minor";
        minorCheckbox.id = `minor-${newId}`;

        const minorSpan = document.createElement("span");
        minorSpan.classList.add("checkmark", "checkmark-border");
        minorSpan.title = "Moll?";
        minorSpan.id = `minor-span-${newId}`;
        minorLabel.appendChild(minorCheckbox);
        minorLabel.appendChild(minorSpan);
        minorContainer.appendChild(minorLabel);

        const symbolContainer = document.createElement("div");
        symbolContainer.classList.add("symbol-container");
        symbolContainer.id = `symbol-container-${newId}`;

        const symbolSelect = document.createElement("select");
        symbolSelect.name = "symbol";
        symbolSelect.id = `symbol-${newId}`;
        symbolSelect.title = "Wybierz symbol funkcji";
        symbolSelect.style.fontSize = "20px";
        symbolContainer.appendChild(symbolSelect);

        const positionContainer = document.createElement("div");
        positionContainer.classList.add("position-container");
        positionContainer.id = `position-container-${newId}`;

        const positionSelect = document.createElement("select");
        positionSelect.name = "position";
        positionSelect.id = `position-${newId}`;
        positionSelect.title = "Wybierz pozycję";
        positionContainer.appendChild(positionSelect);

        const rootContainer = document.createElement("div");
        rootContainer.classList.add("root-container");
        rootContainer.id = `root-container-${newId}`;

        const rootSelect = document.createElement("select");
        rootSelect.name = "root";
        rootSelect.id = `root-${newId}`;
        rootSelect.title = "Wybierz oparcie";
        rootContainer.appendChild(rootSelect);

        const addedContainer = document.createElement("div");
        addedContainer.classList.add("added-container");
        addedContainer.id = `added-container-${newId}`;

        const addAddedButton = document.createElement("input");
        addAddedButton.type = "button";
        addAddedButton.name = "add-added";
        addAddedButton.id = `add-added-${newId}`;
        addAddedButton.title = "Dodaj składnik dysonujący";
        addedContainer.appendChild(addAddedButton);

        const suspensionContainer = document.createElement("div");
        suspensionContainer.classList.add("suspension-container");
        suspensionContainer.id = `suspension-container-${newId}`;

        const addSuspensionButton = document.createElement("input");
        addSuspensionButton.type = "button";
        addSuspensionButton.name = "add-suspension";
        addSuspensionButton.id = `add-suspension-${newId}`;
        addSuspensionButton.title = "Dodaj opóźnienie";
        suspensionContainer.appendChild(addSuspensionButton);

        const alterationContainer = document.createElement("div");
        alterationContainer.classList.add("alteration-container");
        alterationContainer.id = `alteration-container-${newId}`;

        const addAlterationButton = document.createElement("input");
        addAlterationButton.type = "button";
        addAlterationButton.name = "add-alteration";
        addAlterationButton.id = `add-alteration-${newId}`;
        addAlterationButton.title = "Dodaj alterację";
        alterationContainer.appendChild(addAlterationButton);

        const removedContainer = document.createElement("div");
        removedContainer.classList.add("removed-container");
        removedContainer.id = `removed-container-${newId}`;

        const removedSelect = document.createElement("select");
        removedSelect.name = "removed";
        removedSelect.id = `removed-${newId}`;
        removedSelect.title = "Wybierz składnik dodany";
        removedContainer.appendChild(removedSelect);

        const rightBraceContainer = document.createElement("div");
        rightBraceContainer.classList.add("right-brace-container");
        rightBraceContainer.id = `right-brace-container-${newId}`;

        const rightBraceSelect = document.createElement("select");
        rightBraceSelect.name = "right-brace";
        rightBraceSelect.id = `right-brace-${newId}`;
        rightBraceSelect.title = "Wybierz typ wtrącenia";
        rightBraceSelect.innerHTML = `
            <option value=" " selected> </option>
            <option value=")">)</option>
            <option value="]">]</option>
        `;

        rightBraceContainer.appendChild(rightBraceSelect);

        container.appendChild(leftBraceContainer);
        container.appendChild(minorContainer);
        container.appendChild(symbolContainer);
        container.appendChild(positionContainer);
        container.appendChild(rootContainer);
        container.appendChild(addedContainer);
        container.appendChild(suspensionContainer);
        container.appendChild(alterationContainer);
        container.appendChild(removedContainer);
        container.appendChild(rightBraceContainer);

        functionCreator.appendChild(gridContainer);

        const formButtons = document.createElement('div');
        formButtons.id = `form-buttons-${this.id}`;
        formButtons.className = 'function-form-buttons';

        [['cancel', 'trash-button'], ['reset', 'round-arrow-button'], ['submit', 'tick-button']].forEach(
            ([action, style]) => {
                const button = document.createElement('input');
                button.type = 'button';
                button.name = `${action}-creator`;
                button.id = `${action}-creator-${this.id}`;
                button.className = `custom-button ${style} button-small`;
                formButtons.appendChild(button);
            }
        );

        functionCreator.appendChild(formButtons);
        this.component.appendChild(functionCreator);
    }
}
