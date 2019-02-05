$(function () {

    // Load Header
    $('#header').load('/views/header.html', function () {
        $('.nav-link:contains("Estoque")').closest('.nav-item').addClass('active');
    });


    var pageSize = 5;
    var page = 1;
    var data = null;
    var empresa = null;
    var produto = null;
    var ordem = 'data';
    var desc = false;

    var loadEstoque = function () {
        var params = {
            pageSize: pageSize,
            page: page,
            ordem: ordem,
            desc: desc
        };
        if (data) params.data = data.toISOString().substring(0, 10);
        if (empresa) params.empresa = empresa;
        if (produto) params.produto = produto;
        estoqueApi.getAll(params)
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

                $('#estoque-table tbody').empty();
                $.each(data.results, function (i, estoque) {
                    estoque.data = moment(estoque.data).format('DD/MM/YYYY');
                    $template = $('#template-estoque');
                    $('#estoque-table tbody').append(Mustache.render($template.html(), estoque));
                });
            });
    }

    $('.pagination').on('click', '.page-link', function () {
        page = $(this).html();
        loadEstoque();
    });

    $('#planilha').on('change', function () {
        var files = $('#planilha')[0].files;
        if (!files || !files.length) return;

        spinner('.card', true);
        estoqueApi.fileImport($('#planilha')[0].files[0])
            .done(function () {
                Swal.fire(
                    'Successo!',
                    'Importação realizada com successo!',
                    'success'
                );
                loadEstoque();
            }).fail(function (jqXHR, textStatus) {
                Swal.fire(
                    'Erro ao realizar a importação!',
                    jqXHR.responseText || textStatus,
                    'error'
                );
            }).always(function () {
                $('#planilha').val('');
                spinner('.card', false);
            });
    });

    $('#table-estoque').on('click', '.btn-delete', function () {
        Swal.fire({
            title: 'Confirmar Exclusão',
            text: 'Você não poderá recuperar esse registro posteriormente.',
            type: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            confirmButtonText: 'Confirmar'
        }).then((result) => {
            if (result.value) {
                var id = $(this).attr('data-id');
                estoqueApi.delete(id)
                    .done(function () {
                        loadEstoque();
                    });
            }
        });
    });

    $('#data').on('blur', function () {
        data = new Date($(this).val());
        if (!data.isValid()) data = null;
        loadEstoque();
    });

    $('#empresa').on('input', function () {
        empresa = $(this).val();
        loadEstoque();
    });

    $('#produto').on('input', function () {
        produto = $(this).val();
        loadEstoque();
    });

    // Typeahead
    var empresas = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: empresasApi.resourceUrl + '?query=%QUERY',
            wildcard: '%QUERY',
            transform: function (data) {
                var empresasNomes = data.results.map(function (empresa) {
                    return empresa.nome;
                });
                return empresasNomes;
            }
        }
    });
    $('#empresa').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    }, { source: empresas });
    $('#empresa').bind('typeahead:select', function (ev, suggestion) {
        // Seta o id no input empresa-id
        empresa = $(this).val();
        loadEstoque();
    });

    var produtos = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: produtosApi.resourceUrl + '?query=%QUERY',
            wildcard: '%QUERY',
            transform: function (data) {
                var produtosNomes = data.results.map(function (produto) {
                    return produto.nome;
                });
                return produtosNomes;
            }
        }
    });

    $('#produto').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    }, { source: produtos });
    $('#produto').bind('typeahead:select', function (ev, suggestion) {
        // Seta o id no input produto-id
        produto = $(this).val();
        loadEstoque();
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

        loadEstoque();

    });

    loadEstoque();

});