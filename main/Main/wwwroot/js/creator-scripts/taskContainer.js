import { BarContainer } from "./barContainer.js";
import { parseAccidentalsCountToTonationInfo, parseMetreValuesToMetre, parseTonationToAccidentalsCount } from "../utils.js";

class Task {
    constructor(taskContainer, taskIndex, maxBars = 32) {
        this.taskIndex = taskIndex;
        this.maxBars = maxBars;
        this.taskContainer = taskContainer;

        this.barContainer = new BarContainer(taskContainer, maxBars);
        this.handleRemove = this.taskContainer.removeTask.bind(this.taskContainer, this.taskIndex);

        this.randomTaskButton = document.createElement('input');
        this.randomTaskButton.type = 'button';
        this.randomTaskButton.id = `random-task-${this.taskIndex}`;
        this.randomTaskButton.className = "btn btn-secondary";
        this.randomTaskButton.title = "Wygeneruj losowe zadanie";
        this.randomTaskButton.value = "Losowe zadanie";

        this.randomTaskInput = document.createElement('input');
        this.randomTaskInput.type = 'number';
        this.randomTaskInput.id = `random-bars-${this.taskIndex}`;
        this.randomTaskInput.placeholder = "Takty";

        // Ukryty input
        this.hiddenInput = document.createElement('input');
        this.hiddenInput.type = "hidden";

        // Zadanie
        this.taskNumber = document.createElement('input');
        this.taskNumber.type = "text";
        this.taskNumber.readOnly = true;
        this.taskNumber.className = "form-control task-id";
                
        this.maxPointsInput = document.createElement('input');
        this.maxPointsInput.type = 'number';
        this.maxPointsInput.className = "form-control bg-white border-secondary points-input no-arrows mt-1";
        this.maxPointsInput.style = "width: 100px;";
        this.maxPointsInput.maxLength = "3";
        this.maxPointsInput.placeholder = "Punkty";
        this.maxPointsInput.value = 10;
        this.maxPointsInput.required = true;        

        this.pointsDiv = document.createElement('div');
        this.pointsDiv.className = "col-auto h-100 p-0 align-items-left";
        this.pointsDiv.appendChild(this.taskNumber);
        this.pointsDiv.appendChild(this.maxPointsInput);
        
        // Pytanie: 
        this.questionInput = document.createElement('textarea');
        this.questionInput.className = "form-control task-input bg-white border-secondary h-100";
        this.questionInput.placeholder = "Wpisz polecenie (opcjonalne)";

        this.questionFormGroup = document.createElement('div');
        this.questionFormGroup.className = "form-group h-100";
        this.questionFormGroup.appendChild(this.questionInput);

        this.questionDiv = document.createElement('div');
        this.questionDiv.className = "col h-100 px-2 pb-1";
        this.questionDiv.appendChild(this.questionFormGroup);        

        // Parametry (metrum i tonacja):
        this.metreSelect = document.createElement('select');
        this.metreSelect.innerHTML = `
            <option value="2/4">2/4</option>
            <option value="3/4">3/4</option>
            <option value="4/4">4/4</option>
            <option value="3/8">3/8</option>
            <option value="6/8">6/8</option>
        `;
        this.metreSelect.ariaLabel = "Metrum";
        this.metreSelect.className = "form-control bg-white border-secondary";
        this.metreSelect.value = "2/4";

        this.tonationLabel = document.createElement('label');
        this.tonationLabel.innerText = "Tonacja";
        this.tonationLabel.className = "fw-bold ms-auto me-2";

        this.tonationNameSelect = document.createElement('select');
        this.tonationNameSelect.innerHTML = `
            <option value="C">C</option>
            <option value="Ces">Ces</option>
            <option value="Cis">Cis</option>

            <option value="D">D</option>
            <option value="Des">Des</option>
            <option value="Dis">Dis</option>

            <option value="E">E</option>
            <option value="Es">Es</option>

            <option value="F">F</option>
            <option value="Fis">Fis</option>

            <option value="G">G</option>
            <option value="Gis">Gis</option>
            <option value="Ges">Ges</option>

            <option value="A">A</option>
            <option value="Ais">Ais</option>
            <option value="As">As</option>

            <option value="H">H</option>
            <option value="H">B</option>`;
        this.tonationNameSelect.className = "form-control bg-white border-secondary";

        this.tonationModeSelect = document.createElement('select');
        this.tonationModeSelect.innerHTML = `
            <option value="dur">Dur</option>
            <option value="moll">moll</option>
        `;
        this.tonationModeSelect.className = "form-control bg-white border-secondary";

        const nameCol = document.createElement('div');
        nameCol.className = "col";
        nameCol.appendChild(this.tonationNameSelect);

        const modeCol = document.createElement('div');
        modeCol.className = "col";
        modeCol.appendChild(this.tonationModeSelect);

        this.paramsInnerDiv = document.createElement('div');
        this.paramsInnerDiv.className = "row mt-1 gx-1";
        this.paramsInnerDiv.appendChild(nameCol);
        this.paramsInnerDiv.appendChild(modeCol);

        this.dropdownsFormGroup = document.createElement('div');
        this.dropdownsFormGroup.className = "align-items-center h-100";
        this.dropdownsFormGroup.appendChild(this.metreSelect);
        // this.dropdownsFormGroup.appendChild(this.tonationLabel);
        this.dropdownsFormGroup.appendChild(this.paramsInnerDiv);

        this.dropdownsDiv = document.createElement('div');
        this.dropdownsDiv.className = "col-auto p-0 h-100";
        this.dropdownsDiv.appendChild(this.dropdownsFormGroup);

        this.deleteButtonContent = document.createElement('div');
        this.deleteButtonContent.className = "trash-icon-white mx-auto w-100 h-100";

        this.deleteButton = document.createElement('button');
        this.deleteButton.className = "col-auto ms-2 mb-1 btn btn-danger p-4";
        this.deleteButton.style = "width: 81px;";
        this.deleteButton.appendChild(this.deleteButtonContent);

        // Główna forma:
        this.form = document.createElement('div');
        this.form.className = "d-flex flex-row";
        this.form.style = "height: 85px;";
        this.form.appendChild(this.pointsDiv);
        this.form.appendChild(this.questionDiv);
        this.form.appendChild(this.dropdownsDiv);
        this.form.appendChild(this.deleteButton);

        // Kontener parametrów
        this.params = document.createElement('div');
        this.params.appendChild(this.form);

        // Pełny kontener zadania
        // Kontener zadania: 
        this.container = document.createElement('div');
        this.container.className = "task-content w-100 rounded mb-4";

        this.container.appendChild(this.hiddenInput);
        this.container.appendChild(this.params);
        this.container.appendChild(this.randomTaskButton);
        this.container.appendChild(this.randomTaskInput);
        this.container.appendChild(this.barContainer.container);

        this.setId(this.taskIndex);

        this.handleChangeMaxBars = this.changeMaxBars.bind(this);
        this.metreSelect.addEventListener('change', this.handleChangeMaxBars);
    }

