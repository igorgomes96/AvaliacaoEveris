var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
    return undefined;
};

var getFormData = function ($form) {
    var obj = {};
    $('input[disabled]', $form).each(function () {
        obj[this.name] = $(this).val();
    });

    return $form.serializeArray().reduce(function (o, item) {
        o[item.name] = item.value;
        return o;
    }, obj);
}

var formReset = function (formId) {
    document.getElementById(formId).reset();
}

Date.prototype.isValid = function () {
    return this instanceof Date && !isNaN(this);
}

function scrollToId(aid) {
    var aTag = $("#" + aid);
    $('html,body').animate({ scrollTop: aTag.offset().top }, 'slow');
}

function spinner($element, loading) {
    if (loading) {
        $($element).prepend(`
            <div style="width: 100%;
                        height: 100%;
                        position: absolute;
                        background: rgba(255, 255, 255, 0.5);
                        z-index: 999;" class="spinner-container">
                <div class="sk-cube-grid"
                        style="position: absolute;
                        top: 50%;
                        left: 50%;">
                    <div class="sk-cube sk-cube1"></div>
                    <div class="sk-cube sk-cube2"></div>
                    <div class="sk-cube sk-cube3"></div>
                    <div class="sk-cube sk-cube4"></div>
                    <div class="sk-cube sk-cube5"></div>
                    <div class="sk-cube sk-cube6"></div>
                    <div class="sk-cube sk-cube7"></div>
                    <div class="sk-cube sk-cube8"></div>
                    <div class="sk-cube sk-cube9"></div>
                </div>
            </div>`);
    } else {
        $($element).children('.spinner-container').remove();
    }
}
