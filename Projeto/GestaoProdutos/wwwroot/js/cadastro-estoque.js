$(function () {

    // Load Header
    $('#header').load('/views/header.html', function () {
        $('.nav-link:contains("Estoque")').closest('.nav-item').addClass('active');
    });


    var loadForm = function (estoque) {
        $('#data').val(new Date(estoque.data).toISOString().substring(0, 10));
        $('#empresa').val(estoque.empresa.nome);
        $('#produto').val(estoque.produto.nome);
        $('#entrada').val(estoque.entrada);
        $('#saida').val(estoque.saida);
        $('#estoque').val(estoque.qtda);
        $('#produto-id').val(estoque.produtoId);
        $('#empresa-id').val(estoque.empresaId);

    }

    // Obtém o id
    var id = getUrlParameter('id');

    if (id) {
        estoqueApi.get(id)
            .done(function (estoque) {
                loadForm(estoque);
            });
    }

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
        empresasApi.getAll({ query: suggestion })
            .done(function (data) {
                $('#empresa-id').val(data.results[0].id);
            });
    });
    $('#empresa').bind('typeahead:change', function (ev, value) {
        // Seta o id no input produto-id
        if (!value)
            $('#empresa-id').val('');
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
        produtosApi.getAll({ query: suggestion })
            .done(function (data) {
                $('#produto-id').val(data.results[0].id);
            });
    });
    $('#produto').bind('typeahead:change', function (ev, value) {
        // Seta o id no input produto-id
        if (!value)
            $('#produto-id').val('');
    });

    $('#form-estoque').on('submit', function (ev) {
        ev.preventDefault();

        var isValid = document.getElementById('form-estoque').checkValidity();
        $(this).addClass('was-validated');
        if (!isValid) return;

        var formData = getFormData($(this));

        if (!formData.empresaId) {
            Swal.fire(
                'Inválido!',
                'Informe uma empresa válida!',
                'error'
            );
            return;
        }

        if (!formData.produtoId) {
            Swal.fire(
                'Inválido!',
                'Informe um produto válido!',
                'warning'
            );
            return;
        }

        delete formData.empresa;
        delete formData.produto;
        var xhr;
        if (id) {
            formData.id = id;
            xhr = estoqueApi.put(id, formData);
        } else {
            xhr = estoqueApi.post(formData);
        }

        xhr.done(function () {
            window.location.href = '/views/estoque.html';
        }).fail(function (jqXHR, textStatus) {
            Swal.fire(
                'Erro ao salvar registro',
                jqXHR.responseText || textStatus,
                'error'
            );
        });


    });


});