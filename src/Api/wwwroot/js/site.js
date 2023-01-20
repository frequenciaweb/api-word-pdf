const uri = 'api/todoitems';
let todos = [];


function upload() {
    var input = document.querySelector('input[type="file"]')

    var data = new FormData()
    data.append('file', input.files[0])
    data.append('user', 'hubot')

    fetch('https://localhost:7102/api/Conversor/', {
        method: 'POST',
        body: data
    })
}