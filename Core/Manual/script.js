const pages = document.querySelectorAll(".menu-item");
const docs = document.getElementById("docs");

pages.forEach((item) => {
    item.addEventListener('click', active_item);
})

function active_item() {
    pages.forEach((item) => {
        item.classList.remove('is-active');
    });
    this.classList.add('is-active');

    docs.setAttribute('src', "docs/" + this.id + ".html");
    document.title = this.textContent;
}

function getPage(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split('&');
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split('=');
        if (decodeURIComponent(pair[0]) == variable) {
            return decodeURIComponent(pair[1]);
        }
    }
}

const page = getPage("page");
if (page != null) {
    pages[page].dispatchEvent(new Event("click"));
}
