$(function () {
    // Load Header
    $('#header').load('/views/header.html', function () {
        $('.nav-link:contains("Empresas")').closest('.nav-item').addClass('active');
    });

    var pageSize = 5;
    var page = 1;
    var ordem = 'nome';
    var desc = false;
    var filtroNome = null;

    var loadEmpresas = function () {
        var params = {
            pageSize: pageSize,
            page: page,
            ordem: ordem,
            desc: desc
        };
        if (filtroNome) params.query = filtroNome;
        empresasApi.getAll(params)
            .done(function (data) {
                var $pagination = $('.pagination');
                $pagination.empty();
                if (data.pageCount > 1) {
                    for (let i = 1; i <= data.pageCount; i++) {
                        var template = $('#template-page').html();
                        var $page = $(Mustache.render(template, { i: i }));
                        if (i == page)
                            $page.addClass('active');
                        $pagination.append($page);

                    }
                }

                $('#empresa-table tbody').empty();
                $.each(data.results, function (i, empresa) {
                    $template = $('#template-empresa');
                    $('#empresa-table tbody').append(Mustache.render($template.html(), empresa));
                });
            });
    }

    $('.pagination').on('click', '.page-link', function () {
        page = $(this).html();
        loadEmpresas();
    });

    $('th.orderable').on('click', function () {
        var hasAsc = $(this).hasClass('asc');
        $('th.orderable').each(function () {
            $(this).removeClass(['asc', 'desc']);
        });

        ordem = $(this).html();
        if (hasAsc) {
            $(this).addClass('desc');
            $(this).removeClass('asc');
            desc = true;
        } else {
            $(this).addClass('asc');
            $(this).removeClass('desc');
            desc = false;
        }

        loadEmpresas();

    });

    var hideForm = function() {
        $('#card-cadastro-empresa').parent().css('display', 'none');
    }

    $('#btn-novo').on('click', function () {
        $('#card-cadastro-empresa').parent().css('display', 'initial');
        formReset('form-empresa');
        scrollToId('card-cadastro-empresa');
    });

    $('#btn-cancelar').on('click', function() {
        hideForm();
    });

    $('#form-empresa').on('submit', function (ev) {

        ev.preventDefault();

        var isValid = document.getElementById('form-empresa').checkValidity();
        $(this).addClass('was-validated');
        if (!isValid) return;

        var formData = getFormData($(this));
        var xhr;
        if (!formData.id) {
            delete formData.id;
            xhr = empresasApi.post(formData);
        } else {
            xhr = empresasApi.put(formData.id, formData);
        }
        xhr.done(function () {
            Swal.fire(
                'Successo!',
                'Empresa salva com sucesso!',
                'success'
            );
            formReset('form-empresa');
            hideForm();
            loadEmpresas();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Swal.fire(
                'Erro ao salvar empresa',
                jqXHR.responseText || textStatus,
                'error'
            );
        });
    });

    $('#filtroNome').on('input', function () {
        filtroNome = $(this).val();
        if (!filtroNome || filtroNome.length < 3) filtroNome = null;
        loadEmpresas();
    });

    $('#table-empresa tbody').on('click', '.btn-delete', function () {
        Swal.fire({
            title: 'Confirmar Exclusão',
            text: 'Ao excluir essa empresa todos os seus registros de estoque também serão excluídos.',
            type: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            confirmButtonText: 'Confirmar'
        }).then((result) => {
            if (result.value) {
                var id = $(this).attr('data-id');
                empresasApi.delete(id)
                    .done(function () {
                        Swal.fire(
                            'Successo!',
                            'Empresa excluída com sucesso!',
                            'success'
                        );
                        loadEmpresas();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        Swal.fire(
                            'Erro ao salvar empresa',
                            jqXHR.responseText || textStatus,
                            'error'
                        );
                    });
            }
        });

    });

    $('#table-empresa tbody').on('click', '.btn-edit-empresa', function () {
        var id = $(this).attr('data-id');
        empresasApi.get(id)
            .done(function (empresa) {
                $('#card-cadastro-empresa').parent().css('display', 'initial');
                scrollToId('card-cadastro-empresa');
                $('#codigo').val(id);
                $('#nome').val(empresa.nome);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                Swal.fire(
                    'Erro ao buscar empresa',
                    jqXHR.responseText || textStatus,
                    'error'
                );
            });
    });

    loadEmpresas();

});