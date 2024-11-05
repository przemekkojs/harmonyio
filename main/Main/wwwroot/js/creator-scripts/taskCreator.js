import { Task } from "./taskEditor.js";

export const tasks = [];

export function createTask(parent, id) {
    let taskHtml = `
        <div class="params">
            <label for="metre-select-task-${id}">Metrum: </label>        
            <select id="metre-select-task-${id}">
                <option value="2/4">2/4</option>
                <option value="3/4">3/4</option>
                <option value="4/4">4/4</option>
                <option value="3/8">3/8</option>
                <option value="6/8">6/8</option>
            </select>

            <label for="tonation-name-select-task-${id}">Tonacja: </label>
            <select id="tonation-name-select-task-${id}" class="smaller-select">
                <option value="C">C</option>
                <option value="D">D</option>
                <option value="E">E</option>
                <option value="F">F</option>
                <option value="G">G</option>
                <option value="A">A</option>
                <option value="H">H</option>
            </select>
                    
            <label for="accidental-task-${id}"></label>
            <select id="accidental-task-${id}" class="smaller-select">
                <option value=" "> </option>
                <option value="#">#</option>
                <option value="b">b</option>
            </select>

            <label for="mode-task-${id}"></label>
            <select id="mode-task-${id}">
                <option value="dur">Dur</option>
                <option value="moll">moll</option>
            </select>
        </div>

        <div id="components-place-${id}" class="components-place">
                
        </div>

        <div class="task-submit">           
            <input type="button" value="Usuń" id="delete-${id}" class="btn btn-danger btn-sm" style="width: 100px;">
        </div>`;

    let taskDiv = document.createElement('div');
    taskDiv.id = `task-${id}`;
    taskDiv.innerHTML = taskHtml;

    parent.appendChild(taskDiv);

    let toAppend = new Task(id);    
    tasks.push(toAppend);
}