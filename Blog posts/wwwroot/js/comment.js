$(() => {
    $("#name").on('input', function () {
        IsFormValid();
    });
    $("#comment").on('input', function () {
        IsFormValid();
    });
    const IsFormValid = function () {
        const name = $("#name").val();
        const comment = $("#comment").val();
        const isValid = name && comment;
        $("#submit").prop("disabled", !isValid);
    }
});