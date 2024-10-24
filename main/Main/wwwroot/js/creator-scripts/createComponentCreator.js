export function createComponent(newId) {
    const newComponent = `
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
        
                <input type="button" name="confirm-button" id="confirm-add-alteration-${newId}" value="Dodaj" class="confirm-button">
                <input type="button" name="cancel-button" id="cancel-add-alteration-${newId}" value="Anuluj" class="cancel-button">
            </form>
        </div>
        
        <div class="added-popup popup-base" id="added-popup-${newId}">
            <span class="popup-title">Dodaj składnik</span>

            <form id="add-added-form-${newId}">
                <div id="add-added-form-${newId}-components">
                    <input type="radio" name="component" id="add-sixth-${newId}" value="sixth">
                    <label for"add-sixth-${newId}">6</label>
                    <input type="radio" name="component" id="add-seventh-${newId}" value="seventh">
                    <label for"add-seventh-${newId}">7</label>
                    <input type="radio" name="component" id="add-ninth-${newId}" value="ninth">
                    <label for"add-ninth-${newId}">9</label>
                </div>
                    
                <div id="add-added-form-${newId}-options">
                    <input type="radio" name="type" id="major-${newId}" value="major">
                    <label for="major-${newId}">&lt;</label>
                    <input type="radio" name="type" id="neutral-${newId}" value="neutral" checked>
                    <label for="neutral-${newId}">-</label>
                    <input type="radio" name="type" id="minor-${newId}" value="minor">
                    <label for="minor-${newId}">&gt;</label>
                </div>

                <input type="button" name="confirm-button" id="confirm-add-added-${newId}" value="Dodaj" class="confirm-button">
                <input type="button" name="cancel-button" id="cancel-add-added-${newId}" value="Anuluj" class="cancel-button">
            </form>
        </div>
        
        <div class="function-creator">
            <form id="main-${newId}">
                <div class="grid-container">
                    <div class="left-brace" id="left-brace-container-${newId}"></div>   

                    <div class="minor" id="minor-container-${newId}">
                        <label class="custom-checkbox">
                            <input type="checkbox" name="minor" id="minor-${newId}">
                            <span class="checkmark" title="Moll?"></span>
                        </label>
                    </div>
                
                    <div class="symbol" id="symbol-container-${newId}">
                        <select name="symbol" id="symbol-${newId}" title="Wybierz symbol funkcji" style="font-size: 30px; font-weight: bold;"></select>
                    </div>
                
                    <div class="position" id="position-container-${newId}">
                        <select name="position" id="position-${newId}" title="Wybierz pozycję"></select>
                    </div>
                
                    <div class="root" id="root-container-${newId}">
                        <select name="root" id="root-${newId}" title="Wybierz oparcie"></select>
                    </div>
                
                    <div class="added" id="added-container-${newId}">
                        <input type="button" name="add-added" id="add-added-${newId}" title="Dodaj składnik dysonujący">
                    </div>
                
                    <div class="suspension" id="suspension-container-${newId}">
                        <!--<input type="button" name="add-suspension" id="add-suspension-${newId}" title="Dodaj opóźnienie">-->
                    </div>
                
                    <div class="alteration" id="alteration-container-${newId}">
                        <input type="button" name="add-alteration" id="add-alteration-${newId}" title="Dodaj alterację">
                    </div>
                
                    <div class="removed" id="removed-container-${newId}">
                        <select name="removed" id="removed-${newId}" title="Wybierz składnik dodany"></select>
                    </div>
                
                    <div class="right-brace" id="right-brace-container-${newId}"></div>
                </div>
            
                <div id="form-buttons-${newId}" class="function-form-buttons">
                    <input type="button" name="cancel-creator" id="cancel-creator-${newId}" alt="Usuń funkcję" title="Usuń funkcję" class="custom-button trash-button button-small">
                    <input type="button" name="reset-creator" id="reset-creator-${newId}" alt="Resetuj funkcję" title="Resetuj funkcję" class="custom-button round-arrow-button button-small">
                    <input type="button" name="submit-creator" id="submit-creator-${newId}" alt="Dodaj funkcję" title="Dodaj funkcję" class="custom-button tick-button button-small">
                </div>
            </form>
        </div>`;

    return newComponent;
}