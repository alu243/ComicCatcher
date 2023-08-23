function addCookie(key, value) {
    document.cookie = `${key}=${value};path=/;`;
}

function addLoginCookie() {
    var newuserid = document.querySelector('#userid').value;
    document.cookie = `userid=${newuserid};path=/;`;
    location.reload();
}
function deleteLoginCookie() {
    document.cookie = `userid=;path=/;`;
    location.reload();
}

function getCookie(cookieKey) {
    let cookieName = `${cookieKey}=`;
    let cookieArray = document.cookie.split(';');

    for (let cookie of cookieArray) {

        while (cookie.charAt(0) == ' ') {
            cookie = cookie.substring(1, cookie.length);
        }

        if (cookie.indexOf(cookieName) == 0) {
            return cookie.substring(cookieName.length, cookie.length);
        }
    }
}