    changeMaxBars() {
        const value = this.metreSelect.value;

        this.barContainer.bars.forEach(b => {
            if (value === "3/8") {
                b.functionContainer.maxFunctionsCount = 6;
                // TODO:
                // b.functionContainer.trim();
            }
            else {
                // TODO: Poprawić
                b.functionContainer.maxFunctionsCount = 6;
            }
        });
    }

    setId(taskIndex) {
        this.deleteButton.removeEventListener('click', this.handleRemove);
        this.randomTaskButton.removeEventListener('click', () => this.generate());

        this.taskIndex = taskIndex;

        this.randomTaskButton.id = `random-task-${this.taskIndex}`;

        this.metreSelect.id = `metre-select-task-${taskIndex}`;
        this.taskNumber.value = `Zadanie ${taskIndex + 1}`;

        this.tonationLabel.for = `tonation-name-select-task-${taskIndex}`;
        this.tonationNameSelect.id = `tonation-name-select-task-${taskIndex}`;
        this.tonationModeSelect.id = `mode-task-${taskIndex}`;

        this.deleteButton.id = `delete-${taskIndex}`;
        this.container.id = `task-container-${taskIndex}`;

        this.questionInput.id = `question-task-${taskIndex}`;
        this.hiddenInput.name = `Questions[${taskIndex}]`;
        this.hiddenInput.id = `question-json-${taskIndex}`;

        this.maxPointsInput.id = `max-points-${taskIndex}`;

        this.barContainer.setId(taskIndex);
        this.handleRemove = this.taskContainer.removeTask.bind(this.taskContainer, this.taskIndex);
        this.deleteButton.addEventListener('click', this.handleRemove);

        this.randomTaskButton.addEventListener('click', () => this.generate());
        this.randomTaskInput.id = `random-bars-${this.taskIndex}`;
    }

    getResult() {
        const result = [];

        this.barContainer.bars.forEach(b => {
            b.functionContainer.functions.forEach(f => {
                const toAdd = f.functionCreator.confirm();
                result.push(toAdd);
            });
        });

        return result;
    }

