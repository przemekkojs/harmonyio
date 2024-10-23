export class Elements {
    constructor(thisId) {
        this.thisId = thisId;
        this.thisDiv = document.getElementById(thisId);
    
        this.positionDropdown = this.thisDiv.querySelector(`#position-${thisId}`);
        this.rootDropdown = this.thisDiv.querySelector(`#root-${thisId}`);
        this.symbolDropdown = this.thisDiv.querySelector(`#symbol-${thisId}`);
        this.removedDropdown = this.thisDiv.querySelector(`#removed-${thisId}`);

        this.addedButton = this.thisDiv.querySelector(`#add-added-${thisId}`);
        this.alterationButton = this.thisDiv.querySelector(`#add-alteration-${thisId}`);
        this.suspensionButton = this.thisDiv.querySelector(`#add-suspension-${thisId}`);

        this.resetButton = this.thisDiv.querySelector(`#reset-creator-${thisId}`);
        this.cancelButton = this.thisDiv.querySelector(`#cancel-creator-${thisId}`);
        this.addButton = this.thisDiv.querySelector(`#submit-creator-${thisId}`);

        this.addedPopup = this.thisDiv.querySelector(`#added-popup-${thisId}`);
        this.suspensionPopup = this.thisDiv.querySelector(`#suspension-popup-${thisId}`);
        this.alterationPopup = this.thisDiv.querySelector(`#alteration-popup-${thisId}`);

        this.avaiableAlterations = ['up', 'down'];    
        this.minor = 'm';

        this.allComponents = [this.positionDropdown, this.rootDropdown,
            this.symbolDropdown, this.removedDropdown, this.addedButton,
            this.alterationButton];//, this.suspensionButton];

        this.controlButtons = [this.resetButton, this.cancelButton, this.addButton];

        this.populateDropdowns();
        this.addOnClickEvents();
    }

    populateDropdowns() {
        this.addEmpty();
        this.populatePosition();
        this.populateSymbols();
        this.populateRoot();
        this.populateRemoved();
    }
    
    addEmpty() {
        [this.positionDropdown, this.rootDropdown, this.symbolDropdown, this.removedDropdown].forEach(element => {
            element.appendChild(document.createElement('option'));
        });
    }
    
    populatePosition() { this.populateDropdown(this.positionDropdown, ['1', '3', '5', '6', '7', '9']); }
    populateRoot() { this.populateDropdown(this.rootDropdown, ['1', '3', '5', '7']); }
    populateSymbols() { this.populateDropdown(this.symbolDropdown, ['T', 'Sii', 'Tiii', 'Diii', 'S', 'D', 'Tvi', 'Svi', 'Dvii', 'Svii']); }
    populateRemoved() { this.populateDropdown(this.removedDropdown, ['1', '5']); }
    
    populateDropdown(dropdown, components) {
        components.forEach(element => {
            const opt = document.createElement('option');
            opt.value = element;
            opt.innerText = element;
            dropdown.appendChild(opt);
        });
    }
    
    addOnClickEvents()
    {
        this.addedButton.addEventListener('click', (event) => { this.addAdded(event); });
        // this.suspensionButton.addEventListener('click', (event) => { this.addSuspension(event); });
        this.alterationButton.addEventListener('click', (event) => { this.addAlteration(event); });
        this.resetButton.addEventListener('click', () => { this.resetAll(); });

        this.addAddedOnClickEvents();
    }

    addAddedOnClickEvents() {
        const components = this.addedPopup
            .querySelectorAll('div')[0]
            .querySelectorAll('input[type="radio"]');

        const options = this.addedPopup
            .querySelectorAll('div')[1]
            .querySelectorAll('input[type="radio"]');
        
        const confirmButton = this.addedPopup.querySelectorAll('input[type="button"]')[0];

        confirmButton.addEventListener('click', () => {
            let checkedComponent = "";
            let checkedOption = "";

            components.forEach(c => {
                if (c.checked) {
                    switch(c.value) {
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
                    switch(c.value) {
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

            if (checkedComponent === "" || checkedOption === "") {
                console.log("Cannot add empty");                
            }
            else {
                if (document.getElementById(`added-container-${this.thisId}`).children.length < 5) {
                    this.createAddedElement(checkedComponent, checkedOption);
                }
                
                this.togglePopupOff(this.addedPopup);
            }
        });        
    }
    
    addAdded(event) {    
        this.togglePopupOn(this.addedPopup, event);
        Elements.disableItems(this.allComponents);
    }

    createAddedElement(component, option) {
        const container = document.getElementById(`added-container-${this.thisId}`);
        let element = document.createElement('input');
        element.type = "button";
        element.value = `${component}${option}`;
        // element.id = `remove-added-${this.thisId}-${container.children.length - 1}`;
        element.className = "added-component";

        element.addEventListener('click', () => {
            if (container.contains(element)) {
                container.removeChild(element);
            }
        });

        container.appendChild(element);
    }
    
    addAlteration(event) {
        this.togglePopupOn(this.alterationPopup, event);
        Elements.disableItems(this.allComponents);
    }
    
    addSuspension(event) {
        this.togglePopupOn(this.suspensionPopup, event);
        Elements.disableItems(this.allComponents);
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
        Elements.enableItems(this.allComponents);
    }
    
    addComponent(popup) {
        this.togglePopupOff(popup);
    }
    
    static disableItems(items) {
        items.forEach(item => {
            Elements.disableElement(item);
        });
    }
    
    static enableItems(items) {
        items.forEach(item => {
            Elements.enableElement(item);
        });
    }
    
    static enableElement(button) { button.disabled = false; }
    static disableElement(button) { button.disabled = true; }
    
    resetAll() {
        Elements.enableItems(this.allComponents);
    
        [this.positionDropdown, this.symbolDropdown, this.removedDropdown, this.rootDropdown].forEach(component => {
            component.value = ' ';
        });
    
        this.addedPopup.style.display = "none";
        this.suspensionPopup.style.display = "none";
        this.alterationPopup.style.display = "none";
        
    }
}