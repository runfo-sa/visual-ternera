const pages = document.querySelectorAll(".menu-item");
const docs = document.querySelector("#docs");

pages.forEach((item) => {
    item.addEventListener('click', active_item);
})

function active_item() {
    pages.forEach((item) => {
        item.classList.remove('is-active');
    });
    this.classList.add('is-active');
    fetch('docs/test.html')
        .then(response => response.text())
        .then(data => {
            docs.innerHTML = data;
        })
        .catch(error => console.error('Error loading the file', error));
}
