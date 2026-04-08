$(function () {

    let currentStep = 1;
    const totalSteps = 4;

    function showStep(step) {

        if (step < 1 || step > totalSteps) return;

        // Ocultar todos los contenidos
        $(".step-content").addClass("d-none");
        $(`.step-content[data-step="${step}"]`).removeClass("d-none");

        // Resetear estilos del stepper
        $(".stepper-item").removeClass("active completed");

        // Marcar pasos
        $(".stepper-item").each(function () {
            const itemStep = parseInt($(this).data("step"));

            if (itemStep < step) {
                $(this).addClass("completed");
            }
            else if (itemStep === step) {
                $(this).addClass("active");
            }
        });

        currentStep = step;
        $(document).trigger("stepChanged", [currentStep]);
    }

    // Botón siguiente
    $(document).on("click", ".next-btn", function () {

        const currentStepContent = $(`.step-content[data-step="${currentStep}"]`);
        const inputs = currentStepContent.find("input, select, textarea");

        let isValid = true;

        inputs.each(function () {
            if (!this.checkValidity()) {
                this.classList.add("is-invalid");
                isValid = false;
            } else {
                this.classList.remove("is-invalid");
            }
        });

        if (!isValid) {
            return; 
        }

        showStep(currentStep + 1); 
    });

    // Botón anterior
    $(document).on("click", ".prev-btn", function () {
        showStep(currentStep - 1);
    });

});