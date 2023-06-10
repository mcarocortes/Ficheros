using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaFicheros_MCARO
{
    internal class Cliente
    {
        string nombre;
        string apellido;
        string provincia;

        public string Nombre { get => nombre; set => nombre = value; }
        public string Apellido { get => apellido; set => apellido = value; }
        public string Provincia { get => provincia; set => provincia = value; }

        //CREO MI CONSTRUCTOR DE CLIENTE
        public Cliente(string name, string surname, string prov)
        {

            this.nombre = name;
            this.apellido = surname;
            this.provincia = prov;

        }


    }
}
