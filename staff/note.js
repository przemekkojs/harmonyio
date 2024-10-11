class Note{
    static availableBaseValues = [1,2,4,8,16];

    constructor(baseValue = 1, hasDot = false, isFacingUp = true){
        this.hasDot = hasDot;
        this.setBaseValue(baseValue);
        this.isFacingUp = isFacingUp;
        // TODO add chromatic symbol
    }

    setBaseValue(baseValue) {
        if(Note.availableBaseValues.includes(baseValue)) {
            this.baseValue = baseValue;

            // when setting last available note value, disable dot
            const lastAvailableBaseValueIndex = Note.availableBaseValues.length - 1;
            const lastAvailableBaseValue = Note.availableBaseValues[lastAvailableBaseValueIndex]; 
            if(baseValue === lastAvailableBaseValue) {
                this.hasDot = false;
            }
        } else {
            this.baseValue = 4; // if baseValue is not correct, default to 4
        }
    }

    getBaseValue() {
        return this.baseValue;
    }

    getValue() {
        if(this.hasDot) {
            return this.baseValue + 2 * this.baseValue;
        } else {
            return this.baseValue;
        }
    }

    getSlotsTaken() {
        if (this.hasDot) {
            return Note.baseValueToSlots(this.baseValue) + Note.baseValueToSlots(this.baseValue * 2)
        } else {
            return Note.baseValueToSlots(this.baseValue)
        }
    }

    hasStem() {
        return this.baseValue !== 1;
    }

    getNumberOfFlags() {
        if (this.baseValue < 8) {
            return 0;
        }

        if (this.baseValue === 8) {
            return 1;
        }
        return 2;
    }

    toggleIsFacingUp() {
        return this.isFacingUp = !this.isFacingUp;
    }

    toggleHasDot(){
        // dont allow to have dot while being last possible note (16)
        const lastAvailableBaseValueIndex = Note.availableBaseValues.length - 1;
        const lastAvailableBaseValue = Note.availableBaseValues[lastAvailableBaseValueIndex]
        if(this.baseValue !== lastAvailableBaseValue) {
            this.hasDot = !this.hasDot;
        }
    }

    static baseValueToSlots(baseValue) {
        // value -> slots
        // 1 -> 16
        // 2 -> 8
        // 4 -> 4
        // 8 -> 2
        // 16 -> 1
        return 16 / value;
    }

    hasSameValue(other) {
        return this.getValue() === other.getValue();
    }

    hasOppositeDirection(other) {
        return this.isFacingUp !== other.isFacingUp;
    }
}