function validateForm() {
    debugger;
    if ($('input#checkNewPass').val() != "" && $('input#newPass').val() != "" && $('input#newPass').val() == $('input#checkNewPass').val()) {
        if ($('input#newPass').val().length < 6) {
            $('div#newPassValidMessage').text('');
            $('div#newPassCheckValidMessage').text('');
            $('div#newPassValidMessage').text('Пароль повинен містити мінімум 6 символів');
            return false;
        }
    } else {
        if ($('input#newPass').val() != "" && $('input#checkNewPass').val() != "") {
            $('div#newPassValidMessage').text('');
            $('div#newPassCheckValidMessage').text('');
            $('div#newPassCheckValidMessage').text('Паролі не співпадають');
            return false;
        }
        else {
            if ($('input#checkNewPass').val() == "") {
                $('div#newPassValidMessage').text('');
                $('div#newPassCheckValidMessage').text('');
                $('div#newPassCheckValidMessage').text('Поле повинно бути заповнене');
                return false;
            }
            else {
                $('div#newPassValidMessage').text('');
                $('div#newPassCheckValidMessage').text('');
                $('div#newPassValidMessage').text('Поле повинно бути заповнене');
                return false;
            }
        }
    }

    $('div#newPassValidMessage').text('');
    $('div#newPassCheckValidMessage').text('');

    return true;
}

