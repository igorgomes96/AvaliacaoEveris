var produtosApi = (function() {

    var resourceUrl = config.apiUrl + 'produtos';

    var getAll = function(params) {
        return $.ajax({
            method: 'get',
            url: resourceUrl,
            data: params
        });
    }

    var get = function(id) {
        return $.ajax({
            method: 'get',
            url: `${resourceUrl}/${id}`
        });
    }

    var post = function(data) {
        return $.ajax({
            method: 'post',
            url: resourceUrl,
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    }

    var put = function(id, data) {
        return $.ajax({
            method: 'put',
            url: `${resourceUrl}/${id}`,
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    }

    var del = function(id) {
        return $.ajax({
            method: 'delete',
            url: `${resourceUrl}/${id}`
        });
    }

    return {
        resourceUrl: resourceUrl,
        getAll: getAll,
        get: get,
        post: post,
        put: put,
        delete: del
    }
})();