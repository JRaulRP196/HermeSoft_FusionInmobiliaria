$(function () {


    $(document).on("click", ".btn-editar", function () {

        var idUsuario = $(this).data("id");

        $.ajax({
            url: '/Usuarios/Editar',
            type: 'GET',
            data: { idUsuario: idUsuario },
            success: function (user) {
                console.log(user);
                var modal = $("#modalEditarUsuario");

                modal.find("input[name='IdUsuario']").val(user.idUsuario);
                modal.find("input[name='Nombre']").val(user.nombre);
                modal.find("input[name='Apellido1']").val(user.apellido1);
                modal.find("input[name='Apellido2']").val(user.apellido2);
                modal.find("input[name='Correo']").val(user.correo);
                modal.find("select[name='IdRol']").val(user.idRol);
                modal.find("select[name='Estado']").val(user.estado.toString());

                var modal = new bootstrap.Modal(document.getElementById('modalEditarUsuario'));
                modal.show();
            },
            error: function (error) {
                console.log("Error:", error);
                alert("No se pudo cargar el usuario");
            }
        });

    });


});