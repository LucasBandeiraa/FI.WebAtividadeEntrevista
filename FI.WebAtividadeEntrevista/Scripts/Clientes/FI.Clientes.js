
let beneficiariosList = [];
var currentBeneficiarioIndex = 0;

$(document).ready(function () {
    $("#CPF").inputmask("mask", { "mask": "999.999.999-99" }, { reverse: true });
    $("#Telefone").inputmask("mask", { "mask": "(99) 9999-99999" });
    $("#cpfBenef").inputmask("mask", { "mask": "999.999.999-99" }, { reverse: true });

    $('#formCadastro').submit(function (e) {
        e.preventDefault();
        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "CPF": $(this).find("#CPF").val(),
                "Beneficiarios": beneficiariosList
            },
            error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
            success:
                function (r) {
                    ModalDialog("Sucesso!", r)
                    $("#formCadastro")[0].reset();
                }
        });
    })

    $('#formIncluirBenef').submit(function (e) {
        e.preventDefault();

        const cpf = $(this).find("#cpfBenef").val();
        const nome = $(this).find("#nomeBenef").val();

        beneficiariosList.push({ Nome: nome, CPF: cpf });

        $('#gridBenef > tbody:last-child').append(`
            <tr id="${beneficiariosList.length - 1}">
                <td>${nome}</td>
                <td>${cpf}</td>
                <td>
                    <input type="button" class="btn btn-primary" value="Alterar" onclick="Alterar('${nome}','${cpf}',${beneficiariosList.length - 1})"/>
                    <input type="button" class="btn btn-primary" value="Excluir" onclick="Excluir(${beneficiariosList.length - 1 })"/>
                </td>
            </tr>`
        );

        ClearForm();
    })
})

function AtualizarTabela() {
    const cpf = $("#cpfBenef").val();
    const nome = $("#nomeBenef").val();

    beneficiariosList[currentBeneficiarioIndex].Nome = nome;
    beneficiariosList[currentBeneficiarioIndex].CPF = cpf;

    $(`#${currentBeneficiarioIndex} td:nth-child(1)`).text(nome);
    $(`#${currentBeneficiarioIndex} td:nth-child(2)`).text(cpf);
}

function ClearForm() {
    const cpf = $("#cpfBenef").val(null);
    const nome = $("#nomeBenef").val(null);
}

function Alterar(nome, cpf, index) {
    currentBeneficiarioIndex = index;
    const cpf2 = $("#cpfBenef").val(cpf);
    const nome2 = $("#nomeBenef").val(nome);
}

function Excluir(id) {
    $(`#${id}`).remove();
}

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}