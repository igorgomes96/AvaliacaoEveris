$(function () {
    // Load Header
    $('#header').load('/views/header.html', function () {
        $('.nav-link:contains("Produtos")').closest('.nav-item').addClass('active');
    });

    var pageSize = 5;
    var page = 1;
    var ordem = 'nome';
    var desc = false;
    var filtroNome = null;

    var loadProdutos = function () {
        var params = {
            pageSize: pageSize,
            page: page,
            ordem: ordem,
            desc: desc
        };
        if (filtroNome) params.query = filtroNome;
        produtosApi.getAll(params)
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

                $('#produto-table tbody').empty();
                $.each(data.results, function (i, produto) {
                    $template = $('#template-produto');
                    $('#produto-table tbody').append(Mustache.render($template.html(), produto));
                });
            });
    }

    $('.pagination').on('click', '.page-link', function () {
        page = $(this).html();
        loadProdutos();
    });

    $('#filtroNome').on('input', function () {
        filtroNome = $(this).val();
        if (!filtroNome || filtroNome.length < 3) filtroNome = null;
        loadProdutos();
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

        loadProdutos();

    });

    $('#btn-novo').on('click', function () {
        $('#card-cadastro-produto').parent().css('display', 'initial');
        $('#codigo').attr('disabled', false);
        formReset('form-produto');
        scrollToId('card-cadastro-produto');
    });

    var hideForm = function() {
        $('#card-cadastro-produto').parent().css('display', 'none');
    }

    $('#btn-cancelar').on('click', function() {
        hideForm();
    });

    $('#form-produto').on('submit', function (ev) {

        ev.preventDefault();

        var isValid = document.getElementById('form-produto').checkValidity();
        $(this).addClass('was-validated');
        if (!isValid) return;

        var formData = getFormData($(this));
        var xhr;
        if ($('#codigo').attr('disabled') === 'disabled') {
            xhr = produtosApi.put(formData.id, formData);
        } else {
            xhr = produtosApi.post(formData);
        }
        xhr.done(function () {
            Swal.fire(
                'Successo!',
                'Produto salvo com sucesso!',
                'success'
            );
            formReset('form-produto');
            hideForm();
            loadProdutos();
        }).fail(function (jqXHR, textStatus, errorThrown) {
            Swal.fire(
                'Erro ao salvar produto',
                jqXHR.responseText || textStatus,
                'error'
            );
        });

        ev.preventDefault();

    });

    $('#table-produto tbody').on('click', '.btn-delete', function () {
        Swal.fire({
            title: 'Confirmar Exclusão',
            text: 'Ao excluir esse produto todos os seus registros de estoque também serão excluídos.',
            type: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            confirmButtonText: 'Confirmar'
        }).then((result) => {
            if (result.value) {
                var id = $(this).attr('data-id');
                produtosApi.delete(id)
                    .done(function () {
                        Swal.fire(
                            'Successo!',
                            'Produto excluído com sucesso!',
                            'success'
                        );
                        loadProdutos();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        Swal.fire(
                            'Erro ao salvar produto',
                            jqXHR.responseText || textStatus,
                            'error'
                        );
                    });
            }
        });

    });

    $('#table-produto tbody').on('click', '.btn-edit-produto', function () {
        var id = $(this).attr('data-id');
        produtosApi.get(id)
            .done(function (produto) {
                $('#card-cadastro-produto').parent().css('display', 'initial');
                $('#codigo').attr('disabled', true);
                scrollToId('card-cadastro-produto');
                $('#codigo').val(id);
                $('#nome').val(produto.nome);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                Swal.fire(
                    'Erro ao buscar produto',
                    jqXHR.responseText || textStatus,
                    'error'
                );
            });
    });

    loadProdutos();

});