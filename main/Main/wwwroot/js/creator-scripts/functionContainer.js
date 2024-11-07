﻿import { Function } from "./functionCreator.js"

export class FunctionContainer {
    constructor(bar, maxFunctionsCount) {
        //console.log("FunctionContainer");

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
        this.barNumber.innerText = `${this.barIndex + 1}`;

        this.container = document.createElement('div');
        this.container.id = `bar-${this.taskIndex}-${this.barIndex}`;
        this.container.className = "bars";

        this.deleteButton = document.createElement('input');
        this.deleteButton.className = "button-medium custom-button trash-button";
        this.deleteButton.title = `Usuń takt ${this.barIndex + 1}`;
        this.deleteButton.id = `delete-bar-${this.taskIndex}-${this.barIndex}`;

        this.barInfo = `<svg heigth="120" width="10"><line x1="5" y1="0" x2="5" y2="110" style="stroke:black;stroke-width:1px;"/></svg>`

        this.barInfoPlaceholder = document.createElement('div');
        this.barInfoPlaceholder.style.display = 'flex';
        this.barInfoPlaceholder.style.flexDirection = 'column';
        this.barInfoPlaceholder.style.alignItems = 'center';

        this.barInfoPlaceholder.appendChild(this.barNumber);
        this.barInfoPlaceholder.innerHTML += this.barInfo;
        this.barInfoPlaceholder.appendChild(this.deleteButton);

        this.container.appendChild(this.barInfoPlaceholder);
        this.container.appendChild(this.addFunctionButton);

        this.addFunction();
    }

    addFunction() {
        //console.log("FunctionContainer.addFunction()");
        const functionCount = this.functions.length;

        if (functionCount < this.maxFunctionsCount) {
            const newFunction = new Function(this, functionCount);

            this.functions.push(newFunction);
            this.container.insertBefore(newFunction.functionCreator.container, this.addFunctionButton);
        }        
    }

    removeFunction(functionIndex) {
        const functionCount = this.functions.length;

        if (functionIndex >= functionCount)
            return;
        
        const currentFunction = this.functions[functionIndex];
        //console.log("FunctionContainer: removeFunction(): functionIndex", functionIndex);
        //console.log("FunctionContainer: removeFunction(): currentFunction", currentFunction);

        this.functions.splice(functionIndex, 1);
        this.setId(this.taskIndex, this.barIndex, functionIndex);

        //console.log("FunctionContainer: removeFunction(): container", this.container);
        //console.log("FunctionContainer: removeFunction(): currentFunction.functionCreator.container", currentFunction.functionCreator.container);
        this.container.removeChild(currentFunction.functionCreator.container);
    }

    setId(taskIndex, barIndex, startFunctionIndex=0) {
        this.addFunctionButton.removeEventListener('click', this.handleAddFunction);
        this.taskIndex = taskIndex;
        this.barIndex = barIndex;

        const functionCount = this.functions.length;

        for (let index = startFunctionIndex; index < functionCount; index++) {
            const toChange = this.functions[index];
            toChange.setId(taskIndex, barIndex, index);
        }

        this.addFunctionButton.id = `add-function-${this.taskIndex}-${this.barIndex}`;
        this.container.id = `bar-${this.taskIndex}-${this.barIndex}`;

        this.handleAddFunction = this.addFunction.bind(this);
        this.addFunctionButton.addEventListener('click', this.handleAddFunction);
    }

    removeAll() {
        this.functions = [];
        this.container.replaceChildren();
    }
}