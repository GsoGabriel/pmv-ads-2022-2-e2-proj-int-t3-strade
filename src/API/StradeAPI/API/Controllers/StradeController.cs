﻿using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QueryTypes.Controllers
{
    [ApiController]
    [Route("v1/controller")]
    public class StradeController : ControllerBase
    {
        [HttpGet]
        [Route("transportadora")]
        public async Task<ActionResult<List<TransportadoraDTO>>> GetTransportadoras([FromServices] DataContext context){

            var transportadoras = await (from trans in context.Transportadoras
                                         let bairrosVinculados = context.BairroTransportadoras
                                                .Where(bt => bt.IdTransportadora == trans.IdTransportadora)
                                                .ToList()
                                         let info = context.Informacaos
                                                .FirstOrDefault(i => i.IdInformacao == trans.IdInformacao)
                                         select new TransportadoraDTO() {

                                             IdInformacao = info.IdInformacao,
                                             Nome = info.Nome,
                                             Email = info.Email,
                                             Endereco = info.Endereco,
                                             Aniversario = info.Aniversario,
                                             NumeroContato = info.NumeroContato,

                                             IdTransportadora = trans.IdTransportadora,
                                             Cnpj = trans.Cnpj,
                                             MediaPreco = trans.MediaPreco,
                                             NotaMediaQualidade = trans.NotaMediaQualidade,
                                             
                                             Bairros =  (from b in context.Bairros
                                                        join bv in bairrosVinculados on b.IdBairro equals bv.IdBairro
                                                        select new BairroDTO() {
                                                            IdBairro = b.IdBairro,
                                                            Cep = b.Cep,
                                                            Nome = b.Nome,
                                                        }).ToList(),

                                         }).ToListAsync();
                                         
            if(transportadoras != null && transportadoras.Count > 0) {
                return Ok(transportadoras);
            }

            return NotFound("Não existe transportadoras.");
        }

        [HttpPost]
        [Route("transportadora")]
        public async Task<ActionResult<int>> SaveTransportadora([FromServices] DataContext context, [FromBody] TransportadoraDTO transportadoraDto){

            var informacao = new Informacao() {
                Nome = transportadoraDto.Nome,
                IdInformacao = transportadoraDto.IdInformacao,
                Aniversario = transportadoraDto.Aniversario,
                Email = transportadoraDto.Email,
                Endereco = transportadoraDto.Endereco,
                NumeroContato = transportadoraDto.NumeroContato
            };

            context.Informacaos.Add(informacao);
            await context.SaveChangesAsync();

            var transportadora = new Transportadora() {
                Cnpj = transportadoraDto.Cnpj,
                IdInformacao = informacao.IdInformacao,
                MediaPreco = transportadoraDto.MediaPreco,
                NotaMediaQualidade = transportadoraDto.NotaMediaQualidade,
            };

            context.Transportadoras.Add(transportadora);
            await context.SaveChangesAsync();

            await SaveBairros(context, transportadora.IdTransportadora, transportadoraDto.Bairros);

            return Ok(transportadora.IdTransportadora);
        }

        [HttpPost]
        [Route("bairros/{idTransportadora}")]
        public async Task<ActionResult<bool>> SaveBairros([FromServices] DataContext context, int idTransportadora, [FromBody] List<BairroDTO> bairrosDto){

            var bairros = new List<Bairro>();
            foreach(var bairro in bairrosDto)
                bairros.Add(new Bairro() {
                    IdBairro = bairro.IdBairro,
                    Nome = bairro.Nome,
                    Cep = bairro.Cep,
                });

            context.Bairros.AddRange(bairros);
            await context.SaveChangesAsync();

            var bairrosVinculados = new List<BairroTransportadora>();
            foreach(var bairro in bairros) {
                bairrosVinculados.Add(new BairroTransportadora() { IdTransportadora = idTransportadora,IdBairro = bairro.IdBairro });
            }

            context.BairroTransportadoras.AddRange(bairrosVinculados);
            await context.SaveChangesAsync();

            return Ok(true);
        }

        [HttpGet]
        [Route("bairros")]
        public async Task<ActionResult<List<BairroDTO>>> GetBairros([FromServices] DataContext context){
            var bairros = await context.Bairros.ToListAsync();
            return Ok(bairros.Select(b => new List<BairroDTO>() { 
                new BairroDTO() { 
                    IdBairro = b.IdBairro,
                    Cep = b.Cep,
                    Nome = b.Nome,
            }}));
        }
    }
}
