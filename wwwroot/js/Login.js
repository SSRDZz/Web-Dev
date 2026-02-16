function Login_page(){
    
    document.addEventListener('DOMContentLoaded',function(){
        console.log(window.loginFailed);
        if (window.loginFailed){

            const el = document.createElement('div');
            el.className = 'login-error';
            el.textContent = "Login failed";
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
            : 'image/eye_closed.svg'
    })
}

Show_password();
Login_page();
