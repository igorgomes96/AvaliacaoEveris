using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Helpers
{
    public static class Util
    {
        /// <summary>
        /// Processa uma lista de arquivos (upload)
        /// </summary>
        /// <param name="files">Arquivos recebidos (upload)</param>
        /// <param name="funcao">Função que processa cada arquivo - recebe o path do arquivo como argumento</param>
        public static void ProcessFiles(IFormFileCollection files, Action<string> funcao)
        {
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    var newFile = Path.ChangeExtension(filePath, Path.GetExtension(formFile.FileName));
                    File.Move(filePath, newFile);  //Renomeia o arquivo para adicionar a extensão correta
                    funcao(newFile);
                    File.Delete(filePath);
                }
            }
        }
    }
}
