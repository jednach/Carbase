document.addEventListener("DOMContentLoaded", () => {
    initializeTunerFilter();
    initializeNumericFilters();
    initializeClearFilters();
    initializeTableScrollSync();
    initializeSorting();
    initializeScrollRestore();
    initializeFilterSubmit();
    initializeDeleteModal();
    initializePagination();
    initializePageSize();
});

function initializeTunerFilter() {
    const tunerStatus =
        document.getElementById("tunerStatus");

    const tunerName =
        document.getElementById("tunerName");

    const tunerNameContainer =
        document.getElementById("tunerNameContainer");

    if (!tunerStatus ||
        !tunerName ||
        !tunerNameContainer) {
        return;
    }

    function update(clearUnused) {
        const show =
            tunerStatus.value === "With";

        tunerNameContainer.classList.toggle(
            "d-none",
            !show);

        tunerName.disabled = !show;

        if (!show && clearUnused) {
            tunerName.value = "";
        }
    }

    tunerStatus.addEventListener(
        "change",
        () => update(true));

    update(false);
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
                    mode === "Exact" ||
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

function initializeTableScrollSync() {
    const topScroll =
        document.getElementById("tableTopScroll");

    const topScrollContent =
        document.getElementById("tableTopScrollContent");

    const tableScroll =
        document.getElementById("tableScroll");

    if (!topScroll ||
        !topScrollContent ||
        !tableScroll) {
        return;
    }

    function updateTopScrollWidth() {
        topScrollContent.style.width =
            `${tableScroll.scrollWidth}px`;
    }

    topScroll.addEventListener("scroll", () => {
        tableScroll.scrollLeft =
            topScroll.scrollLeft;
    });

    tableScroll.addEventListener("scroll", () => {
        topScroll.scrollLeft =
            tableScroll.scrollLeft;
    });

    updateTopScrollWidth();

    window.addEventListener(
        "resize",
        updateTopScrollWidth);
}

function initializeSorting() {
    const form = document.getElementById("carsFilterForm");
    const sortField = document.getElementById("sortField");
    const sortDirection = document.getElementById("sortDirection");
    const pageNumber = document.getElementById("pageNumber");

    if (!form || !sortField || !sortDirection || !pageNumber) {
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

                pageNumber.value = "1";

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

function initializeDeleteModal() {
    const deleteButtons =
        document.querySelectorAll(".delete-car-button");

    const carId =
        document.getElementById("deleteCarId");

    const carName =
        document.getElementById("deleteCarName");

    const carYear =
        document.getElementById("deleteCarYear");

    const carTuner =
        document.getElementById("deleteCarTuner");

    if (!carId ||
        !carName ||
        !carYear ||
        !carTuner) {
        return;
    }

    deleteButtons.forEach(button => {
        button.addEventListener("click", () => {
            carId.value =
                button.dataset.carId;

            carName.textContent =
                `${button.dataset.carBrand} ${button.dataset.carModel}`;

            carYear.textContent =
                button.dataset.carYear;

            carTuner.textContent =
                button.dataset.carTuner;
        });
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