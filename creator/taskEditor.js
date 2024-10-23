import { createComponent } from "./createComponentCreator.js";
import { Elements } from "./addFunctionality.js";

let allElements = [];

class Bar {
    constructor(taskId, index, maxFunctionsCount) {
        this.index = index;
        this.taskId = taskId;
        this.functions = 0;
        this.maxFunctionsCount = maxFunctionsCount;
        this.bar = document.getElementById(`bar-${taskId}-${index}`);

        this.addFunctionButton = document.createElement('button');
        this.addFunctionButton.id = `add-function-${this.taskId}-${this.index}`;
        this.addFunctionButton.innerText = "...";
        this.addFunctionButton.className = "adder-button";
        this.addFunctionButton.addEventListener('click', () => {
            this.addFunction(this.functions);
        });

        this.bar.appendChild(this.addFunctionButton);
    }

    addFunction() {
        if (this.functions < this.maxFunctionsCount) {
            this.functions++;

            let component = document.createElement('div');
            let newId = `x${this.taskId}-${this.index}-${this.functions}`;
            component.id = newId;
            component.className = 'task-box';
            component.innerHTML = createComponent(this.taskId, this.index, this.functions);

            let remover = component.querySelector(`#cancel-creator-${newId}`);

            remover.addEventListener('click', () => {
                this.removeFunction()
            });
            
            this.bar.insertBefore(component, this.bar.children[this.functions]);
            allElements.push(new Elements(`x${this.taskId}-${this.index}-${this.functions}`));
        }            
    }

    removeFunction() {
        if (this.functions > 0) {
            let element = null;

            const newId = `#x${this.taskId}-${this.index}-${this.functions}`;

            allElements.forEach(e => {
                element = this.bar.querySelector(newId);
            });

            this.functions--;
            this.bar.removeChild(element);
        }        
    }

    shift(newIndex) {
        this.index = newIndex;
        this.refreshButton();
    }
}

export class Task {
    constructor(id, limit=16) {
        this.bars = []; //TODO: Change limit
        this.id = id;
        this.limit = limit;

        this.componentsPlace = document.getElementById(`components-place-${id}`);

        this.adder = document.createElement('button');
        this.adder.id = `add-bar-${this.id}`;
        this.adder.innerText = '...';
        this.adder.className = "adder-button";
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
                <span style="margin-bottom: 5px; font-style: italic;">${barIndex}</span>
                <svg height="150" width="10">
                    <line x1="5" y1="0" x2="5" y2="150" style="stroke:black;stroke-width:1" />
                </svg>
            </div>`;

            newBar.className = 'bars';

            for (let i = barIndex; i < this.bars.length; i++) {
                this.bars[i].shift(i);
            }

            this.componentsPlace.insertBefore(newBar, this.componentsPlace.children[barIndex]);
            this.bars.splice(barIndex, 0, new Bar(this.id, barIndex, 8));
        }
    }
}