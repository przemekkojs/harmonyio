import { Component } from "./component.js";

export function createComponent(newId) {
    const newComponent2 = `
        <div class="suspension-popup popup-base" id="suspension-popup-${newId}">
            <span class="popup-title">Dodaj opóźnienie</span>
        
            <input type="button" name="confirm-button" id="confirm-add-suspension-${newId}" value="Dodaj opóźnienie" class="confirm-button">
            <input type="button" name="cancel-button" id="cancel-add-suspension-${newId}" value="Anuluj opóźnienie" class="cancel-button">
        </div>
        
        <div class="alteration-popup popup-base" id="alteration-popup-${newId}">
            <span class="popup-title">Dodaj alterację</span>

            <div class="component-choice">
                <input type="radio" name="component" id="alter-root-${newId}" value="root">
                <label for="alter-root-${newId}">1</label>
                <input type="radio" name="component" id="alter-third-${newId}" value="third">
                <label for="alter-third-${newId}">3</label>
                <input type="radio" name="component" id="alter-fifth-${newId}" value="fifth">
                <label for="alter-fifth-${newId}">5</label>
            </div>
        
            <div class="radio-buttons">
                <input type="radio" name="type" id="down-${newId}" value="down">
                <label for="down-${newId}">&darr;</label>
                <input type="radio" name="type" id="up-${newId}" value="up">
                <label for="up-${newId}">&uarr;</label>
            </div>
        
            <input type="button" name="confirm-button" id="confirm-add-alteration-${newId}" value="Dodaj alterację" class="confirm-button">
            <input type="button" name="cancel-button" id="cancel-add-alteration-${newId}" value="Anuluj alterację" class="cancel-button">
        </div>
        
        <div class="added-popup popup-base" id="added-popup-${newId}">
            <span class="popup-title">Dodaj składnik</span>

            <div id="add-added-form-${newId}-components">
                <input type="radio" name="component" id="add-sixth-${newId}" value="sixth">
                <label for="add-sixth-${newId}">6</label>
                <input type="radio" name="component" id="add-seventh-${newId}" value="seventh">
                <label for="add-seventh-${newId}">7</label>
                <input type="radio" name="component" id="add-ninth-${newId}" value="ninth">
                <label for="add-ninth-${newId}">9</label>
            </div>
                    
            <div id="add-added-form-${newId}-options">
                <input type="radio" name="type" id="added-major-${newId}" value="major">
                <label for="added-major-${newId}">&lt;</label>
                <input type="radio" name="type" id="added-neutral-${newId}" value="neutral" checked>
                <label for="added-neutral-${newId}">-</label>
                <input type="radio" name="type" id="added-minor-${newId}" value="minor">
                <label for="added-minor-${newId}">&gt;</label>
            </div>

            <input type="button" name="confirm-button" id="confirm-add-added-${newId}" value="Dodaj składnik" class="confirm-button">
            <input type="button" name="cancel-button" id="cancel-add-added-${newId}" value="Anuluj składnik" class="cancel-button">
        </div>
        
        <div class="function-creator">
            <div class="grid-container">
                <div class="left-brace-container" id="left-brace-container-${newId}">
                    <select name="left-brace" id="left-brace-${newId}" title="Wybierz typ wtrącenia">
                        <option value=" " selected> </option>    
                        <option value="(">(</option>
                        <option value="[">[</option>
                    </select>
                </div>
                <div class="minor-container" id="minor-container-${newId}">
                    <label class="custom-checkbox">
                        <input type="checkbox" name="minor" id="minor-${newId}">
                        <span class="checkmark checkmark-border" title="Moll?" id="minor-span-${newId}"></span>
                    </label>
                </div>
                
                <div class="symbol-container" id="symbol-container-${newId}">
                    <select name="symbol" id="symbol-${newId}" title="Wybierz symbol funkcji" style="font-size: 20px;"></select>
                </div>
                
                <div class="position-container" id="position-container-${newId}">
                    <select name="position" id="position-${newId}" title="Wybierz pozycję"></select>
                </div>
                
                <div class="root-container" id="root-container-${newId}">
                    <select name="root" id="root-${newId}" title="Wybierz oparcie"></select>
                </div>
                
                <div class="added-container" id="added-container-${newId}">
                    <input type="button" name="add-added" id="add-added-${newId}" title="Dodaj składnik dysonujący" value=" ">
                </div>
                
                <div class="suspension-container" id="suspension-container-${newId}">
                    <input type="button" name="add-suspension" id="add-suspension-${newId}" title="Dodaj opóźnienie" value=" ">
                </div>
                
                <div class="alteration-container" id="alteration-container-${newId}">
                    <input type="button" name="add-alteration" id="add-alteration-${newId}" title="Dodaj alterację" value=" ">
                </div>
                
                <div class="removed-container" id="removed-container-${newId}">
                    <select name="removed" id="removed-${newId}" title="Wybierz składnik dodany"></select>
                </div>
                
                <div class="right-brace-container" id="right-brace-container-${newId}">
                    <select name="right-brace" id="right-brace-${newId}" title="Wybierz typ wtrącenia">
                        <option value=" " selected> </option>    
                        <option value=")">)</option>
                        <option value="]">]</option>
                    </select>
                </div>
            </div>
            
            <div id="form-buttons-${newId}" class="function-form-buttons">
                <input type="button" name="cancel-creator" id="cancel-creator-${newId}" title="Usuń funkcję" class="custom-button trash-button button-small" value=" ">
                <input type="button" name="reset-creator" id="reset-creator-${newId}" title="Resetuj funkcję" class="custom-button round-arrow-button button-small" value=" ">
                <input type="button" name="submit-creator" id="submit-creator-${newId}" title="Dodaj funkcję" class="custom-button tick-button button-small" value=" ">
            </div>
        </div>`;

    const component = new Component(newId);

    return component;
}