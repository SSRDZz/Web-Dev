function Login_page(){
    
    document.addEventListener('DOMContentLoaded',function(){
        // console.log(window.loginFailed);
        if (window.loginFailed){

            const el = document.createElement('div');
            el.className = 'login-error';
            el.textContent = "Username or Password is wrong.";

            el.style.color = "#d93232";
            el.style.backgroundColor = "#feefef";
            el.style.padding = "10px 10px 10px 15px";
            el.style.borderRadius = "5px";
            el.style.fontSize = "14px";
            el.style.marginBottom = "10px";
            el.style.lineHeight = "18px";

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
