document.addEventListener("DOMContentLoaded", () => {
    const tunerStatus = document.getElementById("tunerStatus");
    const tunerName = document.getElementById("tunerName");

    if (!tunerStatus || !tunerName) {
        return;
    }

    function updateTunerInput() {
        const enabled = tunerStatus.value === "With";

        tunerName.disabled = !enabled;

        if (!enabled) {
            tunerName.value = "";
        }
    }

    tunerStatus.addEventListener("change", updateTunerInput);

    updateTunerInput();
});