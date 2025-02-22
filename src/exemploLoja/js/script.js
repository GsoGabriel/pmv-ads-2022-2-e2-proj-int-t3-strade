loadTheme();

const changeThemeBtn = document.querySelector("#change-theme");

// Toggle dark mode
function toggleDarkMode() {
    document.body.classList.toggle("dark");
}

// Load light or dark mode
function loadTheme() {
    const darkMode = localStorage.getItem("dark")

    if(darkMode) {
        toggleDarkMode();
    }
}

changeThemeBtn.addEventListener("change", function () {
    toggleDarkMode();

    // Save or remove dark mode
    localStorage.removeItem("dark");

    if(document.body.classList.contains("dark")) {
        localStorage.setItem("dark", 1);
    }
});


// botão cep
const form = document.getElementById("form");
const cep = document.getElementById("cep");

form.addEventListener("submit", (e) => {
  e.preventDefault();

  checkInputs();
});

function checkInputs() {
  const cepValue = cep.value;

  if (cepValue === "") {
    setErrorFor(cep, "Número cep obrigatório.");
  } else {
    setSuccessFor(cep);
  }

  const formControls = form.querySelectorAll(".form-control");

  const formIsValid = [...formControls].every((formControl) => {
    return formControl.className === "form-control success";
  });

  if (formIsValid) {
    console.log("O formulário está 100% válido!");
  }
}

function setErrorFor(input, message) {
  const formControl = input.parentElement;
  const small = formControl.querySelector("small");

  // Adiciona a mensagem de erro
  small.innerText = message;

  // Adiciona a classe de erro
  formControl.className = "form-control error";
}

function setSuccessFor(input) {
  const formControl = input.parentElement;

  // Adicionar a classe de sucesso
  formControl.className = "form-control success";
}

// HTTP
const getBtn = document.querySelector("#getTransportadora");
const modal = document.querySelector("dialog");
const closeModal = document.querySelector("dialog button");

var consoleStorage = [];

function getTransportadora(){
  const cepValue = cep.value;

  if (cepValue === "") {
    setErrorFor(cep, "Número cep obrigatório.");
  } else {
    modal.showModal();
    fetch('https://localhost:7292/v1/controller/transportadora/')
    .then(data => data.json())
    .then(response => console.log(response));
  }
}
closeModal.onclick = function(){
  modal.close();
}

getBtn.addEventListener('click', function() {
    getTransportadora();
})