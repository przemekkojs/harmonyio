import { BarContainer } from "./barContainer.js";
import { parseAccidentalsCountToTonationInfo, parseMetreValuesToMetre, parseTonationToAccidentalsCount } from "../utils.js";

class Task {
    constructor(taskContainer, taskIndex, maxBars = 16) {
        this.taskIndex = taskIndex;
        this.maxBars = maxBars;
        this.taskContainer = taskContainer;

        this.barContainer = new BarContainer(taskContainer, maxBars);
        this.handleRemove = this.taskContainer.removeTask.bind(this.taskContainer, this.taskIndex);

        // Ukryty input
        this.hiddenInput = document.createElement('input');
        this.hiddenInput.type = "hidden";

        // Główny kontener
        this.form = document.createElement('div');
        this.form.className = 'd-flex flex-row gap-2';
        this.form.style.height = '132px';

        // Pierwszy podkontener
        const column1 = document.createElement('div');
        column1.className = 'd-flex flex-column gap-2 h-100';

        // Numer zadania
        this.taskNumber = document.createElement('div');
        this.taskNumber.className = 'bg-secondary text-white rounded align-items-center h-50 justify-content-center d-flex fs-4';
        this.taskNumber.style.width = '60px';
        this.taskNumber.textContent = '1';
        column1.appendChild(this.taskNumber);

        // Ikona kosza
        this.deleteButton = document.createElement('button');
        this.deleteButton.className = 'btn btn-danger h-50';
        this.deleteButton.style.width = '60px';

        const trashIcon = document.createElement('i');
        trashIcon.className = 'fas fa-trash';
        trashIcon.style.fontSize = '20px';
        this.deleteButton.appendChild(trashIcon);

        column1.appendChild(this.deleteButton);
        this.form.appendChild(column1);

        // Drugi podkontener
        const column2 = document.createElement('div');
        column2.className = 'flex-grow-1 d-flex flex-column gap-2 h-100';

        // Wiersz pól wejściowych
        const inputRow = document.createElement('div');
        inputRow.className = 'h-50 d-flex flex-row gap-2';

        // Pole "Max punkty"
        const maxPointsSection = document.createElement('div');
        maxPointsSection.className = 'h-100 d-flex flex-column';
        maxPointsSection.style.maxWidth = '150px';
        
        const maxPointsLabel = document.createElement('div');
        maxPointsLabel.className = 'fw-bold text-nowrap';
        maxPointsLabel.textContent = 'Max punkty';
        
        this.maxPointsInput = document.createElement('input');
        this.maxPointsInput.type = 'number';
        this.maxPointsInput.value = '10';
        this.maxPointsInput.className = 'form-control points-input border-secondary bg-white';
        
        maxPointsSection.appendChild(maxPointsLabel);
        maxPointsSection.appendChild(this.maxPointsInput);

        inputRow.appendChild(maxPointsSection);

        // Pole "Metrum"        
        const metreSection = document.createElement('div');
        metreSection.className = 'h-100 d-flex flex-column';
        metreSection.style.maxWidth = '150px';
        metreSection.style.width = '100px';
        
        const metreLabel = document.createElement('div');
        metreLabel.className = 'fw-bold text-nowrap';
        metreLabel.textContent = 'Metrum';
        
        this.metreSelect = document.createElement('select');
        this.metreSelect.value = '2/4';
        this.metreSelect.className = 'form-control points-input border-secondary bg-white';
        this.metreSelect.innerHTML = `
            <option value="2/4">2/4</option>
            <option value="3/4">3/4</option>
            <option value="4/4">4/4</option>
            <option value="3/8">3/8</option>
            <option value="6/8">6/8</option>
        `;
        
        metreSection.appendChild(metreLabel);
        metreSection.appendChild(this.metreSelect);
        
        inputRow.appendChild(metreSection);

        // Pole "Tonacja"        
        const keySignature = document.createElement('div');
        keySignature.className = 'h-100 d-flex flex-column';
        keySignature.style.maxWidth = '150px';

        const keyLabel = document.createElement('div');
        keyLabel.className = 'fw-bold text-nowrap';
        keyLabel.textContent = 'Tonacja';
        keySignature.appendChild(keyLabel);

        const keyInputs = document.createElement('div');
        keyInputs.className = 'd-flex flex-row gap-1';

        this.tonationNameSelect = document.createElement('select');
        this.tonationNameSelect.value = 'C';
        this.tonationNameSelect.className = 'form-control points-input border-secondary bg-white';
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

        this.tonationModeSelect = document.createElement('select');
        this.tonationModeSelect.value = 'dur';
        this.tonationModeSelect.className = 'form-control points-input border-secondary bg-white';
        this.tonationModeSelect.innerHTML = `
            <option value="dur">Dur</option>
            <option value="moll">Moll</option>
        `;

        keyInputs.appendChild(this.tonationNameSelect);
        keyInputs.appendChild(this.tonationModeSelect);
        keySignature.appendChild(keyInputs);

        inputRow.appendChild(keySignature);
        column2.appendChild(inputRow);

        // Polecenie (textarea)
        this.questionInput = document.createElement('textarea');
        this.questionInput.className = 'rounded h-50 p-2';
        this.questionInput.placeholder = 'Wpisz polecenie (opcjonalne)';
        column2.appendChild(this.questionInput);

        this.form.appendChild(column2);

        // Trzeci podkontener
        const column3 = document.createElement('div');
        column3.className = 'd-flex flex-column rounded border border-secondary overflow-hidden';
        column3.style.width = '180px';

        const randomTaskHeader = document.createElement('div');
        randomTaskHeader.className = 'fw-bold bg-secondary text-white w-100 mb-auto p-2 text-center text-nowrap';
        randomTaskHeader.textContent = 'Wylosuj zadanie';
        column3.appendChild(randomTaskHeader);

        const randomTaskBody = document.createElement('div');
        randomTaskBody.className = 'd-flex flex-row gap-2 p-2';

        // Pole "Takty"         
        const randomTaskInputSection = document.createElement('div');
        randomTaskInputSection.className = 'h-100 d-flex flex-column';
        randomTaskInputSection.style.maxWidth = 'auto';
        
        const randomTaskInputLabel = document.createElement('div');
        randomTaskInputLabel.className = 'fw-bold text-nowrap';
        randomTaskInputLabel.textContent = 'Takty';
        
        this.randomTaskInput = document.createElement('input');
        this.randomTaskInput.type = 'number';
        this.randomTaskInput.value = '2';
        this.randomTaskInput.className = 'form-control points-input border-secondary bg-white';
        
        randomTaskInputSection.appendChild(randomTaskInputLabel);
        randomTaskInputSection.appendChild(this.randomTaskInput);
        
        randomTaskBody.appendChild(randomTaskInputSection);

        // Przycisk kostki
        this.randomTaskButton = document.createElement('button');
        this.randomTaskButton.type = 'button';
        this.randomTaskButton.className = 'btn btn-secondary';
        this.randomTaskButton.style.minWidth = '62px';

        const diceIcon = document.createElement('i');
        diceIcon.className = 'fas fa-dice';
        diceIcon.style.fontSize = '20px';
        this.randomTaskButton.appendChild(diceIcon);

        randomTaskBody.appendChild(this.randomTaskButton);
        column3.appendChild(randomTaskBody);

        this.form.appendChild(column3);

        // Funkcja do tworzenia pola wejściowego
        function createInput(type, value, className) {
            const input = document.createElement('input');
            input.type = type;
            input.value = value;
            input.className = className;
            return input;
        }

        this.form.appendChild(this.hiddenInput);
        this.form.appendChild(this.barContainer.container);

        // Pełny kontener zadania
        this.container = document.createElement('div');
        this.container.className = "task-content w-100 rounded mb-4";

        this.container.appendChild(this.form);
        this.container.appendChild(this.barContainer.container);

        this.setId(this.taskIndex);
    }

    setId(taskIndex) {
        this.deleteButton.removeEventListener('click', this.handleRemove);
        this.randomTaskButton.removeEventListener('click', () => this.generate());

        this.taskIndex = taskIndex;

        this.randomTaskButton.id = `random-task-${this.taskIndex}`;

        this.metreSelect.id = `metre-select-task-${taskIndex}`;
        this.taskNumber.innerText = `${taskIndex + 1}`;

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