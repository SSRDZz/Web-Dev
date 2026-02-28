function Show_password(){
    const show_pw_2 = document.querySelectorAll('.show-passwd');
    show_pw_2.forEach(show_pw => {
        const show_pw_icon = show_pw.firstElementChild;
        const show_pw_parent = show_pw.parentElement;
        const pw_input = show_pw_parent.firstElementChild;

        show_pw.addEventListener('click', function(){
            pw_input.type = pw_input.type === 'password' 
                ? 'text' 
                : 'password'; // check type and return it
            
            show_pw_icon.src = pw_input.type === 'password' 
                ? "/image/eye_open.svg"
                : '/image/eye_closed.svg';

            // console.log(show_pw_icon.src);
        })

    })
}

function Css_submit(valid){
    const sub_button = document.querySelector('.submit-button');
    sub_button.disabled = !valid ;
    console.log("now button is :",valid)
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



function Disable_submit(){
    const input_box = document.querySelectorAll('input[required]');
    input_box.forEach(function(input){       
        input.addEventListener('input', check_form);
    })
    check_form();
}


function check_form(){
    const input_box = document.querySelectorAll('input[required]');
    let valid = true;
    input_box.forEach(function(input){
        if(!input.value.trim() || !input.checkValidity()){ // trim for check " " -> string spacebar
            valid = false;
        }
    })
    Css_submit(Check_pwd() && valid);
}

function Check_pwd(){
    const pwd = document.querySelector("#pwd-1").value;
    const con_pwd = document.querySelector("#pwd-2").value;

    let valid = true
    let show_err = false;

    // console.log(pwd , con_pwd);

    if(pwd=="" || con_pwd==""){
        valid = false;
        show_err = false;
    }
    else if ( pwd != con_pwd ){
        valid = false;
        show_err = true;
    }
    
    document.querySelector("#pwd_not_match").style.display = "none";
    if(show_err){
        document.querySelector("#pwd_not_match").style.display = "block";
    }
    return valid
} 


// function Compare_pwd(){

//     const pwd_input = document.querySelectorAll(".pwd_input_box");

//     pwd_input.forEach(function(input){
//         input.addEventListener('input', function(){

//             Css_submit(check_form() && Check_pwd());
//         })
//     })
// }


Disable_submit();   
// Compare_pwd()
Show_password();