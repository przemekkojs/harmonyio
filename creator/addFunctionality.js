export class Elements {
    constructor(thisId) {
        this.thisDiv = document.getElementById(thisId);
    
        this.positionDropdown = this.thisDiv.querySelector(`#position-${thisId}`);
        this.rootDropdown = this.thisDiv.querySelector(`#root-${thisId}`);
        this.symbolDropdown = this.thisDiv.querySelector(`#symbol-${thisId}`);
        this.removedDropdown = this.thisDiv.querySelector(`#removed-${thisId}`);

        this.addedButton = this.thisDiv.querySelector(`#add-added-${thisId}`);
        this.alterationButton = this.thisDiv.querySelector(`#add-alteration-${thisId}`);
        this.suspensionButton = this.thisDiv.querySelector(`#add-suspension-${thisId}`);
        this.resetButton = this.thisDiv.querySelector(`#reset-creator-${thisId}`);

        this.addedPopup = this.thisDiv.querySelector(`#added-popup-${thisId}`);
        this.suspensionPopup = this.thisDiv.querySelector(`#suspension-popup-${thisId}`);
        this.alterationPopup = this.thisDiv.querySelector(`#alteration-popup-${thisId}`);

        this.avaiableAlterations = ['up', 'down'];    
        this.minor = 'm';

        this.allComponents = [this.positionDropdown, this.rootDropdown,
            this.symbolDropdown, this.removedDropdown, this.addedButton,
            this.alterationButton, this.suspensionButton];

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
        this.suspensionButton.addEventListener('click', (event) => { this.addSuspension(event); });
        this.alterationButton.addEventListener('click', (event) => { this.addAlteration(event); });
        this.resetButton.addEventListener('click', () => { this.resetAll(); });
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
    
    togglePopupOn(popup, event) {
        const button = event.currentTarget;
        const rect = button.getBoundingClientRect();
    
        popup.style.display = "block";
        popup.style.left = (rect.left - (popup.style.width / 2)) + "px";
        popup.style.top = (rect.top + button.offsetHeight) + "px";
        
        const confirmButton = popup.querySelectorAll('input[type="button"]')[0];
        const cancelButton = popup.querySelectorAll('input[type="button"]')[1];       
    
        confirmButton.addEventListener('click', () => { this.addComponent(popup); });
        cancelButton.addEventListener('click', () =>{ this.togglePopupOff(popup); });
    }
    
    togglePopupOff(popup) {
        popup.style.display = "none";
        this.enableItems(this.allComponents);
    }
    
    addComponent(popup) {
        this.togglePopupOff(popup);
    }
    
    disableItems(items) {
        items.forEach(item => {
            this.disableElement(item);
        });
    }
    
    enableItems(items) {
        items.forEach(item => {
            this.enableElement(item);
        });
    }
    
    enableElement(button) { button.disabled = false; }
    disableElement(button) { button.disabled = true; }
    
    resetAll() {
        this.enableItems(this.allComponents);
    
        [this.positionDropdown, this.symbolDropdown, this.removedDropdown, this.rootDropdown].forEach(component => {
            component.value = ' ';
        });
    
        this.addedPopup.style.display = "none";
        this.suspensionPopup.style.display = "none";
        this.alterationPopup.style.display = "none";
    }
}