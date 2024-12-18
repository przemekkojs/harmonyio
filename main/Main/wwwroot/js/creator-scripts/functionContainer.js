﻿import { Function } from "./functionCreator.js"

export class FunctionContainer {
    constructor(bar, maxFunctionsCount) {
        this.maxFunctionsCount = maxFunctionsCount;
        this.bar = bar;
        this.taskIndex = bar.taskIndex;
        this.barIndex = bar.barIndex;
        this.functions = [];

        this.addFunctionButton = document.createElement('input');
        this.addFunctionButton.type = 'button';
        this.addFunctionButton.id = `add-function-${this.taskIndex}-${this.barIndex}`;
        this.addFunctionButton.className = "adder-button custom-button button-medium plus-button";
        this.addFunctionButton.title = "Dodaj funkcję";        

        this.handleAddFunction = this.addFunction.bind(this);
        this.addFunctionButton.addEventListener('click', this.handleAddFunction);

        this.barNumber = document.createElement('span');
        this.barNumber.style = "margin-bottom: 5px; font-style: italic;";
        this.barNumber.className = "text-nowrap";
        this.barNumber.innerText = `${this.barIndex + 1}`;

        this.container = document.createElement('div');
        this.container.id = `bar-${this.taskIndex}-${this.barIndex}`;
        this.container.className = "bars";

        this.deleteButton = document.createElement('input');
        this.deleteButton.className = "button-medium custom-button trash-button";
        this.deleteButton.title = `Usuń takt ${this.barIndex + 1}`;
        this.deleteButton.id = `delete-bar-${this.taskIndex}-${this.barIndex}`;

        this.barInfo = `
            <svg heigth="135" width="10">
                <line x1="5" y1="0" x2="5" y2="140" style="stroke:black;stroke-width:1px;"/>
            </svg>`;

        this.barInfoDiv = document.createElement('div');
        this.barInfoDiv.innerHTML = this.barInfo;

        this.barInfoPlaceholder = document.createElement('div');
        this.barInfoPlaceholder.style.display = 'flex';
        this.barInfoPlaceholder.style.flexDirection = 'column';
        this.barInfoPlaceholder.style.alignItems = 'center';

        this.barInfoPlaceholder.appendChild(this.barNumber);
        this.barInfoPlaceholder.appendChild(this.barInfoDiv);
        this.barInfoPlaceholder.appendChild(this.deleteButton);

        this.container.appendChild(this.barInfoPlaceholder);
        this.container.appendChild(this.addFunctionButton);
    }

    // TODO:
    trim() {
        const functionCount = this.functions.length;

        while (functionCount > this.maxFunctionsCount) {
            const functionIndex = this.functions.length - 1;

            const currentFunction = this.functions[functionIndex];
            this.functions.splice(functionIndex, 1);
            this.container.removeChild(currentFunction.functionCreator.container);
        }
    }

    addFunction() {
        const functionCount = this.functions.length;

        if (functionCount < this.maxFunctionsCount) {
            const newFunction = new Function(this, functionCount);

            this.functions.push(newFunction);
            this.container.insertBefore(newFunction.functionCreator.container, this.addFunctionButton);

            // Fix automatycznego przesuwania zawsze
            if (this.barIndex == this.bar.barContainer.bars.length - 1) {
                this.bar.barContainer.container.scrollTo({
                    left: this.bar.barContainer.container.scrollWidth,
                    behavior: 'smooth'
                });
            }            
        }        
    }

    removeFunction(functionIndex) {
        const functionCount = this.functions.length;

        if (functionIndex >= functionCount)
            return;
        
        const currentFunction = this.functions[functionIndex];

        this.functions.splice(functionIndex, 1);
        this.setId(this.taskIndex, this.barIndex, functionIndex);
        this.container.removeChild(currentFunction.functionCreator.container);
    }

    setId(taskIndex, barIndex, startFunctionIndex = 0) {
        this.taskIndex = taskIndex;
        this.barIndex = barIndex;
        this.addFunctionButton.removeEventListener('click', this.handleAddFunction);

        const functionCount = this.functions.length;

        for (let index = startFunctionIndex; index < functionCount; index++) {
            const toChange = this.functions[index];
            toChange.setId(taskIndex, barIndex, index);
        }

        this.addFunctionButton.id = `add-function-${this.taskIndex}-${this.barIndex}`;
        
        this.container.id = `bar-${this.taskIndex}-${this.barIndex}`;
        this.barNumber.innerText = `${barIndex + 1}`;

        this.handleAddFunction = this.addFunction.bind(this);
        this.addFunctionButton.addEventListener('click', this.handleAddFunction);
    }

    removeAll() {
        this.functions = [];
        this.container.replaceChildren();
    }
}