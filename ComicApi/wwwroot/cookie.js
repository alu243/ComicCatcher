function addCookie(key, value) {
    var dt = new Date();
    dt.setTime(dt.getTime() + (365 * 24 * 60 * 60 * 1000)); // 最久就一年 //24 * 60 * 60 * 1000
    document.cookie = `${key}=${value};expires=${dt.toGMTString()};path=/;`;
}

function addLoginCookie() {
    var dt = new Date();
    dt.setTime(dt.getTime() + (365 * 24 * 60 * 60 * 1000)); // 最久就一年 //24 * 60 * 60 * 1000
    var newuserid = document.querySelector('#userid').value;
    document.cookie = `userid=${newuserid};expires=${dt.toGMTString()};path=/;`;
    location.reload();
}
function deleteLoginCookie() {
    document.cookie = `userid=;max-age=-1;path=/;`;
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