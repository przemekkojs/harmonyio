import { BarContainer } from "./barContainer.js";

class Task {
    constructor(taskContainer, taskIndex, maxBars = 16) {     
        //console.log("Task");
        this.taskIndex = taskIndex;
        this.maxBars = maxBars;
        this.taskContainer = taskContainer;

        this.barContainer = new BarContainer(taskContainer, maxBars);
        this.handleRemove = this.taskContainer.removeTask.bind(this.taskContainer, this.taskIndex);

        this.container = document.createElement('div');
        this.container.className = "task-content w-100 p-3";        

        this.questionInput = document.createElement('input');
        this.questionInput.type = "text";
        this.questionInput.class = "form-control task-input";
        this.questionInput.placeholder = "Wpisz polecenie";

        this.hiddenInput = document.createElement('input');
        this.hiddenInput.type = "hidden";        

        this.params = document.createElement('div');
        this.params.className = "params";

        this.metreLabel = document.createElement('label');
        this.metreLabel.innerText = "Metrum: ";

        this.metreSelect = document.createElement('select');
        this.metreSelect.innerHTML = `
            <option value="2/4">2/4</option>
            <option value="3/4">3/4</option>
            <option value="4/4">4/4</option>
            <option value="3/8">3/8</option>
            <option value="6/8">6/8</option>
        `;

        this.tonationLabel = document.createElement('label');
        this.tonationLabel.innerText = "Tonacja: ";

        this.tonationNameSelect = document.createElement('select');
        this.tonationNameSelect.className = "smaller-select";
        this.tonationNameSelect.innerHTML = `
            <option value="C">C</option>
            <option value="D">D</option>
            <option value="E">E</option>
            <option value="F">F</option>
            <option value="G">G</option>
            <option value="A">A</option>
            <option value="H">H</option>`;

        this.tonationAccidentalSelect = document.createElement('select');
        this.tonationAccidentalSelect.className = "smaller-select";
        this.tonationAccidentalSelect.innerHTML = `
            <option value=" "> </option>
            <option value="#">#</option>
            <option value="b">b</option>
        `;

        this.tonationModeSelect = document.createElement('select');
        this.tonationModeSelect.innerHTML = `
            <option value="dur">Dur</option>
            <option value="moll">moll</option>
        `;

        this.params.appendChild(this.metreLabel);
        this.params.appendChild(this.metreSelect);
        this.params.appendChild(this.tonationLabel);
        this.params.appendChild(this.tonationNameSelect);
        this.params.appendChild(this.tonationAccidentalSelect);
        this.params.appendChild(this.tonationModeSelect);

        this.taskSubmit = document.createElement('div');
        this.taskSubmit.className = "task-submit";

        this.deleteButton = document.createElement('input');
        this.deleteButton.type = "button";
        this.deleteButton.className = "btn btn-danger btn-sm";
        this.deleteButton.style = "width: 100px;";
        this.deleteButton.value = "Usuń";

        this.taskSubmit.appendChild(this.deleteButton);

        this.container.appendChild(this.hiddenInput);
        this.container.appendChild(this.questionInput);
        this.container.appendChild(this.params);
        this.container.appendChild(this.barContainer.container);
        this.container.appendChild(this.taskSubmit);        

        this.setId(this.taskIndex);
    }

    setId(taskIndex) {
        //console.log("Task: setId(): taskIndex: ", taskIndex);
        this.deleteButton.removeEventListener('click', this.handleRemove);

        this.taskIndex = taskIndex;

        this.metreLabel.for = `metre-select-task-${taskIndex}`;
        this.metreSelect.id = `metre-select-task-${taskIndex}`;

        this.tonationLabel.for = `tonation-name-select-task-${taskIndex}`;
        this.tonationNameSelect.id = `tonation-name-select-task-${taskIndex}`;
        this.tonationAccidentalSelect.id = `accidental-task-${taskIndex}`;
        this.tonationModeSelect.id = `mode-task-${taskIndex}`;

        this.deleteButton.id = `delete-${taskIndex}`;
        this.container.id = `task-container-${taskIndex}`;

        this.questionInput.id = `question-task-${taskIndex}`;
        this.hiddenInput.name = `Questions[${taskIndex}]`;
        this.hiddenInput.id = `question-json-${taskIndex}`;

        this.barContainer.setId(taskIndex);
        this.handleRemove = this.taskContainer.removeTask.bind(this.taskContainer, this.taskIndex);
        this.deleteButton.addEventListener('click', this.handleRemove);
    }

    getResult() {
        const result = [];

        this.barContainer.bars.forEach(b => {
            b.functionContainer.functions.forEach(f => {
                const toAdd = f.functionCreator.confirm();
                result.push(toAdd);
            });
        });

        console.log(result);
        return result;
    }

