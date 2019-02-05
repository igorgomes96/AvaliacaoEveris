var estoqueApi = (function () {

    var resourceUrl = config.apiUrl + 'estoque';

    var getAll = function (params) {
        return $.ajax({
            method: 'get',
            url: resourceUrl,
            data: params
        });
    }

    var get = function (id) {
        return $.ajax({
            method: 'get',
            url: `${resourceUrl}/${id}`
        });
    }

    var post = function (data) {
        return $.ajax({
            method: 'post',
            url: resourceUrl,
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    }

    var put = function (id, data) {
        return $.ajax({
            method: 'put',
            url: `${resourceUrl}/${id}`,
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    }

    var del = function (id) {
        return $.ajax({
            method: 'delete',
            url: `${resourceUrl}/${id}`
        });
    }

    var fileImport = function (file) {
        var formData = new FormData();
        formData.append("file", file, file.name);

        return $.ajax({
            method: 'post',
            url: `${resourceUrl}/importacao`,
            data: formData,
            cache: false,
            contentType: false,
            processData: false
        });
    }

    return {
        resourceUrl: resourceUrl,
        getAll: getAll,
        get: get,
        post: post,
        put: put,
        delete: del,
        fileImport: fileImport
    }
})();