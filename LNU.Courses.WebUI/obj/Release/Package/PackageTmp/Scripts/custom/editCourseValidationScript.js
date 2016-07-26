function validateForm() {
    debugger;
    if ($('.courseEditor').val() < 5 && $('.courseEditor').val() > 0) {
        $('div#newCourse').text('');
        return true;
    }

    $('div#newCourse').text('Не правильно введено дані');
    return false;
}