    // Tutaj wlatuje obiekt zadania
    load(exerciseObject) {
        this.barContainer.removeAll();

        const sharpsCount = exerciseObject.sharpsCount;
        const flatsCount = exerciseObject.flatsCount;
        const minor = exerciseObject.minor;
        const metreValue = exerciseObject.metreValue;
        const metreCount = exerciseObject.metreCount;
        const taskObject = exerciseObject.task;
        const question = exerciseObject.question;
        const maxPoints = exerciseObject.maxPoints;

        const metreValueResult = parseMetreValuesToMetre([metreValue, metreCount]);
        const tonationInfo = parseAccidentalsCountToTonationInfo([sharpsCount, flatsCount, minor]);

        this.tonationNameSelect.value = tonationInfo[0];
        this.tonationModeSelect.value = tonationInfo[1];
        this.metreSelect.value = metreValueResult;
        this.questionInput.value = question;
        this.maxPointsInput.value = maxPoints;

        let index = 0;
        let lastBar = 0;

        taskObject.forEach(parsedFunction => {
            const barIndex = Number(parsedFunction.BarIndex);

            if (barIndex != lastBar)
                index = 0;

            let barCount = this.barContainer.bars.length;

            while (barIndex >= barCount) {                
                this.barContainer.addBar();
                barCount++;
            }

            const bar = this.barContainer.bars[barIndex];
            const functionCount = bar ?
                bar.functionContainer.functions.length - 1 :
                0;

            if (index >= functionCount) {
                bar.functionContainer.addFunction();
            }

            const functionCountInBar = bar.functionContainer.functions.length;
            const newFunction = bar.functionContainer.functions[functionCountInBar - 1];

            if (parsedFunction.Minor != null && parsedFunction.Minor)
                newFunction.functionCreator.minor.checked = true;

            if (parsedFunction.Symbol != null)
                newFunction.functionCreator.symbol.value = parsedFunction.Symbol;

            if (parsedFunction.Root != null)
                newFunction.functionCreator.root.value = parsedFunction.Root;

            if (parsedFunction.Position != null)
                newFunction.functionCreator.position.value = parsedFunction.Position;

            if (parsedFunction.Removed != null)
                newFunction.functionCreator.removed.value = parsedFunction.Removed;

            if (parsedFunction.Added != null) {
                parsedFunction.Added.forEach(a => {
                    const component = a[0];
                    const option = a.length == 1 ? "-" : a[1];

                    newFunction.functionCreator.createAddedElement(component, option);
                });
            }            

            // TODO: Alteracje (Alterations)
            // TODO: Opóźnienia (Suspensions)

            index++;
        });
    }

    generate() {
        const questionText = this.questionInput.value;

        const tonationLetterSelect = this.tonationNameSelect;
        const modeSelect = this.tonationModeSelect;
        const metreSelect = this.metreSelect;

        const metreSplitted = metreSelect.value.split('/');
        const metreCount = metreSplitted[0];
        const metreValue = metreSplitted[1];

        const tonationString = `${tonationLetterSelect.value}${modeSelect.value}`;
        const tonationParsed = parseTonationToAccidentalsCount(tonationString);
        const sharpsCount = tonationParsed[0];
        const flatsCount = tonationParsed[1];
        const minor = tonationParsed[2];
        const maxPoints = Number(this.maxPointsInput.value);

        let barsValue = this.randomTaskInput.value;

        if (barsValue == null || barsValue <= 0) {
            barsValue = 2;
        }

        else if (barsValue > 16) {
            barsValue = 16;
        }            

        this.randomTaskInput.value = barsValue;        
        const bars = barsValue; // TODO
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        // Call the backend
        fetch("/Creator?handler=GenerateTask", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify({
                Bars: bars,
                MetreValue: metreValue,
                MetreCount: metreCount,
                SharpsCount: sharpsCount,
                FlatsCount: flatsCount,
                Minor: minor
            })
        })
        .then(response => response.json())
        .then(data => {
            const taskResult = JSON.parse(data);

            const exercise = {
                question: questionText,
                sharpsCount: Number(sharpsCount),
                flatsCount: Number(flatsCount),
                minor: Number(minor),
                metreValue: Number(metreValue),
                metreCount: Number(metreCount),
                maxPoints: Number(maxPoints),
                task: taskResult
            };

            this.load(exercise);
        })
        .catch(error => console.error('Error:', error));
    }
}

export class TaskContainer {
    constructor(parent, maxTasksCount = 100) {
        this.tasks = [];
        this.maxTasksCount = maxTasksCount;
        this.parent = parent;

        this.addTask();
    }

    addTask() {        
        const taskCount = this.tasks.length;

        if (taskCount < this.maxTasksCount) {
            const toAdd = new Task(this, taskCount);            
            this.tasks.push(toAdd);
            this.parent.appendChild(toAdd.container);
        }
    }

    removeTask(taskIndex) {
        const taskCount = this.tasks.length;

        if (taskIndex >= taskCount)
            return;

        const currentTask = this.tasks[taskIndex];

        this.tasks.splice(taskIndex, 1);      
        this.setId(this.taskIndex);
        this.parent.removeChild(currentTask.container);        
    }

    setId(taskIndex = 0) {
        const taskCount = this.tasks.length;

        for (let index = taskIndex; index < taskCount; index++) {
            const toChange = this.tasks[index];

            if (toChange != null)
                toChange.setId(index);
        }
    }

    // Ładowanie z całego JSONa
    load(exercises) {
        let index = 0;

        exercises.forEach(exercise => {
            const exerciseObject = JSON.parse(exercise);           
            const tasksCount = this.tasks.length - 1;

            if (index > tasksCount)
                this.addTask();

            const task = this.tasks[index];

            task.load(exerciseObject);

            index++;
        });
    }
}