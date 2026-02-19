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
        
        // change css submit-button
        if(sub_button.disabled){
            sub_button.style.pointerEvents = "none";

            sub_button.style.backgroundColor = "#F1F1F2";
            sub_button.style.color = "#9D9D9F";
            sub_button.style.boxShadow = "none";
        }
        else {
            sub_button.style.pointerEvents = "auto";
            
            sub_button.style.backgroundColor = "#FF6F61";
            sub_button.style.color = "white";
            sub_button.style.boxShadow = "0 4px 15px rgba(255, 111, 97, 0.3)";
        }
    }

    input_box.forEach(function(input){       // ใส่ function 
        input.addEventListener('input', check_form);
    })
    check_form();
}

Disable_submit();
Show_password();
Login_page();
