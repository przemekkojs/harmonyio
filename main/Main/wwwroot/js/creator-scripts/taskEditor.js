import { createComponent } from "./createComponentCreator.js";
import { Elements } from "./addFunctionality.js";

let allElements = [];

class Function {
    constructor(task, bar, functionIndex) {
        this.functionIndex = functionIndex;
        this.barIndex = bar.index;
        this.taskId = bar.taskId;
        this.bar = bar;
        this.id = `x${this.taskId}-${this.barIndex}-${this.functionIndex}`;
        this.barElement = document.getElementById(`bar-${this.taskId}-${this.barIndex}`);

        this.component = createComponent(this.id);
        this.barElement.insertBefore(this.component.component, this.barElement.children[this.functionIndex + 1]);

        this.element = new Elements(this.id, task);
        allElements.push(this.element);

        this.remover = this.component.cancelCreator;
        this.resetter = this.component.resetCreator;

        this.handleRemoveClick = this.handleRemoveClick.bind(this);
        this.handleResetClick = this.handleResetClick.bind(this);

        this.addListeners();
    }

    handleRemoveClick() {
        this.bar.removeFunction(this.id);
    };

    handleResetClick() {
        this.bar.resetFunction(this.id);
    };

    removeListeners() {
        this.remover.removeEventListener('click', this.handleRemoveClick);
        this.resetter.removeEventListener('click', this.handleResetClick);
    }

    addListeners() {
        this.remover.addEventListener('click', this.handleRemoveClick);
        this.resetter.addEventListener('click', this.handleResetClick);
    }
}

class Bar {
    constructor(task, index, maxFunctionsCount) {
        this.index = index;
        this.taskId = task.id;
        this.task = task;
        this.functions = [];
        this.maxFunctionsCount = maxFunctionsCount;
        this.bar = document.getElementById(`bar-${this.taskId}-${this.index}`);

        this.addFunctionButton = document.createElement('input');
        this.addFunctionButton.type = 'button';
        this.addFunctionButton.id = `add-function-${this.taskId}-${this.index}`;
        this.addFunctionButton.className = "adder-button custom-button button-medium plus-button";
        this.addFunctionButton.title = "Dodaj funkcję";

        this.addFunctionButton.addEventListener('click', () => this.addFunction());
        this.bar.appendChild(this.addFunctionButton);
    }

    addFunction() {
        if (this.functions.length < this.maxFunctionsCount) {
            this.functions.push(new Function(this.task, this, this.functions.length));
        }            
    }

    resetFunction(id) {
        const element = this.functions.filter(f => f.id == id)[0];
        const elementIndex = this.functions.indexOf(element);
        this.functions.splice(elementIndex, 1);
    }

    removeFunction(id) {
        const element = this.functions.filter(f => f.id == id)[0];

        if (!element)
            return;

        const elementIndex = this.functions.indexOf(element);

        this.functions.forEach(f => {
            if (f.functionIndex > element.functionIndex) {
                const newId = `x${f.taskId}-${f.barIndex}-${f.functionIndex - 1}`;

                f.removeListeners();

                f.functionIndex--;
                f.id = newId;
                f.component.setIds(newId);
                f.element.setId(newId);

                f.addListeners();
            }
        });

        this.bar.removeChild(element.component.component);
        this.functions.splice(elementIndex, 1);
    }

    removeAll() {
        this.functions = [];
        this.bar.replaceChildren();
    }
}

class BarContainer {
    constructor(bar) {
        this.bar = bar;

        const barIndex = bar.index;
        const taskIndex = bar.taskId;

        this.container = document.createElement('div');
        this.container.id = `bar-${taskIndex}-${barIndex}`;
        this.container.className = "bars";

        this.barNumber = document.createElement('span');
        this.barNumber.style = "margin-bottom: 5px; font-style: italic;";
        this.barNumber.innerText = `${barIndex + 1}`;

        this.deleteButton = document.createElement('input');
        this.deleteButton.className = "button-medium custom-button trash-button";
        this.deleteButton.title = `Usuń takt ${barIndex + 1}`;
        this.deleteButton.id = `delete-bar-${taskIndex}-${barIndex}`;

        this.container.appendChild(this.barNumber);
        this.container.innerHTML += `<svg height="120" width="10">
                <line x1="5" y1="0" x2="5" y2="110" style="stroke:black;stroke-width:1"/>
            </svg >`;

        this.container.appendChild(this.deleteButton);

        this.deleteButton.addEventListener('click', () => bar.task.removeBar(barIndex));
    }
}

