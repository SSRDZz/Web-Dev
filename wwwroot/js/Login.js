function Login_page(){
    
    document.addEventListener('DOMContentLoaded',function(){
        // console.log(window.loginFailed);
        if (window.loginFailed){

            const el = document.createElement('div');
            el.className = 'login-error';
            el.textContent = "Username or Password is wrong.";

            document.querySelector('form.Login').prepend(el);
        }
    })
}

function Show_password(){
    const show_pw = document.querySelector('#show-passwd');
    const show_pw_icon = show_pw.querySelector('img');
    const pw_input = document.querySelector('#password');

    show_pw.addEventListener('click', function(){
        pw_input.type = pw_input.type === 'password' 
            ? 'text' 
            : 'password'; // check type and return it
        
        show_pw_icon.src = pw_input.type === 'password' 
            ? 'image/eye_open.svg' 
            : 'image/eye_closed.svg';
    })
}


function Disable_submit(){
    const input_box = document.querySelectorAll('input[required]');
    const sub_button = document.querySelector('.submit-button');

    function check_form(){
        let valid = true;

        input_box.forEach(function(input){
            if(!input.value.trim() || !input.checkValidity()){ // trim for check " " -> string spacebar
                valid = false;
            }
        })

        sub_button.disabled = valid 
        ? false
        : true;

    }

    input_box.forEach(function(input){       // ใส่ function 
        input.addEventListener('input', check_form);
    })
    check_form();
}

Disable_submit();
Show_password();
Login_page();
