// class Function {
//     constructor() {
//         this.isMain = false;
//         this.minor = false;
//         this.root = "";
//         this.position = "";
//         this.symbol = "";
//         this.removed = "";
//         this.added = [];
//         this.alterations = []
//         this.barIndex = 0;
//         this.correspondingStackIndex = 0;
//     }
// }

function load(thisId) {
    let thisDiv = document.getElementById(thisId);
    
    let positionDropdown = thisDiv.getElementById('position');
    let rootDropdown = thisDiv.getElementById('root');
    let symbolDropdown = thisDiv.getElementById('symbol');
    let removedDropdown = thisDiv.getElementById('removed');

    let addedButton = thisDiv.getElementById('add-added');
    let alterationButton = thisDiv.getElementById('add-alteration');
    let suspensionButton = thisDiv.getElementById('add-suspension');
    let resetButton = thisDiv.getElementById('reset-creator');

    let addedPopup = thisDiv.getElementById('added-popup');
    let suspensionPopup = thisDiv.getElementById('suspension-popup');
    let alterationPopup = thisDiv.getElementById('alteration-popup');

    let avaiableAlterations = ['<', '>'];    
    let minor = 'm';

    let allComponents = [minor, positionDropdown, rootDropdown,
                        symbolDropdown, removedDropdown, addedButton,
                        alterationButton, suspensionButton];

    let objectFunction = new Function();

    function populateDropdowns() {
        addEmpty();
        populatePosition();
        populateSymbols();
        populateRoot();
        populateRemoved();
    }

    function addEmpty() {
        [positionDropdown, rootDropdown, symbolDropdown, removedDropdown].forEach(element => {
            element.appendChild(document.createElement('option'));
        });
    }

    let populatePosition = () => populateDropdown(positionDropdown, ['1', '3', '5', '6', '7', '9']);
    let populateRoot = () => populateDropdown(rootDropdown, ['1', '3', '5', '7']);
    let populateSymbols = () => populateDropdown(symbolDropdown, ['T', 'Sii', 'Tiii', 'Diii', 'S', 'D', 'Tvi', 'Svi', 'Dvii', 'Svii']);
    let populateRemoved = () => populateDropdown(removedDropdown, ['1', '5']);

    function populateDropdown(dropdown, components) {
        components.forEach(element => {
            let opt = document.createElement('option');
            opt.value = element;
            opt.innerText = element;
            dropdown.appendChild(opt);
        });
    }

    function addOnClickEvents()
    {
        addedButton.addEventListener('click', function(e) { addAdded(event); });
        suspensionButton.addEventListener('click', function(e) { addSuspension(event); });
        alterationButton.addEventListener('click', function(e) { addAlteration(event); });
        resetButton.addEventListener('click', function(e) { resetAll(); });
    }

    function addAdded(event) {
        disableItems(allComponents);
        togglePopupOn(addedPopup, event);
    }

    function addAlteration(event) {
        disableItems(allComponents);
        togglePopupOn(alterationPopup, event);
    }

    function addSuspension(event) {
        disableItems(allComponents);
        togglePopupOn(suspensionPopup, event);
    }

    function togglePopupOn(popup, event) {
        let button = event.currentTarget;
        let rect = button.getBoundingClientRect();

        popup.style.display = "block";
        popup.style.left = (rect.left - (popup.style.width / 2)) + "px";
        popup.style.top = (rect.top + button.offsetHeight) + "px";
        
        let confirmButton = popup.querySelectorAll('input[type="button"]')[0];
        let cancelButton = popup.querySelectorAll('input[type="button"]')[1];       

        confirmButton.addEventListener('click', function(e) { addComponent(popup); });
        cancelButton.addEventListener('click', function(e) { togglePopupOff(popup); });
    }

    function togglePopupOff(popup) {
        popup.style.display = "none";
        enableItems(allComponents);
    }

    function addComponent(popup) {
        togglePopupOff(popup);
    }

    function disableItems(items) {
        items.forEach(item => {
            disableElement(item);
        });
    }

    function enableItems(items) {
        items.forEach(item => {
            enableElement(item);
        });
    }

    let enableElement = (button) => button.disabled = false;
    let disableElement = (button) => button.disabled = true;

    function resetAll() {
        enableItems(allComponents);

        [positionDropdown, symbolDropdown, removedDropdown, rootDropdown].forEach(component => {
            component.value = ' ';
        });

        addedPopup.style.display = "none";
        suspensionPopup.style.display = "none";
        alterationPopup.style.display = "none";
    }

    populateDropdowns();
    addOnClickEvents();
}
