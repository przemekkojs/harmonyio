import { createComponent } from "./createComponentCreator.js";
import { Elements } from "./addFunctionality.js";

let allElements = [];

class Function {
    constructor(bar, functionIndex) {
        this.functionIndex = functionIndex;
        this.barIndex = bar.index;
        this.taskId = bar.taskId;
        this.bar = bar;
        this.barElement = document.getElementById(`bar-${this.taskId}-${this.barIndex}`);

        let component = document.createElement('div');
        let newId = `x${this.taskId}-${this.barIndex}-${this.functionIndex}`;
        component.id = newId;
        component.className = 'task-box';
        component.innerHTML = createComponent(newId);

        this.component = component;        
        this.barElement.insertBefore(this.component, this.barElement.children[this.functionIndex + 1]);

        allElements.push(new Elements(newId));
        let remover = component.querySelector(`#cancel-creator-${newId}`);

        remover.addEventListener('click', () => {
            this.bar.removeFunction()
        });
    }
}

class Bar {
    constructor(taskId, index, maxFunctionsCount) {
        this.index = index;
        this.taskId = taskId;
        this.functions = [];
        this.maxFunctionsCount = maxFunctionsCount;
        this.bar = document.getElementById(`bar-${taskId}-${index}`);

        this.addFunctionButton = document.createElement('button');
        this.addFunctionButton.id = `add-function-${this.taskId}-${this.index}`;
        this.addFunctionButton.innerText = "...";
        this.addFunctionButton.className = "adder-button";
        this.addFunctionButton.title = "Dodaj funkcję";

        this.addFunctionButton.addEventListener('click', () => {
            this.addFunction();
        });

        this.bar.appendChild(this.addFunctionButton);
    }

    addFunction() {
        if (this.functions.length < this.maxFunctionsCount) {
            this.functions.push(new Function(this, this.functions.length));
        }            
    }

    removeFunction() {
        if (this.functions.length > 0) {
            let element = this.functions.pop();
            this.bar.removeChild(element.component);
        }        
    }

    removeAll() {
        while (this.functions.length > 0) {
            this.removeFunction();
        }
    }
}

export class Task {
    constructor(id, limit=16) {
        this.bars = [];
        this.id = id;
        this.limit = limit;

        this.componentsPlace = document.getElementById(`components-place-${id}`);

        this.adder = document.createElement('button');
        this.adder.id = `add-bar-${this.id}`;
        this.adder.innerText = '...';
        this.adder.className = "adder-button";
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
                <form>
                    <input type="button" value="x" id="delete-bar-${this.id}-${barIndex}" style="background-color: #ff5f3c; border: none; text-align: center; border-radius: 2px;">
                </form>
            </div>`;

            newBar.className = 'bars';

            let removeBarButton = newBar
                .querySelector('form')
                .querySelector('input');

            for (let i = barIndex; i < this.bars.length; i++) {
                this.bars[i].shift(i);
            }

            removeBarButton.addEventListener('click', () => {
                this.removeBar(barIndex);
            });

            this.componentsPlace.insertBefore(newBar, this.componentsPlace.children[barIndex]);
            this.bars.splice(barIndex, 0, new Bar(this.id, barIndex, 8));
        }
    }

    removeBar(index) {
        console.log(index + 1);

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
}