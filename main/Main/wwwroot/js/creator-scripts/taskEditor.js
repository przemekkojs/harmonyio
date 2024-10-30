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
        this.barElement.insertBefore(this.component, this.barElement.children[this.functionIndex + 1]);

        this.element = new Elements(this.id, task);
        allElements.push(this.element);

        this.remover = component.querySelector(`#cancel-creator-${this.id}`);
        this.remover.addEventListener('click', () => this.handleRemoveClick());

        this.resetter = component.querySelector(`#reset-creator-${this.id}`);
        this.resetter.addEventListener('click', () => this.handleResetClick());
    }

    handleRemoveClick = () => {
        this.bar.removeFunction(this.id);
    };

    handleResetClick = () => {
        this.bar.resetFunction(this.id);
    };
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

        this.addFunctionButton.addEventListener('click', () => {
            this.addFunction();
        });

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
        const elementIndex = this.functions.indexOf(element);
        this.functions.splice(elementIndex, 1);

        this.functions.forEach(f => {
            if (f.functionIndex > element.functionIndex) {
                const oldId = f.id;
                const newId = `x${f.taskId}-${f.barIndex}-${f.functionIndex - 1}`;

                f.remover = f.component.querySelector(`#cancel-creator-${oldId}`);
                f.remover.removeEventListener('click', f.handleRemoveClick)
                f.remover.addEventListener('click', () => {
                    f.bar.removeFunction(newId);
                });

                f.resetter = f.component.querySelector(`#reset-creator-${oldId}`);
                f.resetter.removeEventListener('click', f.handleResetClick)
                f.resetter.addEventListener('click', () => {
                    f.bar.resetFunction(newId);
                });

                f.functionIndex--;
                f.id = newId;
                f.component.id = newId;
                // f.component.innerHTML = createComponent(newId); // TODO: update ID's of inner components
                f.element.setId(newId);
            }
        });

        this.bar.removeChild(element.component);
    }

    removeAll() {
        this.functions = [];
        this.bar.replaceChildren();
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
        this.adder.addEventListener('click', () => {        
            this.addBar(this.bars.length);        
        });

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

            for (let i = barIndex; i < this.bars.length; i++) {
                this.bars[i].shift(i);
            }

            removeBarButton.addEventListener('click', () => {
                this.removeBar(barIndex);
            });

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

    updateBars(startIndex) {
        for (let i = startIndex; i < this.bars.length; i++) {
            let bar = this.bars[i];
            bar.index = i;
            
            let barElement = document.querySelector(`#bar-${this.id}-${i + 1}`);
            barElement.id = `bar-${this.id}-${i}`;

            let span = barElement.querySelector('span');
            span.innerText = i + 1;

            let addFunctionButton = barElement.querySelector(`#add-function-${this.id}-${i + 1}`);
            addFunctionButton.id = `add-function-${this.id}-${i}`;
            let addFunctionButtonClone = addFunctionButton.cloneNode(true);            
            addFunctionButton.parentNode.replaceChild(addFunctionButtonClone, addFunctionButton);            
            addFunctionButtonClone.addEventListener('click', () => bar.addFunction());
            
            let deleteBarButton = barElement.querySelector(`#delete-bar-${this.id}-${i + 1}`);
            deleteBarButton.id = `delete-bar-${this.id}-${i}`;
            let deleteBarButtonClone = deleteBarButton.cloneNode(true);
            deleteBarButton.parentNode.replaceChild(deleteBarButtonClone, deleteBarButton); 
            deleteBarButtonClone.addEventListener('click', () => this.removeBar(i));
        }
    }

    confirmFunction(element) {
        if (element.confirmed)
            return true;

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
}