$(function () {
    // Load Header
    $('#header').load('/views/header.html', function () {
        $('.nav-link:contains("Home")').closest('.nav-item').addClass('active');
    });

});