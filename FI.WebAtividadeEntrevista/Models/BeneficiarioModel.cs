using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    public class BeneficiarioModel
    {
        public long? Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        [Cpf(ErrorMessage = "CPF do Beneficiário inválido.")]
        public string CPF { get; set; }

        public long IdCliente { get; set; }

        public bool Excluido { get; set; } = false;
    }
}