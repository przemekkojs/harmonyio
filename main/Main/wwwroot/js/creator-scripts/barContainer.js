import { FunctionContainer } from "./functionContainer.js"

class Bar {
    constructor(barContainer, taskIndex, barIndex, maxFunctionsCount = 8) {
        this.taskIndex = taskIndex;
        this.barIndex = barIndex;
        this.maxFunctionsCount = maxFunctionsCount;
        this.barContainer = barContainer;

        this.functionContainer = new FunctionContainer(this, maxFunctionsCount);
        this.deleteButton = this.functionContainer.deleteButton;

        this.handleRemoveBarClick = this.barContainer.removeBar.bind(this.barContainer, barIndex);
        this.deleteButton.addEventListener('click', this.handleRemoveBarClick);
    }

    setId(taskIndex, barIndex) {
        this.deleteButton.removeEventListener('click', this.handleRemoveBarClick);

        this.taskIndex = taskIndex;
        this.barIndex = barIndex;
        this.functionContainer.setId(taskIndex, barIndex);
        this.deleteButton.id = `delete-bar-${this.taskIndex}-${this.barIndex}`;

        this.handleRemoveBarClick = this.barContainer.removeBar.bind(this.barContainer, barIndex);
        this.deleteButton.addEventListener('click', this.handleRemoveBarClick);
    }
}

export class BarContainer {
    constructor(taskContainer, maxBars) {
        this.bars = [];
        this.taskIndex = taskContainer.taskIndex;
        this.taskContainer = taskContainer;
        this.maxBars = maxBars;

        this.container = document.createElement('div');
        this.container.className = "components-place";
        this.container.id = `components-place-${this.taskIndex}`;

        this.addBarButton = document.createElement('input');
        this.addBarButton.type = 'button';
        this.addBarButton.id = `add-bar-${this.taskIndex}`;
        this.addBarButton.className = "adder-button custom-button button-large plus-button";
        this.addBarButton.title = "Dodaj takt";

        this.handleAddBar = this.addBar.bind(this);
        this.addBarButton.addEventListener('click', this.handleAddBar);
                
        this.container.appendChild(this.addBarButton);
        this.addBar();
    }

    addBar() {
        const barsLength = this.bars.length;

        console.log(this.container);
        
        if (barsLength < this.maxBars) {
            const newBar = new Bar(this, this.taskIndex, barsLength);
            this.bars.push(newBar);

            this.container.insertBefore(newBar.functionContainer.container, this.addBarButton);
        }      

        // If more bars than one were added, show delete button.
        const bar = this.bars[0];
        const deleteButton = bar.functionContainer.deleteButton;

        if (this.bars.length == 1)
            deleteButton.className = "button-medium custom-button empty-button";
        else
            deleteButton.className = "button-medium custom-button trash-button";     
    }

    removeBar(barIndex) {
        const barsLength = this.bars.length;

        // Disable deleting only one bar
        if (barsLength == 1) {
            return;
        }            

        if (barIndex >= barsLength)
            return;

        const currentBar = this.bars[barIndex];

        this.bars.splice(barIndex, 1);
        this.setId(this.taskIndex, barIndex);        
        this.container.removeChild(currentBar.functionContainer.container);

        if (this.bars.length == 1) {
            const bar = this.bars[0];
            const deleteButton = bar.functionContainer.deleteButton;
            deleteButton.className = "button-medium custom-button empty-button";
        }
    }

    setId(taskIndex, startBarIndex = 0) {
        const barsLength = this.bars.length;

        this.addBarButton.removeEventListener('click', this.handleAddBar);
        this.taskIndex = taskIndex;

        this.container.id = `components-place-${this.taskIndex}`;        
        this.addBarButton.id = `add-bar-${this.taskIndex}`;

        for (let index = startBarIndex; index < barsLength; index++) {
            const toChange = this.bars[index];            
            toChange.setId(taskIndex, index);
        }

        this.handleAddBar = this.addBar.bind(this);
        this.addBarButton.addEventListener('click', this.handleAddBar);
    }

    removeAll() {
        this.bars = [];
        this.container.replaceChildren();
        this.container.appendChild(this.addBarButton);
    }
}
