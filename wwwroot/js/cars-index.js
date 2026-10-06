document.addEventListener("DOMContentLoaded", () => {
    initializeTunerFilter();
    initializeNumericFilters();
    initializeClearFilters();
    initializeSorting();
    initializeScrollRestore();
    initializeFilterSubmit();
    initializePagination();
    initializePageSize();
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
                    mode === "GreaterThan";

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

function initializeSorting() {
    const form = document.getElementById("carsFilterForm");
    const sortField = document.getElementById("sortField");
    const sortDirection = document.getElementById("sortDirection");

    if (!form || !sortField || !sortDirection) {
        return;
    }

    document.querySelectorAll(".sort-header")
        .forEach(button => {

            button.addEventListener("click", () => {
                const clickedField = button.dataset.sortField;

                if (sortField.value === clickedField) {
                    sortDirection.value =
                        sortDirection.value === "Ascending"
                            ? "Descending"
                            : "Ascending";
                } else {
                    sortField.value = clickedField;
                    sortDirection.value = "Ascending";
                }

                saveScrollPosition();

                form.submit();
            });
        });
}

function initializeClearFilters() {
    const clearButton = document.getElementById("clearFilters");

    if (!clearButton) {
        return;
    }

    clearButton.addEventListener("click", () => {
        saveScrollPosition();
    });
}

function initializeScrollRestore() {
    restoreScrollPosition();

    const form = document.getElementById("carsFilterForm");

    if (form) {
        form.addEventListener("submit", saveScrollPosition);
    }
}

function saveScrollPosition() {
    sessionStorage.setItem("carsScrollY", window.scrollY.toString());
}

function restoreScrollPosition() {
    const scrollY = sessionStorage.getItem("carsScrollY");

    if (scrollY !== null) {
        window.scrollTo(0, Number(scrollY));
        sessionStorage.removeItem("carsScrollY");
    }
}

function initializeFilterSubmit() {
    const form = document.getElementById("carsFilterForm");
    const pageNumber = document.getElementById("pageNumber");

    if (!form || !pageNumber) {
        return;
    }

    form.addEventListener("submit", () => {
        pageNumber.value = "1";
        saveScrollPosition();
    });
}

function initializePagination() {
    const form = document.getElementById("carsFilterForm");
    const pageNumber = document.getElementById("pageNumber");

    if (!form || !pageNumber) {
        return;
    }

    document.querySelectorAll(".pagination-button")
        .forEach(button => {
            button.addEventListener("click", () => {
                pageNumber.value = button.dataset.page;

                saveScrollPosition();

                form.submit();
            });
        });
}

function initializePageSize() {
    const form = document.getElementById("carsFilterForm");
    const pageNumber = document.getElementById("pageNumber");
    const pageSize = document.getElementById("pageSize");

    if (!form || !pageNumber || !pageSize) {
        return;
    }

    pageSize.addEventListener("change", () => {
        pageNumber.value = "1";

        saveScrollPosition();

        form.submit();
    });
}