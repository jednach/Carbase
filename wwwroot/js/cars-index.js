document.addEventListener("DOMContentLoaded", () => {
    initializeTunerFilter();
    initializeNumericFilters();
});

function initializeTunerFilter() {
    const tunerStatus = document.getElementById("tunerStatus");
    const tunerName = document.getElementById("tunerName");

    if (!tunerStatus || !tunerName) {
        return;
    }

    function update() {
        const enabled = tunerStatus.value === "With";

        tunerName.disabled = !enabled;

        if (!enabled) {
            tunerName.value = "";
        }
    }

    tunerStatus.addEventListener("change", update);

    update();
}

function initializeNumericFilters() {
    document.querySelectorAll(".numeric-filter")
        .forEach(filter => {

            const type =
                filter.querySelector(".numeric-filter-type");

            const singleContainer =
                filter.querySelector(".numeric-single");

            const betweenContainer =
                filter.querySelector(".numeric-between");

            const value =
                filter.querySelector(".numeric-value");

            const from =
                filter.querySelector(".numeric-from");

            const to =
                filter.querySelector(".numeric-to");

            if (!type) {
                return;
            }

            function update(clearUnused) {
                const mode = type.value;

                const showSingle =
                    mode === "LessThan" ||
                    mode === "MoreThan";

                const showBetween =
                    mode === "Between";

                singleContainer.classList.toggle(
                    "d-none",
                    !showSingle);

                betweenContainer.classList.toggle(
                    "d-none",
                    !showBetween);

                value.disabled = !showSingle;
                from.disabled = !showBetween;
                to.disabled = !showBetween;

                if (clearUnused) {
                    if (!showSingle) {
                        value.value = "";
                    }

                    if (!showBetween) {
                        from.value = "";
                        to.value = "";
                    }
                }
            }

            type.addEventListener(
                "change",
                () => update(true));

            update(false);
        });
}