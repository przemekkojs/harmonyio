export async function playTask(taskObject) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const response = await fetch('/Browse?handler=PlayFile', {
            method: 'POST',
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify({
                SolutionIndex: currentIndex,
                Task: taskObject
            })
        })
            .then(response => response.blob())
            .then(blob => {
                const reader = new FileReader();
                reader.onload = function (e) { MIDIjs.play(e.target.result); };
                reader.readAsDataURL(blob);
            })
            .catch(error => console.error('Error fetching MIDI file:', error));
    }
    catch (error) {
        console.error(error)
    }
}