    // Tutaj wlatuje obiekt zadania
    load(excersiseObject) {
        const sharpsCount = excersiseObject.sharpsCount;
        const flatsCount = excersiseObject.sharpsCount;
        const metreValue = excersiseObject.metreValue;
        const metreCount = excersiseObject.metreCount;
        const taskObject = excersiseObject.task;
        const question = excersiseObject.question;

        this.questionInput.value = question;

        //console.log("TaskContainer: load(): taskObject: ", taskObject); 

        // TODO: Tonacja i metrum

        let index = 0;
        let lastBar = 0;

        taskObject.forEach(parsedFunction => {
            const barIndex = parsedFunction.barIndex;

            if (barIndex != lastBar)
                index = 0;

            let barCount = this.barContainer.bars.length;

            while (barIndex >= barCount) {
                barCount++;
                this.barContainer.addBar();
            }                

            const bar = this.barContainer.bars[barIndex];

            //console.log("TaskContainer: load(): bar: ", bar);

            const functionCount = bar ? bar.functionContainer.functions.length - 1 : 0;

            //console.log(parsedFunction);
            //console.log(index, '/', functionCount);

            if (index >= functionCount) {
                bar.functionContainer.addFunction();
            }

            const functionCountInBar = bar.functionContainer.functions.length;
            const newFunction = bar.functionContainer.functions[functionCountInBar - 1];

            //console.log("TaskContainer: load(): newFunction: ", newFunction);

            if (parsedFunction.minor)
                newFunction.functionCreator.minor.checked = true;

            if (parsedFunction.symbol != null)
                newFunction.functionCreator.symbol.value = parsedFunction.symbol;

            if (parsedFunction.root != null)
                newFunction.functionCreator.root.value = parsedFunction.root;

            if (parsedFunction.position != null)
                newFunction.functionCreator.position.value = parsedFunction.position;

            if (parsedFunction.removed != null)
                newFunction.functionCreator.removed.value = parsedFunction.removed;

            parsedFunction.added.forEach(a => {
                const component = a[0];
                const option = a.length == 1 ? "-" : a[1];

                newFunction.functionCreator.createAddedElement(component, option);
            });

            // TODO: Alteracje (Alterations)
            // TODO: Opóźnienia (Suspensions)

            index++;
        });
    }
}

export class TaskContainer {
    constructor(parent, maxTasksCount = 3) {
        //console.log("TaskContainer");
        this.tasks = [];
        this.maxTasksCount = maxTasksCount;
        this.parent = parent;

        this.addTask();
    }

    addTask() {        
        const taskCount = this.tasks.length;
        //console.log("addTask(): TaskContainer.tasks.length: " + this.tasks.length);

        if (taskCount < this.maxTasksCount) {
            const toAdd = new Task(this, taskCount);
            this.tasks.push(toAdd);
            this.parent.appendChild(toAdd.container);
        }

        //console.log("addTask(): TaskContainer.tasks: " + this.tasks);
    }

    removeTask(taskIndex) {
        //console.log("removeTask(): TaskContainer.tasks: " + this.tasks);
        //console.log("removeTask(): TaskContainer: ");
        //console.log(this);

        const taskCount = this.tasks.length;

        if (taskIndex >= taskCount)
            return;

        const currentTask = this.tasks[taskIndex];

        this.tasks.splice(taskIndex, 1);      
        this.setId(this.taskIndex);
        this.parent.removeChild(currentTask.container);        
    }

    setId(taskIndex = 0) {
        //console.log("TaskContainer: setId(): taskIndex: ", taskIndex); 
        const taskCount = this.tasks.length;

        //console.log("TaskContainer: setId(): TaskContainer.tasks: ", this.tasks); 

        for (let index = taskIndex; index < taskCount; index++) {
            const toChange = this.tasks[index];
            //console.log("TaskContainer: setId(): toChange: ", toChange)

            if (toChange != null)
                toChange.setId(index);
            // else
                //console.log("TaskContainer: setId() null: index: ", index)
        }
    }

    // Ładowanie z całego JSONa
    load(excersises) {
        //console.log("TaskContainer: load(): excersises: ", excersises);

        let index = 0;

        excersises.forEach(excersise => {
            const excersiseObject = JSON.parse(excersise);

            //console.log("TaskContainer: load(): excersiseObject: ", excersiseObject);            
            const tasksCount = this.tasks.length - 1;

            if (index > tasksCount)
                this.addTask();

            const task = this.tasks[index];

            task.load(excersiseObject);

            index++;
        });
    }
}