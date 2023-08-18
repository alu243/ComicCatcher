function addCookie() {
    var newuserid = document.querySelector('#userid').value;
    console.log('loginuser = ', newuserid);
    document.cookie = `userid=${newuserid};path=/;`;
    location.reload();
}
function addIgnoreComic(comic, comicName) {
    var request = { "comic": comic, "comicName": comicName };
    fetch("/IgnoreComic", {
        method: "POST",
        credentials: "same-origin", // 帶 cookie
        headers: { "Content-Type": "application/json"},
        body: JSON.stringify(request)
    }).then(response => {
        location.reload();
    }).catch((error) => {
        console.log(error);
        alert("哦哦，被你玩壞了");
    })
}

function deleteIgnoreComic(comic) {
    var request = { "comic": comic };
    fetch("/IgnoreComic", {
        method: "DELETE",
        credentials: "same-origin", // 帶 cookie
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request)
    }).then(response => {
        location.reload();
    }).catch((error) => {
        console.log(error);
        alert("哦哦，被你玩壞了");
    })
}


function addFavoriteComic(comic, comicName, iconUrl) {
    var request = { "comic": comic, "comicName": comicName, "iconUrl": iconUrl };
    fetch("/FavoriteComic", {
        method: "POST",
        credentials: "same-origin", // 帶 cookie
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request)
    }).then(response => {
        location.reload();
    }).catch((error) => {
        console.log(error);
        alert("哦哦，被你玩壞了");
    })
}

function deleteFavoriteComic(comic) {
    var request = { "comic": comic };
    fetch("/FavoriteComic", {
        method: "DELETE",
        credentials: "same-origin", // 帶 cookie
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request)
    }).then(response => {
        location.reload();
    }).catch((error) => {
        console.log(error);
        alert("哦哦，被你玩壞了");
    })
}