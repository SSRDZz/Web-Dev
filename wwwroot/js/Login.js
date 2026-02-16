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

Login_page();