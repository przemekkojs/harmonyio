import { Elements } from "./addFunctionality.js";

const all = [];

function createComponent(parent, taskIndex, barIndex, verticalIndex) {
    const newId = `${taskIndex}-${barIndex}-${verticalIndex}`;
    const newComponent = `
        <div id="${newId}">
            <div class="suspension-popup popup-base" id="suspension-popup-${newId}">
                <span class="popup-title">Dodaj opóźnienie</span>
            
                <form id="add-suspension-form-${newId}">
                    <input type="button" name="confirm-button" id="confirm-add-suspension-${newId}" value="Dodaj" class="confirm-button">
                    <input type="button" name="cancel-button" id="cancel-add-suspension-${newId}" value="Anuluj" class="cancel-button">
                </form>
            </div>
            
            <div class="alteration-popup popup-base" id="alteration-popup-${newId}">
                <span class="popup-title">Dodaj alterację</span>
            
                <form id="add-alteration-form-${newId}">
                    <div class="component-choice">
                        <input type="radio" name="component" id="prima-${newId}" value="prima">
                        <label for="prima-${newId}">1</label>
                        <input type="radio" name="component" id="third-${newId}" value="third">
                        <label for="third-${newId}">3</label>
                        <input type="radio" name="component" id="fifth-${newId}" value="fifth">
                        <label for="fifth-${newId}">5</label>
                    </div>
            
                    <div class="radio-buttons">
                        <input type="radio" name="type" id="down-${newId}" value="down">
                        <label for="down-${newId}">&darr;</label>
                        <input type="radio" name="type" id="up-${newId}" value="up">
                        <label for="up-${newId}">&uarr;</label>
                    </div>
            
                    <input type="button" name="confirm-button" id="confirm-add-alteration-${newId}" value="Dodaj" class="confirm-button">
                    <input type="button" name="cancel-button" id="cancel-add-alteration-${newId}" value="Anuluj" class="cancel-button">
                </form>
            </div>
            
            <div class="added-popup popup-base" id="added-popup-${newId}">
                <span class="popup-title">Dodaj składnik</span>
            
                <form id="add-added-form-${newId}">
                    <input type="button" name="confirm-button" id="confirm-add-added-${newId}" value="Dodaj" class="confirm-button">
                    <input type="button" name="cancel-button" id="cancel-add-added-${newId}" value="Anuluj" class="cancel-button">
                </form>
            </div>
            
            <div class="function-creator">
                <form id="main-${newId}">
                    <div class="grid-container">
                        <div class="left-brace"></div>   
                    
                        <div class="minor">
                            <input type="checkbox" name="minor" id="minor-${newId}">
                        </div>
                    
                        <div class="symbol">
                            <select name="symbol" id="symbol-${newId}"></select>
                        </div>
                    
                        <div class="position">
                            <select name="position" id="position-${newId}" class=""></select>
                        </div>
                    
                        <div class="root">
                            <select name="root" id="root-${newId}"></select>
                        </div>
                    
                        <div class="added">
                            <input type="button" name="add-added" id="add-added-${newId}" value="+">
                        </div>
                    
                        <div class="suspension">
                            <input type="button" name="add-suspension" id="add-suspension-${newId}" value="+">
                        </div>
                    
                        <div class="alteration">
                            <input type="button" name="add-alteration" id="add-alteration-${newId}" value="+">
                        </div>
                    
                        <div class="removed">
                            <select name="removed" id="removed-${newId}"></select>
                        </div>
                    
                        <div class="right-brace"></div>
                    </div>
                
                    <div id="form-buttons-${newId}">
                        <input type="button" name="cancel-creator" id="cancel-creator-${newId}" value="Anuluj">
                        <input type="button" name="reset-creator" id="reset-creator-${newId}" value="Resetuj">
                        <input type="submit" value="Dodaj" id="submit-creator-${newId}">
                    </div>
                </form>
            </div>
        </div>`;

    parent.insertAdjacentHTML('beforeend', newComponent);
    all.push(new Elements(newId));
}

window.onload = () => {
    createComponent(document.getElementById('task-1'), 0, 0, 0);
    createComponent(document.getElementById('task-2'), 1, 0, 0);
    createComponent(document.getElementById('task-3'), 2, 0, 0);
    createComponent(document.getElementById('task-2'), 1, 1, 0);
};