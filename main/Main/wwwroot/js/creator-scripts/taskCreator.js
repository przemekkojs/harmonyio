import { Task } from "./taskEditor.js";

export class TaskContainer {
    constructor(parent, id) {
        this.parent = parent;
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

        this.componentsPlace = document.createElement('div');
        this.componentsPlace.className = "components-place";

        this.taskSubmit = document.createElement('div');
        this.taskSubmit.className = "task-submit";

        this.deleteButton = document.createElement('input');
        this.deleteButton.type = "button";
        this.deleteButton.className = "btn btn-danger btn-sm";
        this.deleteButton.style = "width: 100px;";
        this.deleteButton.value = "Usuń";

        this.taskSubmit.appendChild(this.deleteButton);

        this.parent.appendChild(this.params);
        this.parent.appendChild(this.componentsPlace);
        this.parent.appendChild(this.taskSubmit);

        console.log(id);
        this.setId(id);

        this.task = new Task(id);
    }

    setId(id) {
        this.id = id;
        this.parent.id = `task-${id}`;

        console.log(this.parent.id);

        this.metreLabel.for = `metre-select-task-${id}`;
        this.metreSelect.id = `metre-select-task-${id}`;

        this.tonationLabel.for = `tonation-name-select-task-${id}`;
        this.tonationNameSelect.id = `tonation-name-select-task-${id}`;
        this.tonationAccidentalSelect.id = `accidental-task-${id}`;
        this.tonationModeSelect.id = `mode-task-${id}`;

        this.componentsPlace.id = `components-place-${id}`;
        this.deleteButton.id = `delete-${id}`;

        if (this.task != null) {
            this.task.id = id;

            this.task.bars.forEach(b => {
                b.fun
            })
        }            
    }
}