export class Task {
    constructor(id, limit=16) {
        this.bars = [];
        this.id = id;
        this.limit = limit;
        this.result = [];

        this.componentsPlace = document.getElementById(`components-place-${id}`);

        this.adder = document.createElement('input');
        this.adder.type = 'button';
        this.adder.id = `add-bar-${this.id}`;
        this.adder.className = "adder-button custom-button button-large plus-button";
        this.adder.title = "Dodaj takt";
        this.adder.addEventListener('click', () => this.addBar(this.bars.length));

        this.addBar(0);

        this.componentsPlace.appendChild(this.adder);
    }

    addBar(barIndex) {
        if (this.bars.length < this.limit){
            let newBar = document.createElement('div');
            newBar.id = `bar-${this.id}-${barIndex}`;
            newBar.innerHTML = `
            <div style="display: flex; flex-direction: column; align-items: center;">
                <span style="margin-bottom: 5px; font-style: italic;">${barIndex + 1}</span>
                <svg height="120" width="10">
                    <line x1="5" y1="0" x2="5" y2="110" style="stroke:black;stroke-width:1" />
                </svg>
                <input type="button" id="delete-bar-${this.id}-${barIndex}" class="button-medium custom-button trash-button" title="Usuń takt ${barIndex + 1}">
            </div>`;

            newBar.className = 'bars';

            let removeBarButton = newBar
                .querySelector('input');

            removeBarButton.addEventListener('click', () => this.removeBar(barIndex));

            this.componentsPlace.insertBefore(newBar, this.componentsPlace.children[barIndex]);
            this.bars.splice(barIndex, 0, new Bar(this, barIndex, 8));
        }
    }

    removeBar(index) {
        let element = document.querySelector(`#bar-${this.id}-${index}`);
        let bar = this.bars[index];

        bar.removeAll();
        this.bars.splice(index, 1);
        this.componentsPlace.removeChild(element);

        this.updateBars(index);
    }

    // Jeżeli parametr newId jest podany, to trzeba potem manualnie ustawić task.id!!!!!
    updateBars(startIndex, newId=this.id) {
        for (let i = startIndex; i < this.bars.length; i++) {
            const bar = this.bars[i];
            bar.index = i;
            
            const barElement = document.querySelector(`#bar-${this.id}-${i + 1}`);
            barElement.id = `bar-${newId}-${i}`;

            const span = barElement.querySelector('span');
            span.innerText = i + 1;

            const addFunctionButton = barElement.querySelector(`#add-function-${this.id}-${i + 1}`);
            addFunctionButton.id = `add-function-${newId}-${i}`;

            const addFunctionButtonClone = addFunctionButton.cloneNode(true);            
            addFunctionButton.parentNode.replaceChild(addFunctionButtonClone, addFunctionButton);            
            addFunctionButtonClone.addEventListener('click', () => bar.addFunction());
            
            const deleteBarButton = barElement.querySelector(`#delete-bar-${this.id}-${i + 1}`);
            deleteBarButton.id = `delete-bar-${newId}-${i}`;

            const deleteBarButtonClone = deleteBarButton.cloneNode(true);
            deleteBarButton.parentNode.replaceChild(deleteBarButtonClone, deleteBarButton); 
            deleteBarButtonClone.addEventListener('click', () => this.removeBar(i));
        }
    }

    confirmFunction(element) {
        const confirmResult = element.confirm();    

        if (confirmResult !== null) {
            this.result.push(confirmResult);
            return true;
        }
        else {
            return false;
        }   
    }

    confirmAll() {
        this.result = [];

        this.bars.forEach(b => {
            b.functions.forEach(f => {
                if (f.taskId === this.id) {
                    const currentResult = this.confirmFunction(f.element);

                    if (!currentResult)
                        return false;
                }
            });
        });

        return true;
    }

    load(task) {        
        task.forEach(parsedFunction => {
            const barIndex = parsedFunction.barIndex;

            if (this.bars.length - 1 < barIndex)
                this.addBar(barIndex);

            const bar = this.bars[barIndex];
            bar.addFunction();
            const newFunction = bar.functions[bar.functions.length - 1];

            if (parsedFunction.minor)
                newFunction.component.minor.checked = true;

            if (parsedFunction.symbol != null)
                newFunction.component.symbol.value = parsedFunction.symbol;

            if (parsedFunction.root != null)
                newFunction.component.root.value = parsedFunction.root;

            if (parsedFunction.position != null)
                newFunction.component.position.value = parsedFunction.position;

            if (parsedFunction.removed != null)
                newFunction.component.removed.value = parsedFunction.removed;

            parsedFunction.added.forEach(a => {
                const component = a[0];
                const option = a.length == 1 ? "-" : a[1];

                newFunction.element.createAddedElement(component, option);
            });

            // TODO: Alteracje
        });
    }
}