using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Transactions;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente clienteBo = new BoCliente();
            BoBeneficiario beneficiarioBo = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (model.beneficiarios == null)
                {
                    model.beneficiarios = new List<BeneficiarioModel>();
                }

                if (clienteBo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("cpf cliente já cadastrado");
                }

                foreach (var beneficiario in model.beneficiarios)
                {
                    if (beneficiarioBo.VerificarExistencia(beneficiario.CPF))
                    {
                        Response.StatusCode = 400;
                        return Json("cpf beneficiário já cadastrado");
                    }
                }

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        model.Id = clienteBo.Incluir(new Cliente()
                        {
                            CEP = model.CEP,
                            Cidade = model.Cidade,
                            Email = model.Email,
                            Estado = model.Estado,
                            Logradouro = model.Logradouro,
                            Nacionalidade = model.Nacionalidade,
                            Nome = model.Nome,
                            Sobrenome = model.Sobrenome,
                            Telefone = model.Telefone,
                            CPF = model.CPF,
                        });

                        // Inclusão de beneficiarios
                        foreach (var beneficiario in model.beneficiarios)
                        {
                            beneficiario.Id = beneficiarioBo.Incluir(new Beneficiario()
                            {
                                IdCliente = model.Id,
                                Nome = beneficiario.Nome,
                                CPF = beneficiario.CPF
                            });
                        }

                        transaction.Complete();
                        return Json("Cadastro efetuado com sucesso");
                    }
                    catch (Exception ex)
                    {
                        Response.StatusCode = 500;
                        return Json($"Erro ao realizar o cadastro: {ex.Message}");
                    }
                }
            }
        }


        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            BoBeneficiario boBeneficiario = new BoBeneficiario();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (model.Id == 0 && bo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("cpf cliente já cadastrado");
                }

                foreach (var beneficiario in model.beneficiarios)
                {
                    bool cpfJaCadastrado = boBeneficiario.VerificarExistencia(beneficiario.CPF, model.Id);

                    if (beneficiario.Id == null && cpfJaCadastrado)
                    {
                        Response.StatusCode = 400;
                        return Json("cpf beneficiário já cadastrado");
                    }
                }

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        bo.Alterar(new Cliente()
                        {
                            Id = model.Id,
                            CEP = model.CEP,
                            Cidade = model.Cidade,
                            Email = model.Email,
                            Estado = model.Estado,
                            Logradouro = model.Logradouro,
                            Nacionalidade = model.Nacionalidade,
                            Nome = model.Nome,
                            Sobrenome = model.Sobrenome,
                            Telefone = model.Telefone,
                            CPF = model.CPF
                        });

                        if (model.beneficiarios != null && model.beneficiarios.Any())
                        {
                            model.beneficiarios.ForEach(beneficiario =>
                            {
                                if (beneficiario.Id != null)
                                {
                                    boBeneficiario.Alterar(new Beneficiario()
                                    {
                                        Id = beneficiario.Id.Value,
                                        CPF = beneficiario.CPF,
                                        Nome = beneficiario.Nome,
                                        IdCliente = model.Id
                                    });

                                    if (beneficiario.Excluido)
                                    {
                                        boBeneficiario.Excluir(beneficiario.Id.Value);
                                    }
                                }
                                else
                                {
                                    boBeneficiario.Incluir(new Beneficiario()
                                    {
                                        CPF = beneficiario.CPF,
                                        Nome = beneficiario.Nome,
                                        IdCliente = model.Id
                                    });
                                }
                            });
                        }

                        transaction.Complete();
                        return Json("Cadastro alterado com sucesso");
                    }
                    catch (Exception ex)
                    {
                        
                        Response.StatusCode = 500;
                        return Json("Ocorreu um erro ao alterar o cadastro: " + ex.Message);
                    }
                }
            }
        }


        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            BoBeneficiario beneficiarioBo = new BoBeneficiario();
            Cliente cliente = bo.Consultar(id);
            ClienteModel model = null;

            if (cliente != null)
            {
                var beneficiarios = beneficiarioBo.Listar(id).Select(b =>
                {
                    return new BeneficiarioModel()
                    {
                        CPF = b.CPF,
                        Nome = b.Nome,
                        IdCliente = b.Id,
                        Id = b.Id
                    };
                }).ToList();

                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF,
                    beneficiarios = beneficiarios
                };
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}